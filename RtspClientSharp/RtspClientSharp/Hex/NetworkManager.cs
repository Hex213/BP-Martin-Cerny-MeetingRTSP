using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibHexCryptoStandard.Algoritm;
using LibHexCryptoStandard.Hashs;
using LibHexCryptoStandard.Packet;
using LibHexCryptoStandard.Packet.RSA;
using LibHexUtils.Arrays;
using LibHexUtils.Random;
using LibNet.Base;
using LibNet.Meeting;
using LibNet.Meeting.Packets.HexPacket;
using LibNet.Meeting.Parsers;
using LibNet.Utils;
using LibRtspClientSharp.Hex.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Net;
using Org.BouncyCastle.X509;
using RtspClientSharp;
using RtspClientSharp.Utils;
using TCP = LibNet.TCP;
using UDP = LibNet.UDP;
using IPAddr = System.Net.IPAddress;

namespace LibRtspClientSharp.Hex
{
    public class NetworkManager
    {
        //control client
        private static TCP.Client _clientControlTcp = null;
        private static UDP.Client _clientDataUdp = null;
        private static System.Net.IPEndPoint _serverIp = null;
        private static ConnectionParameters _connectionParameters = null;

        public static Socket TcpSocket;
        public static Socket UdpSocketRtpT0;
        public static Socket UdpSocketRtcpT0;
        public static Socket UdpSocketRtpT1;
        public static Socket UdpSocketRtcpT1;
        public static ConnectionParameters ConnectionParameters => _connectionParameters;

        public static string errMsg = "";

        public static void InitConParams(ConnectionParameters conParam)
        {
            _connectionParameters = conParam ?? throw new ArgumentNullException(nameof(conParam));
        }

        private static void _connect(ClientBase client, IPAddr ip, int port, byte seconds, byte times)
        {
            byte act = 0;

            do
            {
                try
                {
                    client.Connect(ip, port);
                    break;
                }
                catch (Exception)
                {
                    act++;
                    Thread.Sleep(seconds);
                }
            } while (act < times);

            if (!client.IsConnected())
            {
                throw new ConnectionException("Client cannot connect");
            }
        }

        public static int GetAvailablePort(int startingPort)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            var tcpConnectionPorts = properties.GetActiveTcpConnections()
                .Where(n => n.LocalEndPoint.Port >= startingPort)
                .Select(n => n.LocalEndPoint.Port);

            //getting active tcp listners - WCF service listening in tcp
            var tcpListenerPorts = properties.GetActiveTcpListeners()
                .Where(n => n.Port >= startingPort)
                .Select(n => n.Port);

            //getting active udp listeners
            var udpListenerPorts = properties.GetActiveUdpListeners()
                .Where(n => n.Port >= startingPort)
                .Select(n => n.Port);

            var port = Enumerable
                .Range(startingPort, ushort.MaxValue)
                .Where(i => !tcpConnectionPorts.Contains(i))
                .Where(i => !tcpListenerPorts.Contains(i))
                .FirstOrDefault(i => !udpListenerPorts.Contains(i));

            return port;
        }

