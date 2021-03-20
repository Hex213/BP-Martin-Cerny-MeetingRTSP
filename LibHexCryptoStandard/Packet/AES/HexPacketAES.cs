using System;
using System.Linq;
using System.Text;
using LibHexCryptoStandard.Algoritm;

namespace LibHexCryptoStandard.Packet.AES
{
    public class HexPacketAES
    {
        private byte[] data_bytes = null;
        private readonly bool useBase64;
        private readonly EncryptType type;
        private uint decryptedBytesSize = 0;
        
        public EncryptType Type => type;
        public byte[] DataBytes => data_bytes;
        public uint DecryptedBytesSize => decryptedBytesSize;
        public bool UseBase64 => useBase64;

        public HexPacketAES(byte[] dataBytes, bool useBase64, EncryptType et)
        {
            if (dataBytes.Length <= 8 && et == EncryptType.Decrypt)
            {
                throw new PacketException("Bad array struct!");
            }
            this.data_bytes = dataBytes;
            this.useBase64 = useBase64;
            this.type = et;
        }

        public static HexPacketAES GetDataFromPacket(in byte[] hPacket, ushort offset = 0, bool useBase64 = false)
        {
            byte[] pktBytes = HexPacket.Unpack(hPacket, offset);

            return new HexPacketAES(pktBytes, useBase64, EncryptType.Decrypt);
        }

        #region Encrypt
        //Create hexPacket for encryption
        public static HexPacketAES CreatePacketToEncrypt(byte[] dataToEncrypt, bool usebase64)
        {
            return new HexPacketAES(dataToEncrypt, usebase64, EncryptType.Encrypt);
        }

        public Object Encrypt()
        {
            if (data_bytes == null || data_bytes.Length < 1 || type != EncryptType.Encrypt) throw new PacketException("Missing input data");
            
            byte[] block = data_bytes;
            var data = AesGcm256.Encrypt(block);

            if (data == null || data.Length != data_bytes.Length+16+16)
            {
                throw new PacketException("Error with encryption");
            }

            if (UseBase64)
            {
                string tmp = Convert.ToBase64String(data);
                data = tmp.Select(c => (byte)c).ToArray();
            }

            return HexPacket.Pack(data);
        }
        #endregion

        #region Decryption
        private Object ProcessBytes(ushort offset = 0)
        {
            byte[] dataBytes = null;
            string base64 = null;

            if (type == EncryptType.Decrypt)
            {
                dataBytes = data_bytes;
            }
            else
            {
                if (type == EncryptType.DecryptPacket)
                {
                    dataBytes = HexPacket.Unpack(data_bytes, offset);
                }
                else
                {
                    throw new PacketException("Unsupported type!");
                }
            }

            if (useBase64)
            {
                base64 = Encoding.UTF8.GetString(dataBytes);
            }

            Object decrypted;

            if (dataBytes != null) //decrypt by bytes
            {
                try
                {
                    decrypted = useBase64 ? AesGcm256.Decrypt(base64) : AesGcm256.Decrypt(null, dataBytes);
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
            if (data_bytes == null || data_bytes.Length < 1 ||
                (type != EncryptType.Decrypt && type != EncryptType.DecryptPacket)) throw new PacketException("Missing input data");
            return ProcessBytes(offset);
        }
        #endregion
    }
}
