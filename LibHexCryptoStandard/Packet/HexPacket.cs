using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibHexCryptoStandard.Algoritm;

namespace LibHexCryptoStandard.Packet
{
    public enum EncryptType
    {
        Encrypt,
        Decrypt
    }

    public class HexPacket
    {
        private byte[] data_bytes = null;
        private bool useBase64;
        private EncryptType type;
        private uint decryptedBytesSize = 0;
        private ushort sizeOfPcktData;

        private static byte[] nullBytesStatic = { 0,0,0,0 };

        public static byte[] GetNull() => nullBytesStatic;
        public EncryptType Type => type;
        public byte[] DataBytes => data_bytes;
        public uint DecryptedBytesSize => decryptedBytesSize;
        public bool UseBase64 => useBase64;
        public ushort SizeOfPcktData => sizeOfPcktData;

        public HexPacket(byte[] dataBytes, bool useBase64, EncryptType et, ushort sizeOfPcktData = 4)
        {
            if (dataBytes.Length <= 8 && et == EncryptType.Decrypt)
            {
                throw new PacketException("Bad array struct!");
            }
            this.data_bytes = dataBytes;
            this.sizeOfPcktData = sizeOfPcktData;
            this.useBase64 = useBase64;
            this.type = et;
        }



        #region Encrypt
        //Create hexPacket for encryption
        public static HexPacket CreatePacket(byte[] dataToEncrypt, bool usebase64)
        {
            return new HexPacket(dataToEncrypt, usebase64, EncryptType.Encrypt);
        }

        public Object Encrypt()
        {
            if (data_bytes == null || data_bytes.Length < 1 || type != EncryptType.Encrypt) throw new PacketException("Missing input data");
            
            byte[] block = data_bytes;
            var data = AesGcm256.encrypt(block);

            if (data == null || data.Length != data_bytes.Length+16)
            {
                throw new PacketException("Error with encryption");
            }

            if (UseBase64)
            {
                string tmp = Convert.ToBase64String(data);
                data = tmp.Select(c => (byte)c).ToArray();
            }

            byte[] lenBytes = BitConverter.GetBytes(data.Length);

            //if (!BitConverter.IsLittleEndian)
            //{
            //    lenBytes = lenBytes.Reverse().ToArray();
            //}

            byte[] ret = new byte[GetNull().Length+SizeOfPcktData+data.Length];
            
            Buffer.BlockCopy(GetNull(), 0, ret, 0, GetNull().Length);
            Buffer.BlockCopy(lenBytes, 0, ret, GetNull().Length, SizeOfPcktData);
            Buffer.BlockCopy(data, 0,ret, GetNull().Length+ SizeOfPcktData, data.Length);

            return ret;
        }
        #endregion

        #region Decryption
        //Find firs packet and return it (for decryption)
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
                    offset += (ushort)start;
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
            //todo:miesto 4 dat dynamic
            byte[] pktBytes = new byte[sizeOfData + HexPacket.GetNull().Length + 4];
            //copy data do neho
            Buffer.BlockCopy(inputBytes, start, pktBytes, 0, (int)(sizeOfData + HexPacket.GetNull().Length + 4));

            return new HexPacket(pktBytes, useBase64, EncryptType.Decrypt);
        }

        public static uint GetSizeFrom(in byte[] bytesToFind, in ushort offset = 0, in ushort default_size_bytes = 4)
        {
            //todo:check offset
            if (bytesToFind.Length <= 8)
            {
                return 0;
            }

            var tmp = new byte[bytesToFind.Length - offset];
            
            Buffer.BlockCopy(bytesToFind, offset, tmp, 0, tmp.Length);
            Span<byte> bytes = new Span<byte>(tmp);

            var start = bytes.IndexOf(GetNull());
            byte[] nullBytes = new byte[GetNull().Length];
            byte[] sizeBytes = new byte[default_size_bytes];
            try
            {
                Buffer.BlockCopy(tmp, start, nullBytes, 0, GetNull().Length);
                Buffer.BlockCopy(tmp, start + GetNull().Length, sizeBytes, 0, default_size_bytes);
            }
            catch (Exception e)
            {
                return 0;
            }

            //check null integrity
            //todo:asi nepotrebne
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

        private Object processBytes(ushort offset = 0)
        {
            byte[] nullBytes = new byte[GetNull().Length];
            byte[] sizeBytes = new byte[sizeOfPcktData];
            byte[] dataBytes = new byte[data_bytes.Length - GetNull().Length - sizeOfPcktData];
            string base64 = null;

            try
            {
                Buffer.BlockCopy(data_bytes, offset, nullBytes, 0, GetNull().Length);
                Buffer.BlockCopy(data_bytes, offset + GetNull().Length, sizeBytes, 0, sizeOfPcktData);
                Buffer.BlockCopy(data_bytes, offset + GetNull().Length + sizeOfPcktData, dataBytes, 0, data_bytes.Length - GetNull().Length - sizeOfPcktData);
                if (useBase64)
                {
                    base64 = Encoding.UTF8.GetString(dataBytes);
                }
            }
            catch (Exception e)
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
                if (encryptedBytesSize != data_bytes.Length - GetNull().Length - sizeOfPcktData)
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
                    decryptedBytesSize = (uint)((byte[])decrypted).Length;
                }
                catch (Exception e)
                {
                    throw new PacketException("Error with decryption!", e);
                }
            }
            else
            {
                throw new PacketException("Error with decrypt data!");
            }

            return decrypted;
        }

        public Object Decrypt(ushort offset = 0)
        {
            if (data_bytes == null || data_bytes.Length < 1 || type != EncryptType.Decrypt) throw new PacketException("Missing input data");
            return processBytes(offset);
        }
        #endregion
    }
}
