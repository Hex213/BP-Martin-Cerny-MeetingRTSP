namespace LibNet.Meeting.Packets.HexPacket
{
    public class HexPacketConstants
    {
        public static readonly byte[] nullBytes = { 0, 0, 0, 0 };
        public static readonly byte sizeLen = 4;

        public static int GetNullSize => nullBytes.Length;
        public static int GetSizeSize => sizeLen;
    }
}
