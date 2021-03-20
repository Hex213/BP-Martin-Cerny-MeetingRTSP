using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using LibHexCryptoStandard.Algoritm;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace LibHexCryptoStandard.Packet.RSA
{
    public class HexPacketRSA
    {
        private byte[] data = null;

        public HexPacketRSA(byte[] hPacket)
        {
            this.data = hPacket;
        }

        #region KeyConvertors
        public static byte[] GetKeyToPacket(RsaKeyParameters key)
        {
            return HexPacket.Pack(RsaOAEP.keyToBytes(key));
        }

        public static RsaKeyParameters GetKeyFromPacket(byte[] hPacket)
        {
            if (hPacket.Length <= 8)
            {
                throw new PacketException("Bad array struct!");
            }

            var data = HexPacket.Unpack(hPacket, 0);

            var sha256 = SHA256.Create();

            byte lenExp = data[0];
            int lenMod = data.Length - 1 - lenExp - 32;

            var exp = new byte[lenExp];
            var mod = new byte[lenMod];
            var hash = new byte[32];

            Buffer.BlockCopy(data, 1, exp, 0, lenExp);
            Buffer.BlockCopy(data, 1 + lenExp, mod, 0, lenMod);
            Buffer.BlockCopy(data, 1 + lenExp + lenMod, hash, 0, 32);

            var hashValue = sha256.ComputeHash(data, 1, lenExp + lenMod);

            if (HexPacket.Search(hash, hashValue) == -1)
            {
                throw new PacketException("Hash not correct!");
            }

            var e = new BigInteger(1, exp);
            var m = new BigInteger(1, mod);

            return new RsaKeyParameters(false, m, e);
        }
        #endregion

        #region Encryption
        public byte[] Encrypt(RsaKeyParameters encryptKey, int offset = 0)
        {
            if (encryptKey is null)
            {
                throw new ArgumentNullException(nameof(encryptKey));
            }

            if (data is null || data.Length < 1)
            {
                throw new TypeInitializationException("HexPacketRSA", new Exception("Not initialized"));
            }
            
            return HexPacket.Pack(RsaOAEP.Encrypt(encryptKey, data, offset));
        }
        #endregion

        #region Decryption
        public byte[] Decrypt(RsaKeyParameters decryptKey, bool isInPacket = false, int offset = 0)
        {
            if (decryptKey is null)
            {
                throw new ArgumentNullException(nameof(decryptKey));
            }

            if (data is null || data.Length < 1)
            {
                throw new TypeInitializationException("HexPacketRSA", new Exception("Not initialized"));
            }

            if (isInPacket)
            {
                data = HexPacket.Unpack(data, 0);
            }

            return RsaOAEP.Decrypt(decryptKey, data, offset);
        }
        #endregion
    }
}
