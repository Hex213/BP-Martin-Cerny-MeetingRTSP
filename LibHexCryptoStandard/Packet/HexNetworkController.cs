using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LibHexCryptoStandard.Packet
{
    public static class HexNetworkController
    {
        public static async Task<int> Read(Stream stream, byte[] readBuffer, int offset, bool encryption, bool useBase64, bool output = false)
        {
            //0. read data
            var read = await stream.ReadAsync(readBuffer, offset, readBuffer.Length);

            if(output) Console.WriteLine("Recv(" + read + ")");

            HexPacket hexPacket = null;

            if (!encryption)
            {
                return read;
            }

            //1. get packet
            hexPacket = HexPacket.GetPacketFromArr(readBuffer, (ushort)offset);
            //2. decrypt
            Object decrypted = hexPacket.Decrypt();
            byte[] toCopy;
            if (useBase64)
            {
                toCopy = Encoding.UTF8.GetBytes((string)decrypted);
            }
            else
            {
                toCopy = (byte[])decrypted;
            }
            //3. copy back to buffer
            Buffer.BlockCopy(toCopy, 0, readBuffer, offset, (int)hexPacket.DecryptedBytesSize);
            //4. return of readed
            return (int)hexPacket.DecryptedBytesSize;
        }
    }
}
