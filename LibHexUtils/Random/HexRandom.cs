using System;
using System.Security.Cryptography;
using LibHexUtils.Arrays;

namespace LibHexUtils.Random
{
    public static class HexRandom
    {
        public static byte[] GetRandomBytes(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            var rand = new byte[size];
            var s = new RNGCryptoServiceProvider();
            s.GetBytes(rand);
            return rand;
        }

        public static int GetRandomInt(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
                throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private static uint GetRandomUInt(uint minValue = 0, uint maxExclusiveValue = 0)
        {
            var randomBytes = GetRandomBytes(sizeof(uint));
            var ret = BitConverter.ToUInt32(randomBytes, 0);

            uint down = minValue, up = maxExclusiveValue;
            if (minValue > maxExclusiveValue)
            {
                down = maxExclusiveValue;
                up = minValue;
            }
            if (up > 0)
            {
                ret = (ret % (up - down)) + down;
            }

            return ret;
        }

        public static byte[] GetRandomBytesWithMessage(int size, byte[] secretBytes)
        {
            if (secretBytes == null) throw new ArgumentNullException(nameof(secretBytes));
            if (size <= 0 || size <= secretBytes.Length) throw new ArgumentOutOfRangeException(nameof(size));

            int search = -1;
            uint pos = UInt32.MaxValue;
            byte[] rand = null;

            do
            {
                rand = GetRandomBytes(size);
                pos = GetRandomUInt(0, (uint) (size - secretBytes.Length));
                Buffer.BlockCopy(secretBytes, 0, rand, (int) pos, secretBytes.Length);

                search = ByteArray.Search(rand, secretBytes);
            } while (search < 0 || search != pos);
            

            return rand;
        }
    }
}
