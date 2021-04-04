using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LibHexCryptoStandard.Algoritm;
using LibHexCryptoStandard.Packet.AES;
using LibHexCryptoStandard.Packet.RSA;
using LibHexUtils.Arrays;
using LibHexUtils.Random;
using LibNet.Meeting.Packets.HexPacket;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace HexServer.Net
{
    public static class CipherManager
    {
        public static RsaKeyParameters GetKey(byte[] data)
        {
            return RsaOAEP.BytesTokey(data, false);
        }

        public static byte[] GetRandBytes(byte len)
        {
            return HexRandom.GetRandomBytes(len);
        }

        public static byte[] DecryptRSA(RsaKeyParameters key, byte[] hpkt)
        {
            var pkt = new HexPacketRSA(hpkt);
            return pkt.Decrypt(key, true);
        }

        public static byte[] EncryptRSA(RsaKeyParameters key, byte[] data)
        {
            var pkt = new HexPacketRSA(data);
            return pkt.Encrypt(key);
        }

        public static byte[] AesEngine(byte[] key, byte[] data, bool encrypt)
        {
            if (encrypt)
            {
                var hAesPkt = new HexPacketAES(data, false, EncryptType.Encrypt);
                return (byte[])hAesPkt.Encrypt(key);
            }
            else
            {
                var hAesPkt = new HexPacketAES(data, false, EncryptType.DecryptPacket);
                return (byte[])hAesPkt.Decrypt(key);
            }
        }
    }
}
