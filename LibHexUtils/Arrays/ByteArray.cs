using System;
using System.Collections.Generic;
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
        /// <param name="srcOff">Index to start position of searching</param>
        /// <returns>Return -1 if not found otherwise index of start</returns>
        public static int Search(byte[] src, byte[] pattern, int srcOff = 0)
        {
            if (pattern.Length > src.Length - srcOff)
            {
                return -1;
            }

            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = srcOff; i < c; i++)
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
                    Console.Write((int)b + "-");
                    count--;
                }
                s++;
            }
            Console.Write("\b \b\n");
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

        public static void AppendArray(ref byte[] des, in byte[] add)
        {
            if (add == null) throw new ArgumentNullException(nameof(add));
            if (add.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(add));
            
            des = des != null ? CopyBytes(0, des, add) : SubArray(add, 0);
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

        public static List<byte[]> SplitArray(byte[] src, byte[] pattern)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (src.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(src));
            if (pattern.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(pattern));
            if (src.Length < pattern.Length) throw new ArgumentOutOfRangeException(nameof(src), "Src is smaller than pattern.");

            var splited = new List<byte[]>();
            int startpos = 0, lastf = -1;
            do
            {
                if ((lastf = Search(src, pattern, startpos)) == -1)
                {
                    if (startpos != src.Length)
                    {
                        var lsub = SubArray(src, startpos);
                        splited.Add(lsub);
                    }
                    break;
                }
                if(lastf == startpos)
                {
                    startpos += pattern.Length;
                    continue;
                }
                var sub = SubArray(src, startpos, lastf - startpos);
                startpos = lastf;
                splited.Add(sub);

            } while (true);

            return splited;
        }
    }
}
