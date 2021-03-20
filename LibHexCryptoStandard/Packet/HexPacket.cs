using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;

using c = LibHexCryptoStandard.Packet.HexPacketConstants;

namespace LibHexCryptoStandard.Packet
{
    public enum EncryptType
    {
        None,
        Encrypt,
        Decrypt,
        DecryptPacket
    }

    public class HexPacket
    {
        /// <summary>
        /// Find sequence in byte array.
        /// </summary>
        /// <param name="src">Input data</param>
        /// <param name="pattern">Data to find</param>
        /// <param name="offset">Start of searching</param>
        /// <returns>Return -1 if not found otherwise index of start.</returns>
        public static int Search(byte[] src, byte[] pattern, int offset = 0)
        {
            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = offset; i < c; i++)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }
            return -1;
        }

        public static uint GetSize(byte[] data, int offset = 0)
        {
            var start = -1;

            if((start = Search(data, c.nullBytes, offset)) == -1)
            {
                throw new PacketException("Cannot find start of packet");
            }

            if (start + c.GetSizeSize > data.Length)
            {
                throw new PacketException("Wrong data! - missing full size");
            }

            var sizeBytes = new byte[c.GetSizeSize];

            Buffer.BlockCopy(data, start + c.GetNullSize, sizeBytes, 0, c.GetSizeSize);
            
            var encryptedBytesSize = BitConverter.ToUInt32(sizeBytes, 0);
            
            return encryptedBytesSize;
        }

        public static byte[] Pack(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length <= 0) throw new ArgumentException(nameof(data));

            byte[] ret = new byte[c.GetNullSize + c.GetSizeSize + data.Length];

            byte[] lenBytes = BitConverter.GetBytes(data.Length);

            Buffer.BlockCopy(c.nullBytes, 0, ret, 0, c.GetNullSize);
            Buffer.BlockCopy(lenBytes, 0, ret, c.GetNullSize, c.GetSizeSize);
            Buffer.BlockCopy(data, 0, ret, c.GetNullSize + c.GetSizeSize, data.Length);

            return ret;
        }

        public static byte[] Unpack(byte[] Hpckt, int offset)
        {
            if (Hpckt == null) throw new ArgumentNullException(nameof(Hpckt));
            if (Hpckt.Length - offset <= c.GetSizeSize + c.GetNullSize) throw new ArgumentException("Small size of packet");
            if (offset < 0 || offset > Hpckt.Length) throw new ArgumentException("Bad offset!");
            
            uint size = 0;
            int start = Search(Hpckt, c.nullBytes, offset);

            if ((size = GetSize(Hpckt, start)) == 0)
            {
                throw new PacketException("Bad size of packet!");
            }

            if (size > (Hpckt.Length - c.GetSizeSize - c.GetNullSize - start))
            {
                throw new PacketException("Packet size is larger than data!");
            }

            byte[] outData = new byte[size];

            Buffer.BlockCopy(Hpckt, start + c.GetNullSize + c.GetSizeSize, outData, 0, (int) size);

            return outData;
        }
    }
}
