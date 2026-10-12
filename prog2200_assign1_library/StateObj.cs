using System.Collections.Concurrent;
using System.Net.Sockets;

namespace prog2200_assign1_library
{
    /// <summary>
    /// Everything that belongs to one connection to the server.
    /// A new StateObj is made for every connection, so the threads of an old connection
    /// can never read from or write to a newer one.
    /// </summary>
    public sealed class StateObj
    {
        /// <summary>Number of bytes read from the socket at a time.</summary>
        public const int BufferSize = 1024;

        // volatile: written under the client's lock, but read without it by the send and receive threads.
        private volatile bool _isClosed;

        public StateObj(Socket socket, string address)
        {
            Socket = socket;
            Address = address;
        }

        /// <summary>The connected socket.</summary>
        public Socket Socket { get; }

        /// <summary>Receive buffer - raw bytes before they are turned back into text.</summary>
        public byte[] Buffer { get; } = new byte[BufferSize];

        /// <summary>The server's address as "host:port".</summary>
        public string Address { get; }

        /// <summary>Thread-safe queue of messages waiting for the send thread.</summary>
        public BlockingCollection<string> Outbox { get; } = new BlockingCollection<string>();

        /// <summary>True once the connection has been closed (by either side).</summary>
        public bool IsClosed
        {
            get { return _isClosed; }
            set { _isClosed = value; }
        }
    }
}
