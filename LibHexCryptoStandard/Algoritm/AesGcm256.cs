using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using LibHexCryptoStandard.Packet;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace LibHexCryptoStandard.Algoritm
{
    public class AesGcm256
    {
        public static byte[] Key => key;
        public static byte[] Iv => iv;

        private static readonly SecureRandom Random = new SecureRandom();
        private static byte[] key = null;
        private static byte[] iv = null;

        // Pre-configured Encryption Parameters
        public static readonly int NonceBitSize = 128;
        public static readonly int MacBitSize = 128;
        public static readonly int KeyBitSize = 256;

        private AesGcm256()
        {
        }

        /// <summary>
        /// Initialization function for encrypt/decrypt
        /// </summary>
        /// <param name="key">key (hexa format)</param>
        /// <param name="iv">iv (hexa format)</param>
        public static void init(string key, string iv)
        {
            if (key.Length != 64 || iv.Length != 32)
            {
                throw new TypeInitializationException("key or iv has not correct len(hex format)", null);
            }
            AesGcm256.key = AesGcm256.HexToByte(key);
            AesGcm256.iv = AesGcm256.HexToByte(iv);
        }

        public static byte[] NewKey()
        {
            var key = new byte[KeyBitSize / 8];
            Random.NextBytes(key);
            return key;
        }

        public static byte[] NewIv()
        {
            var iv = new byte[NonceBitSize / 8];
            Random.NextBytes(iv);
            return iv;
        }

        public static Byte[] HexToByte(string hexStr)
        {
            byte[] bArray = new byte[hexStr.Length / 2];
            for (int i = 0; i < (hexStr.Length / 2); i++)
            {
                byte firstNibble = Byte.Parse(hexStr.Substring((2 * i), 1),
                    System.Globalization.NumberStyles.HexNumber); // [x,y)
                byte secondNibble = Byte.Parse(hexStr.Substring((2 * i) + 1, 1),
                    System.Globalization.NumberStyles.HexNumber);
                int finalByte = (secondNibble) | (firstNibble << 4); // bit-operations 
                // only with numbers, not bytes.
                bArray[i] = (byte)finalByte;
            }

            return bArray;
        }

        public static string toHex(byte[] data)
        {
            string hex = string.Empty;
            foreach (byte c in data)
            {
                hex += c.ToString("X2");
            }

            return hex;
        }

        public static string toHex(string asciiString)
        {
            string hex = string.Empty;
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += string.Format("{0:x2}", System.Convert.ToUInt32(tmp.ToString()));
            }

            return hex;
        }

        public static byte[] encrypt(byte[] block)
        {
            if (key == null || iv == null)
            {
                throw new TypeInitializationException("Not initialized!", null);
            }
            string sR = string.Empty;
            try
            {
                byte[] plainBytes = block;
                if (block == null || block.Length == 0)
                {
                    throw new PacketException("Input dta missing!");
                }

                GcmBlockCipher cipher = new GcmBlockCipher(new AesFastEngine());
                AeadParameters parameters =
                    new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(true, parameters);

                byte[] encryptedBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
                Int32 retLen = cipher.ProcessBytes
                    (plainBytes, 0, plainBytes.Length, encryptedBytes, 0);
                cipher.DoFinal(encryptedBytes, retLen);

                return encryptedBytes;
                //sR = Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// Decrypt data with key and IV, which was set by init method<see cref=""/>. If block has non-value, then use Base64 format 
        /// </summary>
        /// <param name="EncryptedText">string to decrypt (for Base64 format)</param>
        /// <param name="block">bytes to decrypt (for Byte format)</param>
        /// <returns></returns>
        public static Object decrypt(string EncryptedText/*, byte[] key, byte[] iv*/, byte[] block = null)
        {
            if (key == null || iv == null)
            {
                throw new TypeInitializationException("Not initialized!", null);
            }
            string sR = string.Empty;
            try
            {
                byte[] encryptedBytes;
                encryptedBytes = block ?? Convert.FromBase64String(EncryptedText);

                GcmBlockCipher cipher = new GcmBlockCipher(new AesFastEngine());
                AeadParameters parameters =
                    new AeadParameters(new KeyParameter(key), 128, iv, null);
                //ParametersWithIV parameters = new ParametersWithIV(new KeyParameter(key), iv);

                cipher.Init(false, parameters);
                byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                Int32 retLen = cipher.ProcessBytes
                    (encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
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
