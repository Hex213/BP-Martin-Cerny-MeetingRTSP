using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibHexCryptoStandard.Hashs;
using LibHexUtils.Arrays;
using LibNet.DNS;
using LibNet.Meeting.Packets.HexPacket;
using LibNet.Meeting.Parsers;
using LibNet.Utils;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace HexServer.Net
{
    public class SessionServer
    {
        private byte[] control;
        private EndPoint adminEP;
        private IPAddress mainIP;
        private Dictionary<IPEndPoint, DataClient> ipClients;
        private int mainPort;

        private RsaKeyParameters adminPubicKey;
        private byte[] aesPass;

        public byte[] Control => control;
        public int MainPort => mainPort;

        private UdpClient listenerControl;
        private TcpListener listenerTCP;
        private IPEndPoint groupEP;

        private object messageSending;
        private object messageReceiving;
        private object lastStateLock;

        public bool messageReceived = false;
        public bool messageSent = false;
        private bool adminSet = false;

        public UDPState lastState = null;

        public SessionServer(IPAddress mainIp, int mainPort, byte[] control, EndPoint adminEp, RsaKeyParameters adminPubicKey)
        {
            this.adminPubicKey = adminPubicKey;
            this.mainPort = mainPort;
            mainIP = mainIp;
            this.control = control;
            adminEP = adminEp;
            listenerControl = new UdpClient(new IPEndPoint(mainIp, mainPort)/*mainPort*/);
            listenerTCP = new TcpListener(mainIp, mainPort);
            IPEndPoint groupEP = new IPEndPoint(mainIp, mainPort);
            ipClients = new Dictionary<IPEndPoint, DataClient>();
            messageSending = new object();
            messageReceiving = new object();
            lastStateLock = new object();
        }

        private void Disconnect()
        {

        }

        private bool compareControl(byte[] recv)
        {
            return ByteArray.Search(recv, SHA.SHA3(control, 256)) != -1;
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
            messageReceived = true;
            lock (lastStateLock)
            {
                lastState = state;
            }
        }

        public UDPState ReceiveMessages(UdpClient listener)
        {
            lock (messageReceiving)
            {
                messageReceived = false;

                var s = new UDPState();
                s.e = groupEP;
                s.u = listenerControl;
                
                s.u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

                // Do some work while we wait for a message. For this example, we'll just sleep
                while (!messageReceived)
                {
                    Thread.Sleep(10);
                }

                return lastState;
            }
        }

        private void DoControlConComm(byte[] data, int status)
        {
            var ipBytes = new byte[] { data[0], data[1], data[2], data[3] };
            var portBytes = new byte[] { data[4], data[5], data[6], data[7] };
            var ipep = new IPEndPoint(new IPAddress(ipBytes), BitConverter.ToInt32(portBytes));
            
            if (ipClients.ContainsKey(ipep))
            {
                if (status == 0)
                {
                    ipClients.Remove(ipep);
                }
                else
                {
                    ipClients[ipep].Blocked = status == 2;
                }
            }
            else
            {
                if (status == 0)
                {
                    return;
                }
                var c = new DataClient(ipep, status == 2);
                ipClients.Add(ipep,c);
                if (!c.Blocked)
                {
                    //todo: send aes data key
                }
            }
        }

        private void DoControlCommand()
        {
            try
            {
                byte[] decr;

                lock (lastStateLock)
                {
                    decr = CipherManager.AesEngine(aesPass, lastState.Buffer, false);
                }
                
                if (Parser.ParseOption(decr, out var type, out var s))
                {
                    switch (type)
                    {
                        case 1:
                            DoControlConComm(s, s[8]);
                            break;
                    }
                }
            }
            catch (Exception)
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

                    s.buffer = CipherManager.AesEngine(aesPass, s.buffer, true);
                    
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

        public void StartServer()
        {
            try
            {
                Console.WriteLine("Session server started at " + mainIP + ":" + mainPort);
                while (true)
                {
                    ReceiveMessages(listenerControl);
                    if (!adminSet)
                    {
                        SetAdmin();
                    }
                    else
                    {
                        ParseInput();
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listenerControl.Close();
            }
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

        public void SendMessageAdmin(byte[] data)
        {
            
            lock (messageSending)
            {
                messageSent = false;
                if (!adminSet)
                {
                    return;
                }

                var sendBytes = CipherManager.AesEngine(aesPass, data, true);
                var send = listenerControl.Send(sendBytes, sendBytes.Length, (IPEndPoint?) adminEP);
                PrintNet.printSend(listenerControl.Client.LocalEndPoint, adminEP, send);

                messageSent = true;
            }
        }

        private bool ConfirmAes()
        {
            byte[] data;

            lock (lastStateLock)
            {
                data = lastState.Buffer;
            }
            
            try
            {
                var decrypted = CipherManager.AesEngine(aesPass, data, false);
                if (!Parser.ParseAesKeyConf(decrypted)) return false;
                Console.WriteLine("("+mainIP+":"+MainPort+"): Admin set: " + adminEP);
                adminSet = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetupAdmin(EndPoint adminEndPoint)
        {
            this.adminEP = adminEndPoint;
            aesPass = CipherManager.GetRandBytes(32);
            var toSend = CipherManager.EncryptRSA(adminPubicKey, aesPass);
            
            SendMessage(toSend, lastState);

            for (var i = 0; i < 3; i++)
            {
                do
                {
                    ReceiveMessages(listenerControl);
                } while (!Equals(lastState.e, adminEndPoint));
                
                if (ConfirmAes())
                { 
                    break;
                }
            }

            if (!adminSet)
            {
                throw new Exception("Not correct admin");
            }
        }

        public void SetAdmin()
        {
            if (!messageReceived) return;

            UDPState us;
            lock (lastStateLock)
            {
                us = lastState;
            }
            try
            {
                var data = us.Buffer;
                var decrypted = CipherManager.DecryptRSA(adminPubicKey, data);
                if (compareControl(decrypted))
                {
                    SetupAdmin(us.e);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
