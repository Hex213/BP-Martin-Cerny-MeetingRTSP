using System.Net.Sockets;
using System.Text;

namespace LibNet.Utils
{
    public class TCPState
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public readonly int BufferSize;
        // Receive buffer.  
        public byte[] buffer;
        //Total readed/sended bytes
        public int totalRead;

        public TCPState(int bufferSize = (4 * 1024))
        {
            BufferSize = bufferSize;
            buffer = new byte[BufferSize];
        }

        public Socket WorkSocket => workSocket;

        public int BufferSize1 => BufferSize;

        public byte[] Buffer => buffer;

        public int TotalRead => totalRead;
    }
}
