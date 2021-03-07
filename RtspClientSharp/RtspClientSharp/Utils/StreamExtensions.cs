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
                int read = await stream.ReadAsync(buffer, offset, count);

                if (read == 0)
                    throw new EndOfStreamException();

                //TODO: presun na HexNetworkController

                if (decrypt)
                {
                    int offOfHPkt = -1;
                    int offsetToPass = 0;
                    do
                    {
                        //najdi hex packet
                        offOfHPkt = ArrayUtils.IndexOfBytes(buffer, HexPacket.GetNull(), offset + offsetToPass,
                            HexPacket.GetNull().Length);
                        //ak nie je ukonci
                        if (offOfHPkt == -1) break;
                        //ziskaj velkost zasifrovanych dat
                        uint sizeOfData = HexPacket.GetSizeFrom(buffer, (ushort) offOfHPkt);
                        //ak nie je velkost/nastala chyba ukonci!
                        if (sizeOfData == 0) break;
                        //vytvor nove pole pre paket
                        byte[] pktBytes = new byte[sizeOfData + HexPacket.GetNull().Length + 4];
                        //copy data do neho
                        Buffer.BlockCopy(buffer, offOfHPkt, pktBytes, 0, (int) (sizeOfData + 4 + 4));

                        HexPacket hexPacket = new HexPacket(pktBytes, useBase64);
                        Object decryptedObj = hexPacket.Decrypt();
                        if (decryptedObj == null) break;
                        byte[] toCopy;
                        if (useBase64)
                        {
                            string base64 = (string)decryptedObj;
                            toCopy = Encoding.UTF8.GetBytes(base64);
                        }
                        else
                        {
                            toCopy = (byte[])decryptedObj;
                        }

                        Buffer.BlockCopy(toCopy, 0, buffer, offOfHPkt, toCopy.Length);
                    } while (true);
                }
                else
                {
                    count -= read;
                    offset += read;
                }
            } while (count != 0);
        }
    }
}