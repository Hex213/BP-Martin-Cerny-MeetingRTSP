using System.Net.Sockets;
using System.Text;

namespace LibNet.Utils
{
    public class State
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public readonly int BufferSize;
        // Receive buffer.  
        public byte[] buffer;
        // Received data string.  
        public StringBuilder sb;

        public State(int bufferSize = (4 * 1024))
        {
            BufferSize = bufferSize;
            buffer = new byte[BufferSize];
            sb = new StringBuilder();
        }

        public Socket WorkSocket => workSocket;

        public int BufferSize1 => BufferSize;

        public byte[] Buffer => buffer;

        public StringBuilder Sb => sb;
    }
}
