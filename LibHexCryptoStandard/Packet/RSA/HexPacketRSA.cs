using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using LibHexCryptoStandard.Algoritm;
using LibHexUtils.Arrays;
using LibNet.Meeting.Packets.Exceptions;
using LibNet.Meeting.Packets.HexPacket;
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
            return HexPacket.Pack(RsaOAEP.KeyToBytes(key));
        }

        public static RsaKeyParameters GetKeyFromPacket(byte[] hPacket, bool exceptPrivateKey, bool inHpkt = true)
        {
            if (hPacket.Length <= 8 && inHpkt)
            {
                throw new PacketException("Bad array struct!");
            }

            var data = inHpkt ? HexPacket.Unpack(hPacket, 0) : hPacket;

            return RsaOAEP.BytesTokey(data, exceptPrivateKey);
        }
        #endregion

        #region Encryption
        /// <summary>
        /// Encrypt data in packet with encryptKey and pack it into HexPacket
        /// </summary>
        /// <param name="encryptKey">Key used for encryption</param>
        /// <param name="offset">Start offset of data</param>
        /// <returns>Encrypted message (OAEP)</returns>
        public byte[] Encrypt(RsaKeyParameters encryptKey, bool isInPacket = true, int offset = 0)
        {
            if (encryptKey is null)
            {
                throw new ArgumentNullException(nameof(encryptKey));
            }

            if (data is null || data.Length < 1)
            {
                throw new TypeInitializationException("HexPacketRSA", new Exception("Not initialized"));
            }
            
            return isInPacket ? HexPacket.Pack(RsaOAEP.Encrypt(encryptKey, data, offset)) : RsaOAEP.Encrypt(encryptKey, data, offset);
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
