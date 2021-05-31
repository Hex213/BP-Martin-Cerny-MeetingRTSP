using System;
using System.Collections.Generic;
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
        public static readonly int keyLen = 32;

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

        public static bool ParseConn(byte[] hpkt, out byte[] id, out byte[] port)
        {
            port = null;
            if (!ParseID(hpkt, out id))
            {
                return false;
            }

            var upkt = HexPacket.Unpack(hpkt, 0);
            var s = Parse(upkt, Encoding.UTF8.GetBytes("HMET CONN "), false);
            port = ByteArray.SubArray(upkt, s + "HMET CONN ".Length + id.Length);

            return true;
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

        //HMET CONN(ip)(port)(key)
        public static bool ParseConReq(byte[] plaint, out byte[] ip, out byte[] port, out byte[] key, out byte[] id)
        {
            int s = -1;
            ip = null;
            port = null;
            key = null;
            id = null;

            if ((s = Parse(plaint, Encoding.UTF8.GetBytes("HMET CONN"), false)) == -1) return false;

            ip = ByteArray.SubArray(plaint, s + "HMET CONN".Length, 4);
            port = ByteArray.SubArray(plaint, s + "HMET CONN".Length + 4, 4);
            key = ByteArray.SubArray(plaint, s + "HMET CONN".Length + 4 + 4, plaint.Length - s - "HMET CONN".Length - 4 - 4 - 4);
            id = ByteArray.SubArray(plaint, s + "HMET CONN".Length + 4 + 4 + key.Length, 4);
            return true;
        }

        public static bool ParseProxy(byte[] plainBytes, out int type)
        {
            int s = -1;
            type = -1;

            if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("HMET PROXY OK"), false)) != -1 && s == 0)
            {
                type = 0;
            }
            else if((s = Parse(plainBytes, Encoding.UTF8.GetBytes("HMET PROXY ERR"), false)) != -1 && s == 0)
            {
                type = 1;
            }
            else if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("HMET PROXY CONN OK"), false)) != -1 && s == 0)
            {
                type = 2;
            }

            return s != -1;
        }

        private static int GetInt(byte[] input, int start)
        {
            var tmp = ByteArray.SubArray(input, start, 4);
            return BitConverter.ToInt32(tmp, 0);
        }

        //T0(4)rtpCS,rtcpCS,rtpSA,rtpAS,T1(4)rtpCS,rtcpCS,rtpSA,rtpAS
        //des,clie,des,clie
        public static bool ParsePorts(byte[] plaint, out List<int> ports)
        {
            ports = null;
            int s = -1;
            if ((s = Parse(plaint, Encoding.UTF8.GetBytes("PSS"), false)) == -1) return false;

            ports = new List<int>(9);
            for (int i = 0; i < 8; i++)
            {
                ports.Add(GetInt(plaint, (s + 3) + (4 * i)));
            }
            ////todo: kontrola dlzky pola
            //var rtpSAbytes = ByteArray.SubArray(plaint, s + 3, 4);
            //var rtcpSAbytes = ByteArray.SubArray(plaint, s + 7, 4);
            //var rtpCSbytes = ByteArray.SubArray(plaint, s + 11, 4);
            //var rtcpCSbytes = ByteArray.SubArray(plaint, s + 15, 4);

            //rtpCS = BitConverter.ToInt32(rtpCSbytes, 0);
            //rtcpCS = BitConverter.ToInt32(rtcpCSbytes, 0);
            //rtpSA = BitConverter.ToInt32(rtpSAbytes, 0); //Client na MediaServery
            //rtcpSA = BitConverter.ToInt32(rtcpSAbytes, 0); //Client na MediaServery

            //Console.Write(rtpSA + " - ");
            //ByteArray.Print(rtpSAbytes, "SA");
            //Console.Write(rtcpSA + " - ");
            //ByteArray.Print(rtcpSAbytes, "SAc");
            //Console.Write(rtpCS + " - ");
            //ByteArray.Print(rtpCSbytes, "CS");
            //Console.Write(rtcpCS + " - ");
            //ByteArray.Print(rtcpCSbytes, "CSc");

            return true;
        }

        private static int ParseOptConn(byte[] data)
        {
            var startSTS = ByteArray.Search(data, Encoding.UTF8.GetBytes("STS"));
            return startSTS != -1 ? data[startSTS + "STS".Length] : -1;
        }

        public static bool ParseOption(byte[] plainBytes, out int type, out object outObj)
        {
            int s;
            outObj = null;
            type = -1;

            if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("HMET OPT "), false)) != -1)
            {
                type = 2;
                var or = new OptionReply();
                if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("PORT "), false)) != -1)
                {
                    s += "PORT ".Length;
                    or.Type = OptType.Port;
                }
                else if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("PORT_S "), false)) != -1)
                {
                    s += "PORT_S ".Length;
                    or.Type = OptType.PortServ;
                }
                else
                {
                    s = 0;
                    or.Type = OptType.Unknown;
                    outObj = or;
                    return false;
                }

                var ports = ByteArray.SubArray(plainBytes, s);
                var l = ByteArray.SplitArray(ports, Encoding.UTF8.GetBytes("-"));
                //foreach (var arr in l)
                //{
                //    ByteArray.Print(arr, "ARR");
                //}
                or.AData = l.ToArray();
                outObj = or;
                return true;
            }
            //HMET CONN(xySTS(m)xy)(identifier)(ip)(port)
            if ((s = Parse(plainBytes, Encoding.UTF8.GetBytes("HMET CONN"), false)) != -1)
            {
                var conRep = new ConnectionReply();
                type = 1;
                conRep.Id = new byte[4];
                var sts = new byte[16];

                if (plainBytes.Length - s < "HMET CONN".Length + sts.Length)
                {
                    return false;
                }

                Buffer.BlockCopy(plainBytes, s + "HMET CONN".Length, sts, 0, sts.Length);
                Buffer.BlockCopy(plainBytes, s + "HMET CONN".Length + sts.Length, conRep.Id, 0, conRep.Id.Length);
                var ip = ByteArray.SubArray(plainBytes, s + "HMET CONN".Length + sts.Length + conRep.Id.Length, 4);
                var port = ByteArray.SubArray(plainBytes, s + "HMET CONN".Length + sts.Length + conRep.Id.Length + ip.Length, 4);
                if (s + "HMET CONN".Length + sts.Length + conRep.Id.Length + ip.Length + port.Length != plainBytes.Length)
                {
                    conRep.AData = ByteArray.SubArray(plainBytes, s + "HMET CONN".Length + sts.Length + conRep.Id.Length + ip.Length + port.Length);
                }

                conRep.Ipep = new IPEndPoint(new IPAddress(ip), BitConverter.ToInt32(port, 0));
                conRep.Status = ParseOptConn(sts);

                outObj = conRep;
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

        public static bool ParseHostServ(byte[] unpkt, out IPAddress ip, out int port, out byte[] key)
        {
            if (unpkt == null) throw new ArgumentNullException(nameof(unpkt));
            port = -1;
            key = null;
            ip = null;
            int start = -1;
            
            string arg = "HMET HOST OK ";

            if ((start = Parse(unpkt, Encoding.UTF8.GetBytes(arg), false)) != -1)
            {
                if ((unpkt.Length - start) < (arg.Length + 4 + 4 + keyLen))
                {
                    return false;
                }

                try
                {
                    var ipBytes = new byte[4];
                    var portBytes = new byte[4];
                    key = new byte[keyLen];

                    int starttemp = start + arg.Length;
                    ipBytes = ByteArray.SubArray(unpkt, starttemp, ipBytes.Length);
                    portBytes = ByteArray.SubArray(unpkt, starttemp + ipBytes.Length, portBytes.Length);
                    key = ByteArray.SubArray(unpkt, starttemp + ipBytes.Length + portBytes.Length, keyLen);
                    
                    port = BitConverter.ToInt32(portBytes, 0);
                    ip = new IPAddress(ipBytes);
                    return true;
                }
                catch
                {
                    return false;
                }
                
            }

            return false;
        }

        public static bool IPCParseConf(byte[] buff)
        {
            var s = Parse(buff, Encoding.UTF8.GetBytes("OK"), false);
            return s <= 5 && s >= 0;
        }

        public static bool IPCParseServPorts(byte[] buff, out int rtpPort, out int rtcpPort)
        {
            rtpPort = -1;
            rtcpPort = -1;
            var s = Parse(buff, Encoding.UTF8.GetBytes("PSP="), false);
            if (s == -1)
            {
                return false;
            }

            var rtpBytes = ByteArray.SubArray(buff, s + 4, 4);
            var rtcpBytes = ByteArray.SubArray(buff, s + 4 + 4, 4);

            Array.Reverse(rtpBytes, 0, rtpBytes.Length);
            Array.Reverse(rtcpBytes, 0, rtcpBytes.Length);

            rtpPort = BitConverter.ToInt32(rtpBytes, 0);
            rtcpPort = BitConverter.ToInt32(rtcpBytes, 0);
            return true;
        }

        public static bool IPCParsePorts(byte[] buff, out int rtpPortT0, out int rtcpPortT0, out int rtpPortT1, out int rtcpPortT1)
        {
            rtpPortT0 = -1;
            rtpPortT1 = -1;
            rtcpPortT1 = -1;
            rtcpPortT0 = -1;
            var s = Parse(buff, Encoding.UTF8.GetBytes("HCPORT "), false);
            if (s == -1)
            {
                return false;
            }

            var rtpSub = ByteArray.SubArray(buff, s + "HCPORT ".Length, 4);
            var rtcpSub = ByteArray.SubArray(buff, s + "HCPORT ".Length + 4, 4);
            var rtpSubt1 = ByteArray.SubArray(buff, s + "HCPORT ".Length + 8, 4);
            var rtcpSubt1 = ByteArray.SubArray(buff, s + "HCPORT ".Length + 12, 4);
            Array.Reverse(rtpSub, 0, rtpSub.Length);
            Array.Reverse(rtcpSub, 0, rtcpSub.Length);
            Array.Reverse(rtpSubt1, 0, rtpSubt1.Length);
            Array.Reverse(rtcpSubt1, 0, rtcpSubt1.Length);
            rtpPortT0 = BitConverter.ToInt32(rtpSub, 0);
            rtcpPortT0 = BitConverter.ToInt32(rtcpSub, 0);
            rtpPortT1 = BitConverter.ToInt32(rtpSubt1, 0);
            rtcpPortT1 = BitConverter.ToInt32(rtcpSubt1, 0);
            return true;
        }
    }
}
