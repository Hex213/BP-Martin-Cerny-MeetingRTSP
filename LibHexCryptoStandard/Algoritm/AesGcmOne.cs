using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using LibHexCryptoStandard.Hashs;

namespace LibHexCryptoStandard.Algoritm
{
    public static class AesGcmOne
    {
        public static string Encrypt(string plain, string key, ushort bitLength, out Span<byte> nonceIV)
        {
            // Default parameter
            nonceIV = default;

            // Get bytes of plaintext string
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
            byte[] _key = SHA.ReverBitConverter(SHA.SHA3(key, bitLength));

            // Get parameter sizes
            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = plainBytes.Length;

            // We write everything into one big array for easier encoding
            int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            Span<byte> encryptedData = encryptedDataLength < 1024
                ? stackalloc byte[encryptedDataLength]
                : new byte[encryptedDataLength].AsSpan();

            // Copy parameters
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Generate secure nonce
            RandomNumberGenerator.Fill(nonce);
            nonce.CopyTo(nonceIV);

            // Encrypt
            using var aes = new AesGcm(_key);
            aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

            // Encode for transmission
            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string cipher, string key, ushort bitLength)
        {
            // Decode
            Span<byte> encryptedData = Convert.FromBase64String(cipher).AsSpan();
            byte[] _key = SHA.ReverBitConverter(SHA.SHA3(key, bitLength));

            // Extract parameter sizes
            int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            // Extract parameters
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Decrypt
            Span<byte> plainBytes = cipherSize < 1024
                ? stackalloc byte[cipherSize]
                : new byte[cipherSize];
            using var aes = new AesGcm(_key);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            // Convert plain bytes back into string
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
