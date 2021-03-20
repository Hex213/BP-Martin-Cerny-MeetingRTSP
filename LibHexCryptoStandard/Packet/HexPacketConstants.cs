using System;
using System.Collections.Generic;
using System.Text;

namespace LibHexCryptoStandard.Packet
{
    public class HexPacketConstants
    {
        public static readonly byte[] nullBytes = { 0, 0, 0, 0 };
        public static readonly byte sizeLen = 4;

        public static int GetNullSize => nullBytes.Length;
        public static int GetSizeSize => sizeLen;
    }
}
