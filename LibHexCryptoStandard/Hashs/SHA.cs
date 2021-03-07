using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace LibHexCryptoStandard.Hashs
{
    public static class SHA
    {
        public static byte[] ReverBitConverter(string input)
        {
            var hexa = input.Split("-");
            List<byte> arr = new List<byte>(hexa.Length);

            arr.AddRange(hexa.Select(pair => pair.Aggregate<char, byte>(0, (current, c) => (byte)(current + (byte)c))));
            return arr.ToArray();
        }

        public static string SHA3(string rawData, ushort bitLength)
        {
            switch (bitLength)
            {
                case 224:
                case 256:
                case 384:
                case 512:
                    break;
                default:
                    throw new ArgumentException("Invalid bitlength");
            }

            var hashAlgorithm = new Sha3Digest(bitLength);

            // Choose correct encoding based on your usecase
            byte[] input = Encoding.ASCII.GetBytes(rawData);

            hashAlgorithm.BlockUpdate(input, 0, input.Length);

            byte[] result = new byte[bitLength / 8];
            hashAlgorithm.DoFinal(result, 0);

            var hashString = BitConverter.ToString(result);
            return hashString;
        }
    }
}
