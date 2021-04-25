using System;
using System.Collections.Generic;
using System.Drawing;
using LibHexUtils.Arrays;
using LibNet.Meeting.Packets.Exceptions;
using c = LibNet.Meeting.Packets.HexPacket.HexPacketConstants;

namespace LibNet.Meeting.Packets.HexPacket
{
    public enum EncryptType
    {
        None,
        Encrypt,
        Encrypt_hpkt,
        Decrypt,
        Decrypt_hpkt
    }

    public class HexPacket
    {
        public static uint GetSize(byte[] data, int offset = 0)
        {
            var start = -1;

            if((start = ByteArray.Search(data, c.nullBytes, offset)) == -1)
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

        public static void CheckPacket(byte[] Hpckt, int offset, out uint size, out int start)
        {
            if (Hpckt == null) throw new ArgumentNullException(nameof(Hpckt));
            if (Hpckt.Length - offset <= c.GetSizeSize + c.GetNullSize) throw new ArgumentException("Small size of packet");
            if (offset < 0 || offset > Hpckt.Length) throw new ArgumentException("Bad offset!");

            size = 0;
            start = ByteArray.Search(Hpckt, c.nullBytes, offset);

            if ((size = GetSize(Hpckt, start)) == 0)
            {
                throw new PacketException("Bad size of packet!");
            }

            if (size > (Hpckt.Length - c.GetSizeSize - c.GetNullSize - start))
            {
                throw new PacketException("Packet size is larger than data!");
            }
        }

        public static byte[] Unpack(byte[] Hpckt, int offset)
        {
            CheckPacket(Hpckt, offset, out var size, out var start);

            byte[] outData = new byte[size];

            Buffer.BlockCopy(Hpckt, start + c.GetNullSize + c.GetSizeSize, outData, 0, (int) size);

            return outData;
        }
    }
}
