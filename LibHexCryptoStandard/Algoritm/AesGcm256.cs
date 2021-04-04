using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using LibHexCryptoStandard.Hashs;
using LibHexCryptoStandard.Packet;
using LibHexUtils.Arrays;
using LibNet.Meeting.Packets.Exceptions;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace LibHexCryptoStandard.Algoritm
{
    public class AesGcm256
    {
        

        private static readonly SecureRandom Random = new SecureRandom();
        private static byte[] key = null;

        // Pre-configured Encryption AdditionalFunctions
        public static readonly int NonceBitSize = 128;
        public static readonly int MacBitSize = 128;
        public static readonly int KeyBitSize = 256;

        private AesGcm256()
        {
        }

        /// <summary>
        /// Generate new random IV with defined size
        /// </summary>
        /// <returns>Byte array with IV</returns>
        private static byte[] NewIv()
        {
            var iv = new byte[NonceBitSize / 8];
            Random.NextBytes(iv);
            return iv;
        }

        public static byte[] HexToByte(string hexStr)
        {
            var bArray = new byte[hexStr.Length / 2];
            for (int i = 0; i < (hexStr.Length / 2); i++)
            {
                var firstNibble = Byte.Parse(hexStr.Substring((2 * i), 1),
                    System.Globalization.NumberStyles.HexNumber); // [x,y)
                var secondNibble = Byte.Parse(hexStr.Substring((2 * i) + 1, 1),
                    System.Globalization.NumberStyles.HexNumber);
                var finalByte = (secondNibble) | (firstNibble << 4); // bit-operations 
                // only with numbers, not bytes.
                bArray[i] = (byte)finalByte;
            }

            return bArray;
        }

        /// <summary>
        /// Encrypt block of data with key and random iv. This function must be initialized before use!
        /// </summary>
        /// <param name="block">Data to encrypt</param>
        /// <returns>Encrypted byte array. </returns>
        /// <exception cref="TypeInitializationException">Throws when is not initialized key.</exception>
        /// <exception cref="PacketException">Throws when is missing input data.</exception>
        public static byte[] Encrypt(byte[] block, byte[] key)
        {
            if (key == null || key.Length != 32)
            {
                throw new ArgumentException("Wrong key!");
            }

            var sR = string.Empty;
            try
            {
                var plainBytes = block;
                if (block == null || block.Length == 0)
                {
                    throw new PacketException("Input data missing!");
                }

                GcmBlockCipher cipher = new GcmBlockCipher(new AesFastEngine());
                var iv = NewIv();
                AeadParameters parameters =
                    new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(true, parameters);

                byte[] encryptedBytes = new byte[iv.Length + cipher.GetOutputSize(plainBytes.Length)];

                //ByteArray.Print(iv, "IV");
                //ByteArray.Print(key, "KEY");

                //copy iv
                Buffer.BlockCopy(iv, 0, encryptedBytes, 0, iv.Length);
                //encrypt
                Int32 retLen = cipher.ProcessBytes
                    (plainBytes, 0, plainBytes.Length, encryptedBytes, iv.Length);
                cipher.DoFinal(encryptedBytes, iv.Length + retLen);

                return encryptedBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public static Object Decrypt(byte[] key, string EncryptedText, byte[] block = null)
        {
            if (key == null || key.Length != 32)
            {
                throw new ArgumentException("Wrong key!");
            }

            var sR = string.Empty;

            try
            {
                var encryptedBytes = block ?? Convert.FromBase64String(EncryptedText);

                byte[] iv = new byte[NonceBitSize / 8];
                byte[] data = new byte[encryptedBytes.Length - NonceBitSize / 8];
                Buffer.BlockCopy(encryptedBytes, 0, iv, 0, NonceBitSize / 8);
                Buffer.BlockCopy(encryptedBytes, NonceBitSize / 8, data, 0, encryptedBytes.Length - (NonceBitSize / 8));

                //ByteArray.Print(iv, "IV");
                //ByteArray.Print(key, "KEY");

                GcmBlockCipher cipher = new GcmBlockCipher(new AesFastEngine());
                AeadParameters parameters =
                    new AeadParameters(new KeyParameter(key), 128, iv, null);
                //ParametersWithIV parameters = new ParametersWithIV(new KeyParameter(key), iv);

                cipher.Init(false, parameters);
                byte[] plainBytes = new byte[cipher.GetOutputSize(data.Length)];
                Int32 retLen = cipher.ProcessBytes
                    (data, 0, data.Length, plainBytes, 0);
                //ByteArray.Print(data, "DATA");
                cipher.DoFinal(plainBytes, retLen);

                return plainBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }
    }
}
