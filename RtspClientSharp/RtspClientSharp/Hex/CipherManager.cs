using System;
using System.Collections.Generic;
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
        private static byte[] AesKeyControl = null;

        public static byte[] HashId => hashID;

        public static void NewID()
        {
            //var ip = DNS.GetActualAddress().GetAddressBytes();
            sessionRsaKeyPair = RsaOAEP.GenerateKeyPair(2048);
            var vk = (RsaKeyParameters)sessionRsaKeyPair.Public;
            var toHash = ByteArray.CopyBytes(0, vk.Exponent.ToByteArray(), vk.Modulus.ToByteArray());
            
            hashID = SHA.SHA3(toHash, 256);
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

        public static byte[] EncryptControl(byte[] plainControlBytes)
        {
            if (plainControlBytes == null) throw new ArgumentNullException(nameof(plainControlBytes));
            var h = SHA.SHA3(plainControlBytes, 256);
            HexPacketRSA conPkt = new HexPacketRSA(h);
            return conPkt.Encrypt((RsaKeyParameters)sessionRsaKeyPair.Private);
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

        public static bool ParseAndSetKey(byte[] hpkt)
        {
            try
            {
                var hpa = new HexPacketRSA(hpkt);
                var k = hpa.Decrypt((RsaKeyParameters) sessionRsaKeyPair.Private, true);
                if (k.Length != 32) return false;
                AesKeyControl = k;
                return true;
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
            var aesPkt = new HexPacketAES(r, false, EncryptType.Encrypt);
            return (byte[]) aesPkt.Encrypt(AesKeyControl);
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
            AesKeyControl = SHA.SHA3(toKey, bitLen);
        }
    }
}
