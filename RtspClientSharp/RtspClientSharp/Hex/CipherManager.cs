using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;
using LibHexCryptoStandard.Algoritm;
using LibHexCryptoStandard.Hashs;
using LibHexCryptoStandard.Packet.AES;
using LibHexCryptoStandard.Packet.RSA;
using LibHexUtils.Arrays;
using LibHexUtils.Random;
using LibNet.DNS;
using LibNet.Meeting.Packets.HexPacket;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace LibRtspClientSharp.Hex
{
    public class CipherManager
    {
        private static AsymmetricCipherKeyPair sessionRsaKeyPair;
        private static byte[] hashID;
        private static byte[] AesKeyAdmin = null;
        private static byte[] AesKeyData = null;

        private static bool CanGet = false;

        public static byte[] HashId => hashID;

        public static void NewID()
        {
            //var ip = DNS.GetActualAddress().GetAddressBytes();
            sessionRsaKeyPair = RsaOAEP.GenerateKeyPair(2048);
            var vk = (RsaKeyParameters)sessionRsaKeyPair.Public;
            var toHash = ByteArray.CopyBytes(0, vk.Exponent.ToByteArray(), vk.Modulus.ToByteArray());
            
            hashID = SHA.SHA3(toHash, 256);
            Console.WriteLine("ID: " + System.Convert.ToBase64String(hashID));
        }

        public static byte[] GetDataKey()
        {
            if (CanGet)
            {
                CanGet = false;
                return AesKeyData;
            }
            else
            {
                return null;
            }
        }

        public static byte[] GetKeyToSend()
        {
            return RsaOAEP.KeyToBytes((RsaKeyParameters) sessionRsaKeyPair.Public);
        }

        public static byte[] GetControlConDet(byte[] hPkt)
        {
            HexPacketRSA hp = new HexPacketRSA(hPkt);
            return hp.Decrypt((RsaKeyParameters) sessionRsaKeyPair.Private, true);
        }

        public static byte[] EcryptAdminData(byte[] data)
        {
            try
            {
                HexPacketAES hpkt = HexPacketAES.CreatePacketForEncrypt(data, true, AesKeyAdmin);
                return (byte[])hpkt.Encrypt();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] DecryptAdminData(byte[] hpktb)
        {
            try
            {
                HexPacketAES hpkt = HexPacketAES.CreatePacketForDecrypt(hpktb, true, AesKeyAdmin);
                return (byte[]) hpkt.Decrypt();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] EncryptControl(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key.Length != 32) throw new ArgumentException("Bad key length!");

            AesKeyAdmin = key;

            var dataBytes = HexRandom.GetRandomBytesWithMessage(64, Encoding.UTF8.GetBytes("HMET AES OK"));
            var pkt = HexPacketAES.CreatePacketForEncrypt(dataBytes, true, AesKeyAdmin);
            return (byte[]) pkt.Encrypt();
        }

        private static byte[] EncodeRSA(AsymmetricKeyParameter key, byte[] data, bool encrypt, bool inHPkt)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (data == null) throw new ArgumentNullException(nameof(data));
            var conPkt = new HexPacketRSA(data);
            return encrypt ?
                conPkt.Encrypt((RsaKeyParameters)key) :
                conPkt.Decrypt((RsaKeyParameters)key, inHPkt);
        }
        public static byte[] ProcessByPrivate(byte[] data, bool encrypt, bool inHPkt)
        {
            return EncodeRSA(sessionRsaKeyPair.Private, data, encrypt, inHPkt);
        }
        public static byte[] ProcessByPublic(byte[] data, bool encrypt, bool inHPkt)
        {
            return EncodeRSA(sessionRsaKeyPair.Public, data, encrypt, inHPkt);
        }

        public static bool ParseHostConf(byte[] hpkt)
        {
            try
            {
                var pkt = HexPacketAES.CreatePacketForDecrypt(hpkt, true, AesKeyAdmin);
                var decrypted = (byte[]) pkt.Decrypt();
                if (decrypted.Length != 32) return false;
                return ByteArray.Search(decrypted, Encoding.UTF8.GetBytes("HMET AES OK")) != -1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static byte[] SendAESconf()
        {
            var l = Encoding.UTF8.GetBytes("HMET AES OK");
            var r = HexRandom.GetRandomBytesWithMessage(32, l);
            var aesPkt = HexPacketAES.CreatePacketForEncrypt(r, true, AesKeyAdmin);//new HexPacketAES(r, false, EncryptType.Encrypt);
            return (byte[]) aesPkt.Encrypt();
        }

        public static byte[] EncryptDataKeyWithIPEP(RsaKeyParameters clientKey, IPEndPoint ipep)
        {
            if (clientKey == null) throw new ArgumentNullException(nameof(clientKey));
            return EncodeRSA(clientKey,
                ByteArray.CopyBytes(0, ipep.Address.GetAddressBytes(), BitConverter.GetBytes(ipep.Port),
                    NetworkManager.ConnectionParameters.Enryption ? AesKeyData : new byte[1]),
                true, true);
        }

        public static void InitEncryption(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key.Length != 32) throw new ArgumentException("Key length is not correct!");

            AesKeyData = key;
        }

        public static void InitEncryption(string key)
        {
            if (key.Length <= 0)
            {
                throw new ArgumentException("Key is wrong");
            }

            ushort bitLen = 256;
            int byteLen = bitLen / 8;
            var srand = new SecureRandom();

            var hkey = SHA.SHA3(key, bitLen);
            var rkey = srand.GenerateSeed(byteLen);
            var toKey = new byte[rkey.Length + hkey.Length];

            Buffer.BlockCopy(rkey, 0, toKey, 0, rkey.Length);
            Buffer.BlockCopy(hkey, 0, toKey, rkey.Length, hkey.Length);
            AesKeyData = SHA.SHA3(toKey, bitLen);

            CanGet = true;
        }

        public static byte[] ProcessData(byte[] data, bool encrypt, bool hpkt)
        {
            if (AesKeyData == null) return data;
            var pkt = encrypt ? HexPacketAES.CreatePacketForEncrypt(data, hpkt, AesKeyData, NetworkManager.ConnectionParameters.UseBase64) : HexPacketAES.CreatePacketForDecrypt(data, hpkt, AesKeyData, 0, NetworkManager.ConnectionParameters.UseBase64);
            return encrypt ? (byte[])pkt.Encrypt() :(byte[])pkt.Decrypt();
        }
    }
}
