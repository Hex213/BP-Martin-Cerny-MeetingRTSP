using System;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace CSNamedPipe
{
    public class NamedPipeServer
    {
        private Queue<byte[]> _inputFromPipe = new Queue<byte[]>();
        private bool _isStarted = false;

        public bool IsStarted => _isStarted;

        public Queue<byte[]> InputFromPipe => _inputFromPipe;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateNamedPipe(
           String pipeName,
           uint dwOpenMode,
           uint dwPipeMode,
           uint nMaxInstances,
           uint nOutBufferSize,
           uint nInBufferSize,
           uint nDefaultTimeOut,
           IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ConnectNamedPipe(
           SafeFileHandle hNamedPipe,
           IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int DisconnectNamedPipe(
           SafeFileHandle hNamedPipe);

        public const uint DUPLEX = (0x00000003);
        public const uint FILE_FLAG_OVERLAPPED = (0x40000000);

        public class Client
        {
            public SafeFileHandle handle;
            public FileStream stream;
        }

        public const int BUFFER_SIZE = 100;
        public Client clientse =null;

        public string pipeName;
        Thread listenThread;
        SafeFileHandle clientHandle;
        public int ClientType;

        public NamedPipeServer(string PName,int Mode)
        {
            pipeName = PName;
            ClientType = Mode;//0 Reading Pipe, 1 Writing Pipe
        }
       
        public void Start()
        {
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }
        private void ListenForClients()
        {
            while (true)
            {
                clientHandle = CreateNamedPipe(this.pipeName,DUPLEX | FILE_FLAG_OVERLAPPED,0,255,BUFFER_SIZE,BUFFER_SIZE,0,IntPtr.Zero);

                //could not create named pipe
                if (clientHandle.IsInvalid)
                {
                    _isStarted = false;
                    return;
                }

                int success = ConnectNamedPipe(clientHandle, IntPtr.Zero);

                //could not connect client
                if (success == 0)
                {
                    _isStarted = false;
                    return;
                }

                clientse = new Client();
                clientse.handle = clientHandle;
                clientse.stream = new FileStream(clientse.handle, FileAccess.ReadWrite, BUFFER_SIZE, true);

                if (ClientType == 0)
                {
                    Thread readThread = new Thread(new ThreadStart(Read));
                    readThread.Start();
                    _isStarted = true;
                }
            }
        }
        private void Read()
        {
            //Client client = (Client)clientObj;
            //clientse.stream = new FileStream(clientse.handle, FileAccess.ReadWrite, BUFFER_SIZE, true);
            byte[] buffer = null;
            ASCIIEncoding encoder = new ASCIIEncoding();

            while (true)
            {
                
                int bytesRead = 0;

                try
                {
                    buffer = new byte[BUFFER_SIZE];
                    bytesRead = clientse.stream.Read(buffer, 0, BUFFER_SIZE);
                }
                catch
                {
                    //read error has occurred
                    break;
                }

                //client has disconnected
                if (bytesRead == 0)
                    break;

                //fire message received event
                //if (this.MessageReceived != null)
                //    this.MessageReceived(clientse, encoder.GetString(buffer, 0, bytesRead));

                int ReadLength = 0;
                for (int i = 0; i < BUFFER_SIZE; i++)
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

                    //Console.WriteLine("C# App: Received " + ReadLength +" Bytes: "+ encoder.GetString(Rc, 0, ReadLength));
                    _inputFromPipe.Enqueue(Rc);

                    buffer.Initialize();
                }
            }

            //clean up resources
            clientse.stream.Close();
            clientse.handle.Close();
        }
        public void SendMessage(string message, Client client)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] messageBuffer = encoder.GetBytes(message);

            if (client.stream.CanWrite)
            {
                client.stream.Write(messageBuffer, 0, messageBuffer.Length);
                client.stream.Flush();
            }
        }
        public void SendMessage(byte[] message, Client client)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] messageBuffer = message;

            if (client.stream.CanWrite)
            {
                client.stream.Write(messageBuffer, 0, messageBuffer.Length);
                client.stream.Flush();
            }
        }

        public void StopServer()
        {
            //clean up resources

            DisconnectNamedPipe(this.clientHandle);
            

            this.listenThread.Abort();
        }
    }
}

/*
 NamedPipeServer PServer1 = new NamedPipeServer(@"\\.\pipe\myNamedPipe1",0);
            NamedPipeServer PServer2 = new NamedPipeServer(@"\\.\pipe\myNamedPipe2",1);

            PServer1.Start();
            PServer2.Start();

            string Ms="Start";
            do
            {
                Console.WriteLine("Enter the message");
                Ms = Console.ReadLine();
                PServer2.SendMessage(Ms, PServer2.clientse);
            } while (Ms != "quit");

            PServer1.StopServer();
            PServer2.StopServer();
 */