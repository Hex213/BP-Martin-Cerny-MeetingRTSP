using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MeetingPlayerRepair.Properties;

namespace MeetingPlayerRepair
{
    class Program
    {
        private static void printHLine()
        {
            for (short i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine();
        }

        private static void printCenter(string what)
        {
            Console.SetCursorPosition((Console.WindowWidth - what.Length) / 2, Console.CursorTop);
            Console.WriteLine(what);
        }

        public static class HSHA1
        {
            public static string SHA1(string path)
            {
                StringBuilder formatted;

                using (FileStream fs = new FileStream(path, FileMode.Open))
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        byte[] hash = sha1.ComputeHash(bs);
                        formatted = new StringBuilder(2 * hash.Length);
                        foreach (byte b in hash)
                        {
                            formatted.AppendFormat("{0:X2}", b);
                        }
                    }
                }

                return formatted.ToString();
            }
        }

        private static bool _tryRunHelper(string name, string sha)
        {
            return HSHA1.SHA1(Directory.GetCurrentDirectory() + @"\x86\" + name).ToLower() == sha;
        }

        private static void _testCopy(string name, string sha, bool force)
        {
            if (File.Exists(Environment.SystemDirectory + @"\" + name) && !force) return;
            if (!_tryRunHelper(name, sha)) return;
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + @"\x86\" + name, Environment.SystemDirectory + @"\" + name, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void test(in string nameOfDll, in string SHA1, in bool force)
        {
            Console.Write(@"Testing: {0} -> ", nameOfDll);
            try
            {
                _testCopy(nameOfDll, SHA1, force);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"Passed");
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@"Error");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void tryRunX86(bool force)
        {
            test("avcodec-58.dll", "61f42fd3a671d73c0025df2132a6c6f176c6a3b3", force);
            test("avdevice-58.dll", "af6e120bb55a19a7086ad72279633fc2f318567a", force);
            test("avfilter-7.dll", "1354ff296e8fe10f3540710170233448d014346e", force);
            test("avformat-58.dll", "09eb16ac1d75de55025997724d942a9dd4215aae", force);
            test("avutil-56.dll", "908784407c99fb78ae21a2fe9118e5ff95ca940a", force);
            test("libffmpeghelper.dll", "5e3375fc6911b08605ba1c565b8b67bb117156ee", force);
            test("postproc-55.dll", "b30116a60dcb86e4f2e2735fb853a1554cc24d56", force);
            test("swresample-3.dll", "0e393f44b711fdb49a39b758bd5cdcaea3f4cdb5", force);
            test("swscale-5.dll", "5243d99be0458164fc1b46cca05b364b2d714dc0", force);
        }

        static void Main(string[] args)
        {
            bool ovrride = false;
            Console.ForegroundColor = ConsoleColor.White;

            if (args.Length > 0)
            {
                ovrride = args[0].ToLower().Equals("-o") | args[0].ToLower().Equals("-override");
            }

            printHLine();
            printCenter("Meeting repair tool by Hex - v" + Resources.version);
            printHLine();

            Console.WriteLine();

            if (ovrride)
            {
                Console.WriteLine(@"Repair with override flag");
            }

            tryRunX86(ovrride);
        }
    }
}
