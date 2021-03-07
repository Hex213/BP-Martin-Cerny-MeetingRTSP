using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibHexCryptoStandard.Packet
{
    public class HexPacket
    {
        private byte[] data_bytes = null;
        private bool useBase64;
        private uint decryptedBytesSize = 0;
        private ushort sizeOfPcktData;

        private static byte[] nullBytesStatic = { 0,0,0,0 };

        public static byte[] GetNull() => nullBytesStatic;

        //Find firs packet and return it
        public static HexPacket GetPacketFromArr(in byte[] inputBytes, ushort offset = 0, bool useBase64 = false)
        {
            //najdi hex packet
            int start = -1;
            do
            {
                Span<byte> bytes = inputBytes;
                start = bytes.IndexOf(GetNull());
                //TODO: Mozno nepotrebne
                for (var i = 0; i < GetNull().Length; i++)
                {
                    if (inputBytes[start + i] == 0) continue;
                    offset += (ushort) start;
                    start = -1;
                }
            } while (start == -1 && offset < inputBytes.Length);
            //ak nie je ukonci
            if (start == -1) throw new PacketException("Packet not found");
            //ziskaj velkost zasifrovanych dat
            uint sizeOfData = HexPacket.GetSizeFrom(inputBytes, (ushort)start);
            //ak nie je velkost/nastala chyba ukonci!
            if (sizeOfData == 0) throw new PacketException("Packet size is 0");
            //vytvor nove pole pre paket
            byte[] pktBytes = new byte[sizeOfData + HexPacket.GetNull().Length + 4];
            //copy data do neho
            Buffer.BlockCopy(inputBytes, start, pktBytes, 0, (int)(sizeOfData + HexPacket.GetNull().Length + 4));

            return new HexPacket(pktBytes, useBase64);
        }

        public static uint GetSizeFrom(in byte[] bytesToFind, in ushort offset = 0, in ushort default_size_bytes = 4)
        {
            if(bytesToFind.Length <= 8)
            {
                return 0;
            }  
            byte[] nullBytes = new byte[4];
            byte[] sizeBytes = new byte[default_size_bytes];
            try
            {
                Buffer.BlockCopy(bytesToFind, offset, nullBytes, 0, 4);
                Buffer.BlockCopy(bytesToFind, offset + default_size_bytes, sizeBytes, 0, default_size_bytes);
            }
            catch (Exception e)
            {
                return 0;
            }
            
            //check null integrity
            if (nullBytes.Any(b => b != 0))
            {
                return 0;
            }
            //check size integrity
            try
            {
                var encryptedBytesSize = BitConverter.ToUInt32(sizeBytes, 0);
                return encryptedBytesSize;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public HexPacket(byte[] dataBytes, bool useBase64, ushort sizeOfPcktData = 4)
        {
            if (dataBytes.Length <= 8)
            {
                throw new PacketException("Bad array struct!");
            }
            this.data_bytes = dataBytes;
            this.sizeOfPcktData = sizeOfPcktData;
            this.useBase64 = useBase64;
        }

        public byte[] DataBytes => data_bytes;
        public uint DecryptedBytesSize => decryptedBytesSize;

        private Object processBytes(ushort offset = 0)
        {
            byte[] nullBytes = new byte[4];
            byte[] sizeBytes = new byte[sizeOfPcktData];
            byte[] dataBytes = new byte[data_bytes.Length - 4 - sizeOfPcktData];
            string base64 = null;

            try {
                Buffer.BlockCopy(data_bytes, offset, nullBytes, 0, 4);
                Buffer.BlockCopy(data_bytes, offset + 4, sizeBytes, 0, sizeOfPcktData);
                Buffer.BlockCopy(data_bytes, offset + 4 + sizeOfPcktData, dataBytes, 0, data_bytes.Length - 4 - sizeOfPcktData);
                if (useBase64)
                {
                    base64 = Encoding.UTF8.GetString(dataBytes);
                }
            }
            catch(Exception e)
            {
                throw new PacketException("Error with packet data!", e);
            }

            //check integrity null
            if (nullBytes.Any(b => b != 0))
            {
                throw new PacketException("Bad format(null start)");
            }

            //check size integrity
            try
            {
                var encryptedBytesSize = BitConverter.ToUInt32(sizeBytes, 0);
                if (encryptedBytesSize != data_bytes.Length - 4 - sizeOfPcktData)
                {
                    throw new Exception("Data length is not correct!");
                }
            }
            catch (Exception e)
            {
                throw new PacketException("Size format error!", e);
            }

            Object decrypted;
            if (data_bytes != null) //decrypt by bytes
            {
                try
                {
                    decrypted = useBase64 ? Algoritm.AesGcm256.decrypt(base64) : Algoritm.AesGcm256.decrypt(null, dataBytes);
                    decryptedBytesSize = (uint) (useBase64 ? ((string) decrypted).Length : ((byte[]) decrypted).Length);
                }
                catch (Exception e)
                {
                    throw new PacketException("Error with decryption!", e);
                }}
            else
            {
                throw new PacketException("Error with decrypt data!");
            }

            return decrypted;
        }

        public Object Decrypt(ushort offset = 0)
        {
            if (data_bytes == null) throw new PacketException("Missing input data");
            return processBytes(offset);
        }
    }
}
