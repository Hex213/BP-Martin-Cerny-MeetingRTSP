using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LibHexCryptoStandard.Packet;

namespace RtspClientSharp.Utils
{
    static class StreamExtensions
    {
        public static async Task ReadExactAsync(this Stream stream, byte[] buffer, int offset, int count, bool decrypt = true, bool useBase64 = false)
        {
            if (count == 0)
                return;

            do
            {
                int read = await HexNetworkController.Read(stream, buffer, offset, decrypt, useBase64, false, count);

                if (read == 0)
                    throw new EndOfStreamException();
                
                count -= read; 
                offset += read;
                
            } while (count != 0);
        }
    }
}