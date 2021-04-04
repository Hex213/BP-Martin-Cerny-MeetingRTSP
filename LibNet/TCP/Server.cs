using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibNet.Base;
using LibNet.Meeting;
using LibNet.TCP;
using LibNet.Utils;

namespace LibNet.TCP
{
    public class Server
    {
        private int mainPort;
        private IPAddress mainIP;
        TcpListener listener = null;
        private AdditionalFunctions af;
        private List<ClientTCP> _clientsTcp;

        public Server(IPAddress mainIp, int port, AdditionalFunctions af = null)
        {
            if (port <= 0) throw new ArgumentOutOfRangeException(nameof(port));
            mainIP = mainIp ?? throw new ArgumentNullException(nameof(mainIp));
            mainPort = port;
            this.af = af;
            _clientsTcp = new List<ClientTCP>();
        }

        public Task ReadTask(ClientTCP client)
        {
            client.TcpState.totalRead = 0;
            return ReadNext(client);
        }
        public Task ReadNext(ClientTCP client)
        {
            ReadClient(client);
            return Task.CompletedTask;
        }

        public void ReadClient(ClientTCP client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            client.TcpState.workSocket = client.Handler.Client;
            var state = client.TcpState;

            //while (true)
            //{
            if (!client.Handler.Connected)
            {
                return;
            }
            //client.Ct.Token.ThrowIfCancellationRequested();

            while (client.Stream.DataAvailable == false)
            {
                Thread.Sleep(1);
                //todo: break
            }
            if (state.Buffer.Length - state.TotalRead < 5)
            {
                Console.WriteLine("Buffer for client is too small, buffer will be rewrite!");
                state.totalRead = 0;
            }
            var read = client.Stream.Read(state.buffer, state.TotalRead, state.Buffer.Length - state.TotalRead);
            if (read > 0)
            {
                state.totalRead += read;
                PrintNet.printRead(state.workSocket, read);
            }
            if (af?.RecvCondition((object)client) == true)
            {
                af?.RecvFunc((object)client);
            }
            else
            {
                ReadClient(client);
            }
            //}
            
        }

        public void Send(byte[] data, ClientTCP client)
        {
            if (data == null || data.Length == 0) throw new ArgumentNullException(nameof(data));


            client.Stream.Write(data, 0, data.Length);
            //Console.WriteLine("S " +
            //                  client.Handler.Client.LocalEndPoint + "-(" + data.Length + ")->" +
            //                  client.Handler.Client.RemoteEndPoint);
            PrintNet.printSend(client.Handler.Client, data.Length);
        }

        public void StartServer()
        {
            TcpClient client = null;
            
            try
            {
                listener = new TcpListener(mainIP, mainPort);
                listener.Start();
                Console.WriteLine("Waiting for connections...");
                while (true)
                {
                    client = listener.AcceptTcpClient();
                    if (client.Connected)
                    {
                        var tokenSource = new CancellationTokenSource();
                        var cl = new ClientTCP(client, new TCPState());
                        if (!_clientsTcp.Contains(cl))
                        {
                            _clientsTcp.Add(cl);
                        }
                        var t = Task.Run(() => ReadTask(cl));
                    }
                }
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }

                listener.Stop();
            }
        }
    }
}
