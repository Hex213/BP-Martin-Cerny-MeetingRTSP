using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibNet.Base;
using LibNet.UDP;
using LibNet.Utils;

namespace LibNet.TCP
{
    public class Client : ClientBase
    {
        private IPAddress _ip;
        private int _port;
        private ClientTCP client;
        private NetworkStream stream;

        public Client()
        {
        }

        public override void Connect(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
            IPEndPoint ep = new IPEndPoint(ip, port);
            TcpClient client = new TcpClient();
            client.Connect(ep);
            stream = client.GetStream();
            this.client = new ClientTCP(client, new TCPState());
        }

        public override bool IsConnected()
        {
            return client != null && client.Handler.Connected;
        }

        public override void Send(byte[] bytes)
        {
            stream.WriteAsync(bytes, 0, bytes.Length);
            PrintNet.printSend(client.Handler.Client, bytes.Length);
        }

        public override object Receive()
        {
            var s = client.TcpState;

            var read = stream.Read(s.buffer, s.totalRead, s.BufferSize - s.totalRead);
            PrintNet.printRead(client.Handler.Client, read);

            return s;
        }

        public override void Release()
        {
        }
        //private int port;
        //private IPAddress ipAddress;
        //private Socket client;
        //private bool _connected;
        //private int readOffset;

        //// ManualResetEvent instances signal completion.  
        //private  ManualResetEvent connectDone =
        //    new ManualResetEvent(false);
        //private  ManualResetEvent sendDone =
        //    new ManualResetEvent(false);
        //private  ManualResetEvent receiveDone =
        //    new ManualResetEvent(false);

        //// The response from the remote device.  
        //private byte[] buf;
        //private int netBytes = 0;

        //public int GetBytes() => netBytes;
        //public byte[] GetBuffer() => buf;

        //public override bool IsConnected()
        //{
        //    var part1 = client.Poll(1000, SelectMode.SelectRead);
        //    var part2 = (client.Available == 0);
        //    return !part1 || !part2;
        //}

        //public Client()
        //{
        //    _connected = false;
        //    readOffset = 0;
        //}

        //public override void Connect(IPAddress ipAddress, int port)
        //{
        //    this.port = port;
        //    this.ipAddress = ipAddress;

        //    // Connect to a remote device.  
        //    try
        //    {
        //        IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

        //        // Create a TCP/IP socket.  
        //        client = new Socket(ipAddress.AddressFamily,
        //            SocketType.Stream, ProtocolType.Tcp);

        //        // Connect to the remote endpoint.  
        //        client.BeginConnect(remoteEP,
        //            new AsyncCallback(ConnectCallback), client);
        //        connectDone.WaitOne();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}

        //private void ConnectCallback(IAsyncResult ar)
        //{
        //    //try
        //    //{
        //    // Retrieve the socket from the tcpState object.  
        //    Socket client = (Socket)ar.AsyncState;

        //    // Complete the connection.  
        //    client.EndConnect(ar);

        //    Console.WriteLine("Socket connected to {0}",
        //        client.RemoteEndPoint.ToString());

        //    // Signal that the connection has been made.  
        //    connectDone.Set();
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine(e.ToString());
        //    //}
        //}

        //public void SendTest()
        //{
        //    Send(new byte[]{0,1,2,3,4,5});
        //    sendDone.WaitOne();
        //}

        //public TCPState Receive()
        //{
        //    readOffset = 0;
        //    return Receive(client);
        //}

        //public override void Release()
        //{
        //    client.Shutdown(SocketShutdown.Both);
        //    client.Close();
        //}

        //private TCPState Receive(Socket client)
        //{

        //        // Create the tcpState object.  
        //        var tcpState = new TCPState();
        //        tcpState.workSocket = client;

        //        // Begin receiving the data from the remote device.  
        //        client.BeginReceive(tcpState.buffer, readOffset, tcpState.BufferSize, 0,
        //            new AsyncCallback(ReceiveCallback), tcpState);

        //        receiveDone.WaitOne();
        //        return tcpState;
        //}

        //private  void ReceiveCallback(IAsyncResult ar)
        //{
        //    //try
        //    //{
        //        // Retrieve the tcpState object and the client socket
        //        // from the asynchronous tcpState object.  
        //    var tcpState = (TCPState)ar.AsyncState;
        //    Socket client = tcpState.workSocket;

        //    // Read data from the remote device.  
        //    netBytes = client.EndReceive(ar);
        //    Console.WriteLine("R " + client.RemoteEndPoint + "-(" + netBytes + ")->" + client.LocalEndPoint);
        //    receiveDone.Set();

        //    //if (netBytes > 0)
        //    //{
        //    //    readOffset += netBytes;
        //    //    // Get the rest of the data.

        //    //    client.BeginReceive(tcpState.buffer, readOffset, tcpState.BufferSize-readOffset, 0,
        //    //            new AsyncCallback(ReceiveCallback), tcpState);


        //    //}
        //    //else
        //    //{
        //    //    // Signal that all bytes have been received.  
        //    //    receiveDone.Set();
        //    //}
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine(e.ToString());
        //    //}
        //}

        //public void Send(byte[] data)
        //{
        //    // Convert the string data to byte data using ASCII encoding.  
        //    byte[] byteData = data;

        //    // Begin sending the data to the remote device.  
        //    client.BeginSend(byteData, 0, byteData.Length, 0,
        //        new AsyncCallback(SendCallback), client);
        //    sendDone.WaitOne();
        //}

        //private void SendCallback(IAsyncResult ar)
        //{
        //    //try
        //    //{
        //        // Retrieve the socket from the tcpState object.  
        //        Socket client = (Socket)ar.AsyncState;

        //    // Complete sending the data to the remote device.  
        //        netBytes = client.EndSend(ar);
        //    Console.WriteLine("S " + client.LocalEndPoint + "-(" + netBytes + ")->" + client.RemoteEndPoint);

        //    // Signal that all bytes have been sent.  
        //    sendDone.Set();
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine(e.ToString());
        //    //}
        //}
    }
}
