using System;
using System.Net.Sockets;
using System.Text;

namespace LibNet.Utils
{
    public class TCPState
    {
        // Client socket.  
        private Socket workSocket = null;
        // Client stream
        private NetworkStream stream = null;
        // Size of receive buffer.  
        public readonly int bufferSize;
        // Receive buffer.  
        public byte[] buffer;
        //Total readed/sended bytes
        public int totalRead;

        public TCPState(int bufferSize = (4 * 1024))
        {
            this.bufferSize = bufferSize;
            buffer = new byte[this.bufferSize];
        }

        public Socket WorkSocket => workSocket;

        public NetworkStream Stream => stream;

        public void setSocket(TcpClient client)
        {
            if (client != null)
            {
                stream = client.GetStream();
                workSocket = client.Client;
            }
        }

        public void setSocket(Socket socket)
        {
            if (socket != null)
            {
                stream = new NetworkStream(socket);
                workSocket = socket;
            }
        }

        public int BufferSize => bufferSize;

        public byte[] Buffer => buffer;

        public int TotalRead => totalRead;
    }
}
