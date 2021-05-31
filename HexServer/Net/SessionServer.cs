using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibHexCryptoStandard.Hashs;
using LibHexCryptoStandard.Packet;
using LibHexUtils;
using LibHexUtils.Arrays;
using LibHexUtils.Counters.Numeric;
using LibHexUtils.Random;
using LibHexUtils.Register;
using LibNet.DNS;
using LibNet.Meeting.Packets.HexPacket;
using LibNet.Meeting.Parsers;
using LibNet.Utils;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

using TCP = LibNet.TCP;
using UDP = LibNet.UDP;

namespace HexServer.Net
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class RegSes
    {
        #region flags
        //Get admin connection
        public static readonly int GAS = 1;
        //Admin is set
        public static readonly int AIS = 1 << 1;
        //Request admin reply
        public static readonly int RAR = 1 << 2;
        #endregion
    }

    public class SessionServer
    {
        private EndPoint adminEP;
        private IPAddress serverIP;
        private int serverPort;
        private Dictionary<IPEndPoint, DataClient> ipClients;
        private Dictionary<int, byte[]> _adminReply;

        private RsaKeyParameters adminPubicKey;
        private byte[] aesPassAdmin = null;

        public int ServerPort => serverPort;
        public IPAddress ServerIP => serverIP;

        private UdpClient listenerControl;
        private TcpListener listenerTCP;
        private IPEndPoint groupEP;

        private object messageSending;
        private object messageReceiving;
        private object lastStateLock;

        private int register = 0;
        public bool messageReceived = false;
        public bool messageSent = false;

        public UDPState lastState = null;

        public byte[] GetAdminConnection()
        {
            if (Register.IsPresent(register, RegSes.GAS))
            {
                //todo: throw / get 2 times
            }

            var i = serverIP.GetAddressBytes();
            var p = BitConverter.GetBytes(ServerPort);
            Register.AddFlag(ref register, RegSes.GAS);
            return ByteArray.CopyBytes(0, i, p, aesPassAdmin);
        }

        public SessionServer(IPAddress serverIp, int serverPort, byte[] adminKey, EndPoint adminEp, RsaKeyParameters adminPubicKey)
        {
            this.adminPubicKey = adminPubicKey;
            this.serverPort = serverPort;
            serverIP = serverIp;
            this.aesPassAdmin = adminKey;
            adminEP = adminEp;
            IPEndPoint groupEP = new IPEndPoint(serverIp, serverPort);
            ipClients = new Dictionary<IPEndPoint, DataClient>();
            _adminReply = new Dictionary<int, byte[]>();
            messageSending = new object();
            messageReceiving = new object();
            lastStateLock = new object();
        }

        private void Disconnect()
        {
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = (UDPState)(ar.AsyncState);
            UdpClient u = state.u;
            IPEndPoint e = state.e;

            state.buffer = u.EndReceive(ar, ref e);

            state.e = e;
            PrintNet.printRead(u.Client.LocalEndPoint, e, state.Buffer.Length);
            //Console.WriteLine($"{u.Client.LocalEndPoint}<-({state.Buffer.Length})-{e}");
            
            lock (lastStateLock)
            {
                lastState = state;
            }
            messageReceived = true;
        }

        /// <summary>
        /// Čaká na správy od listener-a
        /// </summary>
        /// <param name="listener">Udp klient na ktorý sa očakáva príchod správy</param>
        /// <returns>Vráti stav udp z načítanej správy</returns>
        public UDPState ReceiveMessages(UdpClient listener)
        {
            lock (messageReceiving)
            {
                messageReceived = false;

                var s = new UDPState();
                s.e = groupEP;
                s.u = listener;
                
                s.u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
                
                // Do some work while we wait for a message. For this example, we'll just sleep
                while (!messageReceived)
                {
                    Thread.Sleep(10);
                }

                return lastState;
            }
        }

        public byte[] WaitForRequestFromAdmin(int id)
        {
            byte[] reply = null;

            while (reply == null)
            {
                _ = _adminReply.TryGetValue(id, out reply);
                Thread.Sleep(10);
            }
            
            _adminReply.Remove(id);

            return reply;
        }

        private void DoControlConnCom(ConnectionReply cr)
        {
            var _id = BitConverter.ToInt32(cr.Id); //TODO: dorobiť client ID
            if (cr.Status == 0 || cr.Status == 2) //allow, block
            {
                if (ipClients.ContainsKey(cr.Ipep))
                {
                    ipClients[cr.Ipep].Blocked = cr.Status != 0;
                }
                else
                {
                    var client = new DataClient(cr.Ipep, cr.Status != 0);
                    ipClients.Add(cr.Ipep, client);
                }
            }
            else if(cr.Status == 1) // deny
            {
                if (ipClients.ContainsKey(cr.Ipep))
                {
                    ipClients.Remove(cr.Ipep);
                }
            }

            if (_adminReply.ContainsKey(_id))
            {
                _adminReply[_id] = cr.AData;
            }
        }

        private TCP.Proxy tcpProxy = null;
        private IPEndPoint clientRTP_t0 = null;
        private IPEndPoint clientRTCP_t0 = null;
        private IPEndPoint clientRTP_t1 = null;
        private IPEndPoint clientRTCP_t1 = null;
        private IPEndPoint serverRTP_t0 = null;
        private IPEndPoint serverRTCP_t0 = null;
        private IPEndPoint serverRTP_t1 = null;
        private IPEndPoint serverRTCP_t1 = null;
        private UDP.Proxy ProxyRTP_t0;
        private UDP.Proxy ProxyRTCP_t0;
        private UDP.Proxy ProxyRTP_t1;
        private UDP.Proxy ProxyRTCP_t1;

        private void _createIPEP(out IPEndPoint ipEndPoint, IPAddress ip, byte[] port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port));
            if (port.Length != 4) throw new ArgumentException("Port has not 4 bytes!", nameof(port));
            var tmpPort = BitConverter.ToInt32(port, 0);
            ipEndPoint = new IPEndPoint(ip, tmpPort);
        }

        private bool fs = false;

        private void DoOption(OptionReply or)
        {
            if (or.Type == OptType.Port)
            {
                var firstElement = ipClients.FirstOrDefault();
                _createIPEP(out clientRTP_t0, firstElement.Key.Address, (byte[])or.AData[0]);
                _createIPEP(out clientRTCP_t0, firstElement.Key.Address, (byte[])or.AData[1]);
                _createIPEP(out clientRTP_t1, firstElement.Key.Address, (byte[])or.AData[2]);
                _createIPEP(out clientRTCP_t1, firstElement.Key.Address, (byte[])or.AData[3]);

                ProxyRTP_t0.ConnectToSource(clientRTP_t0);
                ProxyRTCP_t0.ConnectToSource(clientRTCP_t0);
                ProxyRTP_t1.ConnectToSource(clientRTP_t1);
                ProxyRTCP_t1.ConnectToSource(clientRTCP_t1);
            }
            else if (or.Type == OptType.PortServ)
            {
                var firstElement = ipClients.FirstOrDefault();
                if (!fs)
                {
                    _createIPEP(out serverRTP_t0, firstElement.Key.Address, (byte[])or.AData[0]);
                    _createIPEP(out serverRTCP_t0, firstElement.Key.Address, (byte[])or.AData[1]);
                    ProxyRTP_t0.ConnectToDestination(serverRTP_t0);
                    ProxyRTCP_t0.ConnectToDestination(serverRTCP_t0);
                    fs = true;
                    var Send = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET PROXY CONN OK"), HexRandom.GetRandomBytes(19));
                    SendMessageAdmin(Send);
                    return;
                }
                else
                {
                    _createIPEP(out serverRTP_t1, firstElement.Key.Address, (byte[])or.AData[0]);
                    _createIPEP(out serverRTCP_t1, firstElement.Key.Address, (byte[])or.AData[1]);
                    ProxyRTP_t1.ConnectToDestination(serverRTP_t1);
                    ProxyRTCP_t1.ConnectToDestination(serverRTCP_t1);
                    var Send = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET PROXY CONN OK"), HexRandom.GetRandomBytes(19));
                    SendMessageAdmin(Send);
                }

                if (ProxyRTP_t0.IsConnected() && ProxyRTCP_t0.IsConnected() && ProxyRTP_t1.IsConnected() && ProxyRTCP_t1.IsConnected())
                {
                    ProxyRTP_t0.StartProxy();
                    ProxyRTCP_t0.StartProxy();
                    ProxyRTP_t1.StartProxy();
                    ProxyRTCP_t1.StartProxy();
                    ProxyRTP_t0.PrintProxy();
                    ProxyRTCP_t0.PrintProxy();
                    ProxyRTP_t1.PrintProxy();
                    ProxyRTCP_t1.PrintProxy();
                    var Send = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET PROXY OK"), HexRandom.GetRandomBytes(19));
                    SendMessageAdmin(Send);
                }
                else
                {
                    var Send = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET PROXY ERR"),
                        HexRandom.GetRandomBytes(18));
                    SendMessageAdmin(Send);
                    
                }
            }
        }

        private void DoControlCommand()
        {
            try
            {
                byte[] buf = null;
                lock (lastStateLock)
                {
                    buf = lastState.Buffer;
                }

                //if(buf.Length == 0) return;
                
                var decr = CipherManager.AesEngine(aesPassAdmin, buf, false);

                if (Parser.ParseOption(decr, out var type, out var obj))
                {
                    switch (type)
                    {
                        case 1:
                            DoControlConnCom((ConnectionReply)obj);
                            break;
                        case 2:
                            DoOption((OptionReply) obj);
                            break;
                    }
                }
            }
            catch
            {
                //ignore
            }
        }

        private void DoConnection()
        {
            UDPState s;

            lock (lastStateLock)
            {
                s = lastState;
            }
            
            if (Parser.ParseKey(s.Buffer, out var key, "HMET CONN"))
            {
                Task.Run(() => {
                    var ip = s.e.Address.GetAddressBytes();
                    var port = BitConverter.GetBytes(s.e.Port);
                    s.buffer = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("HMET CONN"), ip, port, key);

                    s.buffer = CipherManager.AesEngine(aesPassAdmin, s.buffer, true);
                    
                    s.e = (IPEndPoint) adminEP;
                    s.u = listenerControl;
                    
                    SendMessage(s.buffer, s);
                });
            }
        }

        private void ParseInput()
        {
            if (lastState.e.Equals(adminEP))
            {
                DoControlCommand();
            }
            else
            {
                DoConnection();
            }

        }

        private void StartClient(TcpClient client)
        {
            var state = new TCPState();
            client.ReceiveTimeout = 10;
            var cta = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = 10,
                ReceiveBufferSize = 2048,
                DualMode = true,
                NoDelay = true
            };
            cta.Connect(((IPEndPoint) adminEP).Address, 8554);
            state.setSocket(client);
            tcpProxy = new TCP.Proxy(null, null, client.Client, cta, true, true, false, true);

            ProxyRTP_t0 = new UDP.Proxy(true, true, true);
            ProxyRTP_t0.CreateProxyAndBind(IPAddress.Any); 
            ProxyRTCP_t0 = new UDP.Proxy(true, true, true);
            ProxyRTCP_t0.CreateProxyAndBind(IPAddress.Any);
            ProxyRTP_t1 = new UDP.Proxy(true, true, true);
            ProxyRTP_t1.CreateProxyAndBind(IPAddress.Any);
            ProxyRTCP_t1 = new UDP.Proxy(true, true, true);
            ProxyRTCP_t1.CreateProxyAndBind(IPAddress.Any);

            try
            {
                ProxyRTP_t0.GetPorts(out var ssCliRtp_t0, out var ssDesRtp_t0);
                ProxyRTCP_t0.GetPorts(out var ssCliRtcp_t0, out var ssDesRtcp_t0);
                ProxyRTP_t1.GetPorts(out var ssCliRtp_t1, out var ssDesRtp_t1);
                ProxyRTCP_t1.GetPorts(out var ssCliRtcp_t1, out var ssDesRtcp_t1);
                var buff = ByteArray.CopyBytes(0, Encoding.UTF8.GetBytes("PSS"), 
                    BitConverter.GetBytes(ssCliRtp_t0),//pre media je to server port (poziadavka)
                    BitConverter.GetBytes(ssCliRtcp_t0),// ++
                    BitConverter.GetBytes(ssDesRtp_t0),//pre media je to client (maskovane)
                    BitConverter.GetBytes(ssDesRtcp_t0),// ++
                    BitConverter.GetBytes(ssCliRtp_t1),//pre media je to server port (poziadavka)
                    BitConverter.GetBytes(ssCliRtcp_t1),// ++
                    BitConverter.GetBytes(ssDesRtp_t1),//pre media je to client (maskovane)
                    BitConverter.GetBytes(ssDesRtcp_t1));// ++
                SendMessageAdmin(buff);
                tcpProxy.StartProxy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                tcpProxy.StopProxy();
                //todo: dispose proxy all
            }
        }

        public void StartServer()
        {
            Console.WriteLine("Session server starting at " + serverIP + ":" + serverPort);
            Task.Run(() =>
            {
                while (!Register.IsPresent(register, RegSes.AIS/*adminSet*/))
                {
                    Thread.Sleep(10);
                }
                listenerTCP = new TcpListener(serverIP, serverPort);
                try
                {
                    listenerTCP.Start();
                    Console.WriteLine("TCP thread started!");

                    while (true)
                    {
                        TcpClient client = listenerTCP.AcceptTcpClient();
                        if (ipClients.ContainsKey((IPEndPoint) client.Client.RemoteEndPoint ?? throw new InvalidOperationException()))
                        {
                            Console.WriteLine("Con confirm from: " + client.Client.RemoteEndPoint.ToString());
                            Task.Run(() => StartClient(client));
                        }
                        else
                        {
                            client.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            Task.Run(() =>
            {

                listenerControl = new UdpClient(new IPEndPoint(serverIP, serverPort)/*serverPort*/);
                Console.WriteLine("UDP thread started!");
                try
                {
                    while (true)
                    {
                        ReceiveMessages(listenerControl);
                        if (!Register.IsPresent(register, RegSes.AIS/*adminSet*/))
                        {
                            SetAdmin();
                        }
                        else
                        {
                            ParseInput();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                listenerControl.Close();
            });
        }

        public void SendMessage(byte[] data, UDPState state)
        {
            lock (messageSending)
            {
                // create the udp socket
                messageSent = false;
                UdpClient u = state.U ?? new UdpClient();

                //byte[] sendBytes = data;
                var send = u.Send(data, data.Length, state.e);
                PrintNet.printSend(u.Client.LocalEndPoint, state.e, send);

                messageSent = true;
            }
        }

        private int GetNewIdReply()
        {
            int i = 0;
            while (_adminReply.ContainsKey(i))
            {
                i++;
            }

            return i;
        }

        public int SendMessageAdmin(byte[] data, bool waitForReply = false)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(data));

            int ret = -1;
            if (waitForReply)
            {
                ret = GetNewIdReply();
                _adminReply.Add(ret, null);
            }

            data = ByteArray.CopyBytes(0, data, BitConverter.GetBytes(ret));

            lock (messageSending)
            {
                messageSent = false;
                if (!Register.IsPresent(register, RegSes.AIS/*adminSet*/))
                {
                    return -1;
                }

                var sendBytes = CipherManager.AesEngine(aesPassAdmin, data, true);
                var send = listenerControl.Send(sendBytes, sendBytes.Length, (IPEndPoint?) adminEP);
                PrintNet.printSend(listenerControl.Client.LocalEndPoint, adminEP, send);

                messageSent = true;
            }

            return ret;
        }

        public void SetAdmin()
        {
            UDPState us;

            if (!messageReceived) return;

            lock (lastStateLock)
            {
                us = lastState;
            }

            var data = us.Buffer;
            try
            {
                var decrypted = CipherManager.AesEngine(aesPassAdmin, data, false);
                if (ByteArray.Search(decrypted, Encoding.UTF8.GetBytes("HMET AES OK")) == -1) return;

                this.adminEP = us.e;
                Register.AddFlag(ref register, RegSes.AIS);//adminSet = true;
                Console.WriteLine(serverIP+":"+serverPort+": Admin successful set on: " + adminEP);
            }
            catch
            {
                // ignored
            }
        }
    }
}
