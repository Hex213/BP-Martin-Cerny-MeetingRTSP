using System;
using System.Linq;

namespace LibHexUtils.Arrays
{
    public class ByteArray
    {
        /// <summary>
        /// Find sequence in byte array.
        /// </summary>
        /// <param name="src">Input data</param>
        /// <param name="pattern">Data to find</param>
        /// <param name="offset">Start of searching</param>
        /// <returns>Return -1 if not found otherwise index of start.</returns>
        public static int Search(byte[] src, byte[] pattern, int offset = 0)
        {
            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = offset; i < c; i++)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }
            return -1;
        }

        public static void Print(byte[] bytes, string title, int offset = 0, int count = 0)
        {
            if (bytes == null) return;
            Console.WriteLine(title + ":");

            if (count <= 0)
            {
                count = bytes.Length - offset;
            }
            int s = 0;

            foreach (var b in bytes)
            {
                if (s >= offset && count > 0)
                {
                    Console.Write(b + "-");
                    count--;
                }
                s++;
            }
            Console.Write("\b \n");
        }

        public static byte[] CopyBytes(int offset, params byte[][] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(args));

            var size = args.Sum(t => ((byte[]) t).Length);
            if (size <= 0)
            {
                throw new ArgumentException("Count of params arrays <= 0");
            }

            var outBytes = new byte[offset + size];
            var index = offset;
            foreach (var o in args)
            {
                if (((byte[]) o).Length <= 0) continue;
                Buffer.BlockCopy((byte[])o, 0, outBytes, index, ((byte[]) o).Length);
                index += ((byte[]) o).Length;
            }

            return outBytes;
        }

        public static byte[] SubArray(byte[] src, int startOffset, int count = 0)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (startOffset < 0) throw new ArgumentOutOfRangeException(nameof(startOffset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (startOffset + count > src.Length) throw new IndexOutOfRangeException("Count or startOffset > src len");

            if (count == 0)
            {
                count = src.Length - startOffset;
            }

            var dst = new byte[count];
            Buffer.BlockCopy(src, startOffset, dst, 0, count);
            return dst;
        }
    }
}
