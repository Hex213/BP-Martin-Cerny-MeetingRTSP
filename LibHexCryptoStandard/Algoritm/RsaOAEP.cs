using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LibHexUtils.Arrays;
using LibNet.Meeting.Packets.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace LibHexCryptoStandard.Algoritm
{
    public static class RsaOAEP
    {
        public static AsymmetricCipherKeyPair GenerateKeyPair(int keyBitSize)
        {
            switch (keyBitSize)
            {
                case 1024:
                case 2048:
                case 3072:
                case 4096:
                    break;
                default:
                    throw new ArgumentException(keyBitSize + "!= {1024,2048,3072,4096}");
            }
            RsaKeyPairGenerator rsaKeyPairGnr = new RsaKeyPairGenerator();
            rsaKeyPairGnr.Init(new KeyGenerationParameters(new SecureRandom(), keyBitSize));
            return rsaKeyPairGnr.GenerateKeyPair();
        }
        
        // output
        // |1| *L|len-1-*L-32|  32  |
        // |L|exp|  modulus  |sha256|
        public static byte[] KeyToBytes(RsaKeyParameters key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            byte[] returnBytes = null;
            byte[] hashValue = null;

            using (var sha256 = SHA256.Create())
            {
                var exp = key.Exponent.ToByteArray();
                var mod = key.Modulus.ToByteArray();

                if (exp ==  null || mod == null || exp.Length <= 0 || mod.Length <= 0)
                {
                    throw new NullReferenceException();
                }

                byte[] toHash = ByteArray.CopyBytes(0, exp, mod);//new byte[exp.Length + mod.Length];
                //Buffer.BlockCopy(exp, 0, toHash, 0, exp.Length);
                //Buffer.BlockCopy(mod, 0, toHash, exp.Length, mod.Length);

                //ByteArray.Print(toHash, "To hash KTB");

                hashValue = sha256.ComputeHash(toHash);

                returnBytes = new byte[1 + exp.Length + mod.Length + hashValue.Length];

                returnBytes[0] = (byte)exp.Length;
                Buffer.BlockCopy(exp, 0, returnBytes, 1, exp.Length);
                Buffer.BlockCopy(mod, 0, returnBytes, 1 + exp.Length, mod.Length);
                Buffer.BlockCopy(hashValue, 0, returnBytes, 1 + exp.Length + mod.Length, hashValue.Length);
            }

            return returnBytes;
        }

        public static RsaKeyParameters BytesTokey(byte[] data, bool exceptPrivate)
        {
            var sha256 = SHA256.Create();

            byte lenExp = data[0];
            int lenMod = data.Length - 1 - lenExp - 32;

            var exp = new byte[lenExp];
            var mod = new byte[lenMod];
            var hash = new byte[32];

            Buffer.BlockCopy(data, 1, exp, 0, lenExp);
            Buffer.BlockCopy(data, 1 + lenExp, mod, 0, lenMod);
            Buffer.BlockCopy(data, 1 + lenExp + lenMod, hash, 0, 32);

            var hashValue = sha256.ComputeHash(data, 1, lenExp + lenMod);

            //ByteArray.Print(data, "Compute from BTK", 1, lenExp + lenMod);

            if (ByteArray.Search(hash, hashValue) == -1)
            {
                throw new PacketException("Hash not correct!");
            }

            var e = new BigInteger(1, exp);
            var m = new BigInteger(1, mod);

            return new RsaKeyParameters(exceptPrivate, m, e);
        }

        public static byte[] Encrypt(RsaKeyParameters encryptKey, byte[] data, int offset)
        {
            SecureRandom randomNumber = new SecureRandom();
            SHA256Managed hash = new SHA256Managed();

            byte[] encodingParam = hash.ComputeHash(Encoding.UTF8.GetBytes(randomNumber.ToString()));

            IAsymmetricBlockCipher cipher = new OaepEncoding(new RsaEngine(), new Sha256Digest(), encodingParam);
            cipher.Init(true, encryptKey);
            return cipher.ProcessBlock(data, offset, data.Length - offset);
        }

        public static byte[] Decrypt(RsaKeyParameters decryptKey, byte[] data, int offset)
        {
            SecureRandom randomNumber = new SecureRandom();
            SHA256Managed hash = new SHA256Managed();
            byte[] encodingParam = hash.ComputeHash(Encoding.UTF8.GetBytes(randomNumber.ToString()));

            IAsymmetricBlockCipher cipher = new OaepEncoding(new RsaEngine(), new Sha256Digest(), encodingParam);
            cipher.Init(false, decryptKey);
            return cipher.ProcessBlock(data, offset, data.Length);
        }
    }
}
