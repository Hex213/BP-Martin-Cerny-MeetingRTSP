using System;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using LibRtspClientSharp.Hex;
using LibRtspClientSharp.Hex.Exceptions;

namespace CSNamedPipe
{
    public class NamedPipeServer
    {
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

        private Func<object, object> recvFunc;

        public NamedPipeServer(string PName,int Mode, Func<object, object> readFunc)
        {
            if (readFunc == null && Mode != 1) throw new ArgumentNullException(nameof(readFunc));
            if (string.IsNullOrEmpty(PName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(PName));
            if (string.IsNullOrWhiteSpace(PName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(PName));
            if (Mode < 0) throw new ArgumentOutOfRangeException(nameof(Mode));
            pipeName = PName ?? throw new ArgumentNullException(nameof(PName));
            ClientType = Mode;//0 Reading Pipe, 1 Writing Pipe
            recvFunc = readFunc;
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

                clientHandle = CreateNamedPipe(this.pipeName, DUPLEX | FILE_FLAG_OVERLAPPED, 0, 255, BUFFER_SIZE, BUFFER_SIZE, 0, IntPtr.Zero);

                //could not create named pipe
                if (clientHandle.IsInvalid)
                    return;

                int success = ConnectNamedPipe(clientHandle, IntPtr.Zero);

                //could not connect client
                if (success == 0)
                    return;

                clientse = new Client();
                clientse.handle = clientHandle;
                clientse.stream = new FileStream(clientse.handle, FileAccess.ReadWrite, BUFFER_SIZE, true);

                if (ClientType == 0)
                {
                    Thread readThread = new Thread(new ThreadStart(Read));
                    readThread.Start();
                }
            }
        }
        private void Read()
        {
            byte[] buffer = null;
            int bytesRead = 0;

            while (true)
            {
                bytesRead = 0;
                try
                {
                    buffer = new byte[BUFFER_SIZE];
                    bytesRead = clientse.stream.Read(buffer, 0, BUFFER_SIZE);
                }
                catch
                {
                    //clean up resources
                    clientse.stream.Close();
                    clientse.handle.Close();
                    throw new PipeException("Error with reading", 1);
                }

                //client has disconnected
                if (bytesRead <= 0)
                {
                    //clean up resources
                    clientse.stream.Close();
                    clientse.handle.Close();
                    throw new PipeException("Client disconnected", 2);
                }

                _ = recvFunc(buffer);
            }
        }

        public void SendMessage(byte[] data, Client client)
        {
            if (client.stream.CanWrite)
            {
                client.stream.Write(data, 0, data.Length);
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
