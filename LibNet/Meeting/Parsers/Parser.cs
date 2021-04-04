using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using System.Text;
using LibHexUtils.Arrays;
using LibHexUtils.Random;
using LibNet.Meeting.Packets.HexPacket;

namespace LibNet.Meeting.Parsers
{
    public class Parser
    {
        public static readonly int controlLen = 32;

        private static int Parse(byte[] hpkt, byte[]pattern, bool isPacket = true)
        {
            if (hpkt == null) throw new ArgumentNullException(nameof(hpkt));

            var inputUpkt = isPacket ? HexPacket.Unpack(hpkt, 0) : hpkt;

            var start = ByteArray.Search(inputUpkt, pattern);
            return start;
        }

        public static bool ParseCon(byte[] hpkt)
        {
            return Parse(hpkt, Encoding.UTF8.GetBytes("HMET HI")) != -1;
        }

        public static bool ParseID(byte[] hpkt, out byte[] id, string arg = "HMET CONN ", bool inPkt = true)
        {
            var plain = inPkt ? HexPacket.Unpack(hpkt, 0) : hpkt;
            id = null;
            var s = Parse(plain, Encoding.UTF8.GetBytes(arg), false);
            if (s == -1) return false;
            id = ByteArray.SubArray(plain, s + arg.Length, 32);
            return true;
        }

        public static bool ParseConConf(byte[] hpkt)
        {
            return Parse(hpkt, Encoding.UTF8.GetBytes("HMET OK")) != -1;
        }

        private static int ParseOptConn(byte[] data, out int startSTS)
        {
            startSTS = ByteArray.Search(data, Encoding.UTF8.GetBytes("STS"));
            return startSTS != -1 ? data[startSTS + "STS".Length + 4 + 4] : -1;
        }

        public static bool ParseOption(byte[] plainBytes, out int type, out byte[] outBytes)
        {
            int s = -1;
            outBytes = null;
            var status = -1;
            type = -1;

            if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("HMET OPT"), false)) != -1)
            {
                type = 2;
                return true;
            }
            if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("HMET CONN"), false)) != -1)
            {
                if (plainBytes.Length - s - 32 - "HMET CONN".Length < 0)
                {
                    return false;
                }

                outBytes = new byte[32];
                
                Buffer.BlockCopy(plainBytes, s + "HMET CONN".Length, outBytes, 0, outBytes.Length);
                var r = ParseOptConn(outBytes, out s);
                if (r >= 0 && r <= 2)
                {
                    type = 1;
                    outBytes = new byte[4+4+1];//todo: copy aeskey
                    Buffer.BlockCopy(plainBytes, s + "STS".Length, outBytes, 0, outBytes.Length);
                }
                else
                    return false;

                return true;
            }

            return false;
        }

        public static bool ParseJoinConf(byte[] plain, out IPEndPoint ep, out byte[] idclient)
        {
            ep = null;
            idclient = null;
            var unpkt = plain;
            var s = Parse(unpkt, Encoding.UTF8.GetBytes("HMET CONN OK "), false);
            
            if (s == -1) return false;
            if (s + "HMET CONN OK ".Length + 4 + 4 + 16 > unpkt.Length) return false;

            var ipb = ByteArray.SubArray(unpkt, s + "HMET CONN OK ".Length, 4);
            if (ByteArray.Search(ipb, new byte[]{0,0,0,0}) != -1)
            {
                return false;
            }
            var portb = ByteArray.SubArray(unpkt, s + "HMET CONN OK ".Length + 4, 4);
            idclient = ByteArray.SubArray(unpkt, "HMET CONN OK ".Length + 4 + 4, 16);
            ep = new IPEndPoint(new IPAddress(ipb), BitConverter.ToInt32(portb, 0));

            return true;
        }

        public static bool ParseKeyConf(byte[] hpkt)
        {
            return Parse(hpkt, Encoding.UTF8.GetBytes("HMET KEY OK")) != -1;
        }

        public static bool ParseKey(byte[] hpkt, out byte[] key, string arg = "HMET KEY ")
        {
            int start = -1;
            key = null;

            if ((start = Parse(hpkt, Encoding.UTF8.GetBytes(arg))) != -1)
            {
                if ((hpkt.Length - start) <= (1 + 1 + 32))
                {
                    return false;
                }

                var unpkt = HexPacket.Unpack(hpkt, 0);
                key = new byte[unpkt.Length - arg.Length];
                Buffer.BlockCopy(unpkt, arg.Length, key, 0, key.Length);
                return true;
            }

            return false;
        }

        public static bool ParseAesKeyConf(byte[] hpkt)
        {
            return Parse(hpkt, Encoding.UTF8.GetBytes("HMET AES OK"), false) != -1;
        }

        public static bool ParseHostServ(byte[] plainBytes, out int port, out byte[] control)
        {
            if (plainBytes == null) throw new ArgumentNullException(nameof(plainBytes));
            port = -1;
            control = null;
            int start = -1;
            
            string arg = "HMET HOST OK ";

            if ((start = Parse(plainBytes, Encoding.UTF8.GetBytes(arg), false)) != -1)
            {
                if ((plainBytes.Length - start) < (arg.Length + 4 + controlLen))
                {
                    return false;
                }

                var portBytes = new byte[4];
                control = new byte[controlLen];
                Buffer.BlockCopy(plainBytes, start + arg.Length, portBytes, 0, 4);
                port = BitConverter.ToInt32(portBytes, 0);
                Buffer.BlockCopy(plainBytes, start + arg.Length + portBytes.Length, control, 0, controlLen);
                return true;
            }

            return false;
        }
    }
}
