using System;
using System.Linq;
using System.Text;
using LibHexCryptoStandard.Algoritm;
using LibNet.Meeting.Packets.Exceptions;
using LibNet.Meeting.Packets.HexPacket;

namespace LibHexCryptoStandard.Packet.AES
{
    // ReSharper disable once InconsistentNaming
    public class HexPacketAES
    {
        private byte[] _dataBytes = null;
        private byte[] _key = null;
        private readonly bool _useBase64;
        private readonly EncryptType _type;
        private uint _decryptedBytesSize = 0;
        
        public EncryptType Type => _type;
        public byte[] DataBytes => _dataBytes;
        public uint DecryptedBytesSize => _decryptedBytesSize;
        public bool UseBase64 => _useBase64;

        public HexPacketAES(byte[] dataBytes, bool useBase64, EncryptType et, byte[] key)
        {
            if (dataBytes == null) throw new ArgumentNullException(nameof(dataBytes));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key.Length != 32)
            {
                throw new PacketException("Bad key length!");
            }
            if ((dataBytes.Length <= 8 && et == EncryptType.Decrypt_hpkt) || dataBytes.Length <= 0)
            {
                throw new PacketException("Bad data!");
            }

            this._dataBytes = dataBytes;
            this._useBase64 = useBase64;
            this._type = et;
            this._key = key;
        }

        public static HexPacketAES CreatePacketForDecrypt(in byte[] dataToDecrypt, bool isInHPkt, byte[] key, ushort offset = 0, bool useBase64 = false)
        {
            return new HexPacketAES(dataToDecrypt, useBase64, isInHPkt ? EncryptType.Decrypt_hpkt : EncryptType.Decrypt, key);
        }

        #region Encrypt
        //Create hexPacket for encryption
        public static HexPacketAES CreatePacketForEncrypt(byte[] dataToEncrypt, bool toHPkt, byte[] key, bool usebase64 = false)
        {
            return new HexPacketAES(dataToEncrypt, usebase64, toHPkt ? EncryptType.Encrypt_hpkt : EncryptType.Encrypt, key);
        }

        public Object Encrypt()
        {
            if (_dataBytes == null || _dataBytes.Length <= 0 || _type != EncryptType.Encrypt && _type != EncryptType.Encrypt_hpkt) throw new PacketException("Missing input data");
            
            byte[] block = _dataBytes;
            var data = AesGcm256.Encrypt(block, _key);

            if (data == null || data.Length != _dataBytes.Length+16+16)
            {
                throw new PacketException("Error with encryption");
            }

            if (UseBase64)
            {
                string tmp = Convert.ToBase64String(data);
                data = tmp.Select(c => (byte)c).ToArray();
            }

            return _type == EncryptType.Encrypt_hpkt ? HexPacket.Pack(data) : data;
        }
        #endregion

        #region Decryption
        private Object ProcessDecrypt(ushort offset = 0)
        {
            byte[] dataBytes = null;
            string base64 = null;

            if (_type == EncryptType.Decrypt)
            {
                dataBytes = _dataBytes;
            }
            else
            {
                if (_type == EncryptType.Decrypt_hpkt)
                {
                    dataBytes = HexPacket.Unpack(_dataBytes, offset);
                }
                else
                {
                    throw new NotImplementedException("Unsupported type!");
                }
            }

            if (_useBase64)
            {
                base64 = Encoding.UTF8.GetString(dataBytes);
            }

            Object decrypted;

            if (dataBytes != null) //decrypt by bytes
            {
                try
                {
                    decrypted = _useBase64 ? AesGcm256.Decrypt(_key, base64) : AesGcm256.Decrypt(_key, null, dataBytes);
                    _decryptedBytesSize = (uint)((byte[])decrypted).Length;
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
            if (_dataBytes == null || _dataBytes.Length < 1 ||
                (_type != EncryptType.Decrypt && _type != EncryptType.Decrypt_hpkt)) throw new PacketException("Missing input data");
            return ProcessDecrypt(offset);
        }
        #endregion
    }
}