        private static void CreateUdp(ref Socket UdpSocket)
        {
            UdpClient udpClient = new UdpClient();
            
            //UdpSocket?.Dispose();
            UdpSocket?.Close();
            UdpSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveBufferSize = NetworkClientFactory.GetUdpReceiveBufferDefaultSize,
                DualMode = true
            };
            UdpSocket.IOControl((IOControlCode)NetworkClientFactory.GetSIO, NetworkClientFactory.GetEmptyOptInVal, null);
            IPEndPoint freeip = new IPEndPoint(IPAddr.Loopback, GetAvailablePort(49152));
            UdpSocket.Bind(freeip);
        }
        private static void DataSockets()
        {
            //TcpSocket?.Dispose();
            TcpSocket?.Close();
            TcpSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = NetworkClientFactory.GetTcpReceiveBufferDefaultSize,
                DualMode = true,
                NoDelay = true
            };
            IPEndPoint freeip = new IPEndPoint(IPAddr.Loopback, GetAvailablePort(49152));
            TcpSocket.Bind(freeip);
            CreateUdp(ref UdpSocketRtpT0);
            CreateUdp(ref UdpSocketRtcpT0); 
            CreateUdp(ref UdpSocketRtpT1);
            CreateUdp(ref UdpSocketRtcpT1);
            Console.WriteLine("My ports:\nt0: rtp=" + ((IPEndPoint) UdpSocketRtpT0.LocalEndPoint).Port + ",rtcp=" + ((IPEndPoint)UdpSocketRtcpT0.LocalEndPoint).Port +
                              "\nt1: rtp=" + ((IPEndPoint)UdpSocketRtpT1.LocalEndPoint).Port + ",rtcp=" + ((IPEndPoint)UdpSocketRtcpT1.LocalEndPoint).Port);
        }

        public static void ConnectBase(System.Net.IPAddress ip, int port, byte seconds, byte times)
        {
            if (_connectionParameters == null) throw new NullReferenceException(nameof(_connectionParameters));
            if (ip == null) throw new ArgumentNullException(nameof(ip));
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port", "Port <= 0");
            }

            if (ConnectionParameters.UseServer)
            {
                DataSockets();
            }

            _clientControlTcp?.Release();
            _clientControlTcp = new TCP.Client();
            var client = _clientControlTcp;

            _connect(_clientControlTcp, ip, port, 2, 5);

            var b = HexPacket.Pack(Encoding.UTF8.GetBytes("HMET HI"));

            client.Send(b);
            var s = (TCPState)client.Receive();
            if (!Parser.ParseConConf(s.Buffer))
            {
                //todo: wrong recv
            }

            b = Encoding.UTF8.GetBytes("HMET KEY ");
            var key = CipherManager.GetKeyToSend();
            var keybytes = new byte[b.Length + key.Length];

            Buffer.BlockCopy(b, 0, keybytes, 0, b.Length);
            Buffer.BlockCopy(key, 0, keybytes, b.Length, key.Length);

            b = HexPacket.Pack(keybytes);
            client.Send(b);
            s = (TCPState)client.Receive();
            if (!Parser.ParseKeyConf(s.Buffer))
            {
                //todo: wrong recv
            }

            if (!_connectionParameters.UseServer)
            {
                s = (TCPState)client.Receive(true);
                if (!Parser.ParseKey(s.Buffer, out key))
                {
                    throw new Exception("Wrong key!");
                }

                CipherManager.RegisterVK(key);
                b = Encoding.UTF8.GetBytes("HMET KEY OK");
                client.Send(b);
            }
        }

        private static void HostSerMet(string id)
        {
            //if(_connectionParameters.UseServer) {
            var idbytes = SHA.SHA3(Encoding.UTF8.GetBytes(id), 256);
            var b = //CipherManager.ProcessByPrivate(
                ByteArray.CopyBytes(0,
                    Encoding.UTF8.GetBytes("HMET HOST "), idbytes);//, 
             //   true, true);

            _clientControlTcp.Send(HexPacket.Pack(b));
            var s = (TCPState)_clientControlTcp.Receive();

            b = CipherManager.GetControlConDet(s.Buffer);
            if (!Parser.ParseHostServ(b, out var ip, out var port, out var key))
            {
                //todo: wrong recv
            }

            var confMsgBytes = CipherManager.EncryptControl(key);
            _clientDataUdp?.Release();
            _clientDataUdp = new UDP.Client();


            _connect(_clientDataUdp, ip, port, 1, 5);
            _serverIp = new IPEndPoint(ip, port);
            _clientDataUdp.Send(confMsgBytes);
        }

        private static void Parse(byte[]data)
        {
            if (data == null)
            {
                return;
            }

            if (Parser.ParseConReq(data, out var ip, out var port, out var key, out var id))
            {
                var IPEP = new IPEndPoint(new System.Net.IPAddress(ip), BitConverter.ToInt32(port, 0));
                Console.WriteLine("Confirm connection from: " + IPEP + "\n"
                                  + "id:" + Convert.ToBase64String(SHA.SHA3(ByteArray.SubArray(key, 1, key.Length-33), 256))
                                  + "\nConfirm(y/n/b): ");
                var ch_conf = Console.ReadLine();
                int mode = ch_conf[0] == 'y' ? 0 : 
                    (ch_conf[0] == 'n' ? 1 : (
                        ch_conf[0] == 'b' ? 2 : -1));

                var tosend = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET CONN"),
                    HexRandom.GetRandomBytesWithMessage(16,
                        ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("STS"), new byte[] {(byte) mode})),
                    id, ip, port);

                if (mode == 0)
                {
                    var client_vk = HexPacketRSA.GetKeyFromPacket(key, false, false);
                    tosend = ByteArray.CopyBytes(0, tosend, CipherManager.EncryptDataKeyWithIPEP(client_vk, 
                        _serverIp));
                }

                tosend = CipherManager.EcryptAdminData(tosend);
                _clientDataUdp.Send(tosend);
            }
            else if(Parser.ParsePorts(data, out var listPorts))
            {
                //todo: kontrola porty mimo rozsah
                //Odosielam co ma byt server a co client
                var ipcBytes = ByteArray.CopyBytes(0, 
                    Encoding.UTF8.GetBytes("CL0="), BitConverter.GetBytes(listPorts[2]), BitConverter.GetBytes(listPorts[3]),//Maskovane t0
                    Encoding.UTF8.GetBytes("SR0="), BitConverter.GetBytes(listPorts[0]), BitConverter.GetBytes(listPorts[1]),//Server port t0
                    Encoding.UTF8.GetBytes("CL1="), BitConverter.GetBytes(listPorts[6]), BitConverter.GetBytes(listPorts[7]),//Maskovane t1
                    Encoding.UTF8.GetBytes("SR1="), BitConverter.GetBytes(listPorts[4]), BitConverter.GetBytes(listPorts[5]));//Server port t1

                IPC.Send(ipcBytes);
            }
            else if(Parser.ParseProxy(data, out var type))
            {
                byte[] ipcBytes = Encoding.UTF8.GetBytes("PROXY ");
                switch (type)
                {
                    case 0:
                        ByteArray.AppendArray(ref ipcBytes, Encoding.UTF8.GetBytes("OK"));
                        break;
                    case 1:
                        ByteArray.AppendArray(ref ipcBytes, Encoding.UTF8.GetBytes("NO"));
                        break;
                    case 2:
                        ByteArray.AppendArray(ref ipcBytes, Encoding.UTF8.GetBytes("CONN OK"));
                        break;
                    default:
                        ByteArray.AppendArray(ref ipcBytes, Encoding.UTF8.GetBytes("ERR"));
                        break;
                }

                IPC.Send(ipcBytes);
            }
        }

        private static object lRecvAdmin = new object();

        private static void StartRecvAdmin()
        {
            while (true)
            {
                lock (lRecvAdmin)
                {
                    var recv = (UDPState)_clientDataUdp.Receive();
                    try
                    {
                        if (!recv.e.Equals(_serverIp))
                        {
                            continue;//Ak chcem priame pripojenie tak tu
                        }
                        var decrypt = CipherManager.DecryptAdminData(recv?.Buffer);
                        Parse(decrypt);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        private static void ParseIPC(byte[] buff)
        {
            if (Parser.IPCParsePorts(buff, out var rtp, out var rtcp, out var rtpT1, out var rtcpT1))
            {
                Console.WriteLine("\nPARSE HCPORT " + rtp + ", " + rtcp);
                var tosend = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET OPT PORT "),
                    BitConverter.GetBytes(rtp), Encoding.UTF8.GetBytes("-"), BitConverter.GetBytes(rtcp), Encoding.UTF8.GetBytes("-"),
                    BitConverter.GetBytes(rtpT1), Encoding.UTF8.GetBytes("-"), BitConverter.GetBytes(rtcpT1));
                tosend = CipherManager.EcryptAdminData(tosend);
                _clientDataUdp.Send(tosend);
            }
            else if (Parser.IPCParseServPorts(buff, out rtp, out rtcp))
            {
                Console.WriteLine("\nPARSE PSP " + rtp + ", " + rtcp);
                var toSend = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET OPT PORT_S "),
                    BitConverter.GetBytes(rtp), Encoding.UTF8.GetBytes("-"), BitConverter.GetBytes(rtcp));
                toSend = CipherManager.EcryptAdminData(toSend);
                _clientDataUdp.Send(toSend);
            }
            else if(Parser.IPCParseConf(buff))
            {
                IPC.conf = true;
            }
        }

        public static object IPC_Read(object o)
        {
            var buffer = (byte[]) o;
            int ReadLength = 0;
            for (int i = 0; i < 100; i++)
            {
                if (buffer[i].ToString("x2") != "cc")
                {
                    ReadLength++;
                }
                else
                    break;
            }
            if (ReadLength > 0)
            {
                byte[] Rc = new byte[ReadLength];
                Buffer.BlockCopy(buffer, 0, Rc, 0, ReadLength);

                Console.WriteLine("C# App: Received " + ReadLength);
                ParseIPC(Rc);
                buffer.Initialize();
            }

            return null;
        }

        private static object l = new object();
        private static bool conar = false;
        private static bool conreply = false;
        private static bool reply = false;
        private static string id = "null";

        public static void SetReply(bool allowConn)
        {
            reply = allowConn;
            conreply = true;
        }

        public static string ConnArrived()
        {
            string ret;
            lock (l)
            {
                while (true)
                {
                    if (conar) break;
                    Thread.Sleep(5);
                }

                ret = id;
                id = "null";
            }

            conar = false;
            return ret;
        }

        private static void WaitReply()
        {
            while(true)
            {
                if (conreply) break;
                Thread.Sleep(5);
            }

            conreply = false;
        }

        private static void Recv(int type, TcpClient tcp, object obj)
        {
            switch (type)
            {
                case 1:
                    _clientControlTcp.Send(HexPacket.Pack(Encoding.UTF8.GetBytes("HMET OK")));
                    break;
                case 2:
                    CipherManager.RegisterVK((byte[])obj);
                    tcp.Client.Send(HexPacket.Pack(Encoding.UTF8.GetBytes("HMET KEY OK")));
                    Thread.Sleep(100);
                    var b = Encoding.UTF8.GetBytes("HMET KEY ");
                    var key = CipherManager.GetKeyToSend();
                    var keybytes = new byte[b.Length + key.Length];

                    Buffer.BlockCopy(b, 0, keybytes, 0, b.Length);
                    Buffer.BlockCopy(key, 0, keybytes, b.Length, key.Length);
                    
                    b = HexPacket.Pack(keybytes);
                    _clientControlTcp.Send(b);
                    b = new byte[4096];
                    int r = tcp.Client.Receive(b);
                    break;
                case 3:
                    if (Parser.Parse((byte[]) obj, _connectionParameters.ID, false) == 0)
                    {
                        var toHash = ByteArray.CopyBytes(0, CipherManager.vkDes.Exponent.ToByteArray(), CipherManager.vkDes.Modulus.ToByteArray());
                        var hashID = SHA.SHA3(toHash, 256);
                        id = Convert.ToBase64String(hashID);
                        conar = true;
                        WaitReply();

                        if (reply)
                        {
                            CipherManager.Test();
                            var buff = CipherManager.EncryptDataKeyWithIPEP(CipherManager.vkDes,
                                new IPEndPoint(((IPEndPoint) tcp.Client.RemoteEndPoint).Address, 8554));

                            //var tpkt = new HexPacketRSA(buff);
                            _clientControlTcp.Send(buff);
                        }

                        reply = false;
                    }
                    break;
                case 4:
                    byte[] data = null;
                    try
                    {
                        data = CipherManager.ProcessByPrivate((byte[]) obj, false, true);
                    }
                    catch
                    {
                        // ignored
                    }

                    if (data == null) break;
                    if ((type = Parser.ParseCH(data, data.Length, out obj, false)) == 3)
                    {
                        Recv(type, tcp, obj);
                    }
                    break;
                case 0:
                default:
                    Console.WriteLine("Unknown communication!");
                    break;
            }
        }

        private static void HostMet()
        {
            _clientControlTcp = new TCP.Client();
            _clientControlTcp.HostRecvFunc(Recv);
            _clientControlTcp.RunListener(40000);
        }

        public static void HostMet(string id, string key)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (key == null && _connectionParameters.Enryption) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Value cannot be null or empty.", nameof(id));
            if (string.IsNullOrEmpty(key) && _connectionParameters.Enryption) throw new ArgumentException("Value cannot be null or empty.", nameof(key));

            _connectionParameters.ID = SHA.SHA3(id, 256);

            if (_connectionParameters.Enryption)
            {
                CipherManager.InitEncryption(key);
            }

            Task.Run(() =>
            {
            if (_connectionParameters.UseServer)
            {
                HostSerMet(id);
            }
            else
            {
                HostMet();
            }
            });

            IPC.Start("HMET" + id, IPC_Read);
            if (_connectionParameters.Enryption)
            {
                IPC.Send(ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("KEY"), CipherManager.GetDataKey()));
                IPC.WaitForConf();
            }
            
            if(_connectionParameters.UseServer)
                StartRecvAdmin();
        }

        public static IPEndPoint ConnMet(string metID)
        {
            var buff = _connectionParameters.UseServer ? ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET CONN "), SHA.SHA3(metID, 256),
                BitConverter.GetBytes(((IPEndPoint) TcpSocket.LocalEndPoint).Port)) : ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET CONN "), SHA.SHA3(metID, 256));

            if(_connectionParameters.UseServer)
            {
                _clientControlTcp.Send(HexPacket.Pack(buff));
            }
            else
            {
                var pkt = new HexPacketRSA(buff);
                _clientControlTcp.Send(pkt.Encrypt(CipherManager.vkDes));
            }
            //datakey = null;
            try
            {
                _clientControlTcp.ResetBuffer();
                var s = (TCPState)_clientControlTcp.ReceiveAndWait(20*1000);

                if (s?.Buffer == null)
                {
                    return null;
                }

                var decr = CipherManager.ProcessByPrivate(s.Buffer, false, true);
                if ((decr.Length != 40 && ConnectionParameters.Enryption) ||
                    (decr.Length != 9 && !ConnectionParameters.Enryption))
                {
                    return null;
                }

                var ip = ByteArray.SubArray(decr, 0, 4);
                var port = ByteArray.SubArray(decr, 4, 4);
                var datakey = ByteArray.SubArray(decr, 4 + 4);
                if(ConnectionParameters.Enryption) CipherManager.InitEncryption(datakey);
                return new IPEndPoint(new System.Net.IPAddress(ip), BitConverter.ToInt32(port, 0));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                errMsg = e.Message;
                return null;
            }
        }

        public static void updateUri(Uri uri)
        {
            var cparam = new ConnectionParameters(uri)
            {
                UseBase64 = _connectionParameters.UseBase64,
                CancelTimeout = _connectionParameters.CancelTimeout,
                ConnectTimeout = _connectionParameters.ConnectTimeout,
                Enryption = _connectionParameters.Enryption,
                ReceiveTimeout = _connectionParameters.ReceiveTimeout,
                RequiredTracks = _connectionParameters.RequiredTracks,
                RtpTransport = _connectionParameters.RtpTransport,
                UserAgent = _connectionParameters.UserAgent,
                UseServer = _connectionParameters.UseServer
            };

            _connectionParameters = cparam;
        }
    }
}
