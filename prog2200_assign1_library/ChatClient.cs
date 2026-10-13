using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace prog2200_assign1_library
{
    /// <summary>
    /// A multi-threaded TCP chat client.
    ///
    /// Connecting is asynchronous (async/await), and every connection gets two dedicated threads:
    ///   - a receive thread that reads bytes from the server and raises <see cref="MessageReceived"/>;
    ///   - a send thread that takes messages from a thread-safe queue and raises <see cref="MessageSent"/>.
    ///
    /// IMPORTANT: events are raised on those background threads, never on the UI thread.
    /// A Windows Forms subscriber must use Invoke / BeginInvoke before touching any controls.
    /// </summary>
    public sealed class ChatClient : IDisposable
    {
        /// <summary>How long to wait for the server to accept the connection.</summary>
        private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);

        // ncat (and most line based servers) treat CR LF as "end of message".
        private const string LineEnding = "\r\n";

        // UTF-8 is identical to ASCII for plain English text, but also keeps accented characters intact.
        private static readonly UTF8Encoding Utf8 = new UTF8Encoding(false);

        // Guards the three fields below.
        private readonly object _sync = new object();

        private StateObj _connection;   // null when not connected
        private bool _isConnecting;
        private bool _isDisposed;

        /// <summary>Raised on the receive thread for every line the server sends.</summary>
        public event ChatMessageEventHandler MessageReceived;

        /// <summary>Raised on the send thread once a message has been written to the server.</summary>
        public event ChatMessageEventHandler MessageSent;

        /// <summary>
        /// Raised (on any thread) when the client connects, disconnects or starts connecting.
        /// A lost connection is reported here, with the reason, rather than through <see cref="ErrorOccurred"/>.
        /// </summary>
        public event ConnectionChangedEventHandler ConnectionChanged;

        /// <summary>Raised (on any thread) when connecting fails or a message is sent while not connected.</summary>
        public event ChatErrorEventHandler ErrorOccurred;

        /// <summary>True while connected to a server. Safe to read from any thread.</summary>
        public bool IsConnected
        {
            get
            {
                lock (_sync)
                {
                    return _connection != null;
                }
            }
        }

        /// <summary>
        /// Connects to a chat server without blocking the caller, then starts the
        /// send and receive threads.
        /// </summary>
        /// <returns>True if the connection was made.</returns>
        public async Task<bool> ConnectAsync(string host, int port)
        {
            lock (_sync)
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (_isConnecting || _connection != null)
                {
                    return false;
                }

                _isConnecting = true;
            }

            string address = host + ":" + port;
            OnConnectionChanged(ConnectionStatus.Connecting, "Connecting to " + address + "...");

            Socket socket = null;

            try
            {
                socket = await OpenSocketAsync(host, port).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is SocketException || ex is TimeoutException)
            {
                lock (_sync)
                {
                    _isConnecting = false;
                }

                OnErrorOccurred("Could not connect to " + address + " - " + DescribeConnectError(ex) + ".", ex);
                OnConnectionChanged(ConnectionStatus.Disconnected, "Not connected");
                return false;
            }

            var state = new StateObj(socket, address);

            lock (_sync)
            {
                _isConnecting = false;

                // Dispose() may have run while we were waiting for the server.
                if (_isDisposed)
                {
                    socket.Close();
                    return false;
                }

                _connection = state;
            }

            OnConnectionChanged(ConnectionStatus.Connected, "Connected to " + address);

            StartBackgroundThread(() => ReceiveLoop(state), "Chat receive thread");
            StartBackgroundThread(() => SendLoop(state), "Chat send thread");

            return true;
        }

        /// <summary>
        /// Queues a message for the send thread. Returns immediately, so it can be called
        /// as often and as quickly as the user likes; messages are sent in order.
        /// </summary>
        /// <returns>True if the message was queued; false if the client is not connected.</returns>
        public bool Send(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (_sync)
            {
                // Added under the lock, so a message can never be queued after the connection closes.
                if (_connection != null)
                {
                    _connection.Outbox.Add(message);
                    return true;
                }
            }

            OnErrorOccurred("Message not sent - you are not connected to a server.", null);
            return false;
        }

        /// <summary>
        /// Closes the connection to the server. Safe to call at any time, from any thread,
        /// any number of times.
        /// </summary>
        public void Disconnect()
        {
            StateObj state;

            lock (_sync)
            {
                state = _connection;
            }

            if (state != null)
            {
                Close(state, "Disconnected");
            }
        }

        /// <summary>
        /// Disconnects and stops the client for good.
        /// </summary>
        public void Dispose()
        {
            lock (_sync)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            Disconnect();
        }

        /// <summary>
        /// Looks up the host's IP address and connects a socket to it, giving up after
        /// <see cref="ConnectTimeout"/>.
        /// </summary>
        private static async Task<Socket> OpenSocketAsync(string host, int port)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);

            // Prefer IPv4, which is what ncat listens on by default.
            IPAddress address = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                ?? addresses.First();

            var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Task connect = Task.Factory.FromAsync<IPAddress, int>(
                    socket.BeginConnect, socket.EndConnect, address, port, null);

                if (await Task.WhenAny(connect, Task.Delay(ConnectTimeout)).ConfigureAwait(false) != connect)
                {
                    // Observe the abandoned attempt so its exception is not reported as unhandled.
                    _ = connect.ContinueWith(t => t.Exception, CancellationToken.None,
                        TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
                    throw new TimeoutException();
                }

                // Rethrows the SocketException if the server refused the connection.
                await connect.ConfigureAwait(false);
                return socket;
            }
            catch
            {
                socket.Close();
                throw;
            }
        }

        /// <summary>
        /// Receive thread: reads bytes from the server, turns them back into text and raises
        /// <see cref="MessageReceived"/> once per line.
        /// </summary>
        private void ReceiveLoop(StateObj state)
        {
            // A Decoder remembers partial multi-byte characters that are split across two reads.
            Decoder decoder = Utf8.GetDecoder();
            char[] chars = new char[Utf8.GetMaxCharCount(StateObj.BufferSize)];

            // TCP may split one message across several reads (or join several into one),
            // so characters are collected here until a line ending arrives.
            var line = new StringBuilder();

            try
            {
                while (true)
                {
                    // Blocks this thread (only) until data arrives.
                    int byteCount = state.Socket.Receive(state.Buffer);

                    if (byteCount == 0)
                    {
                        // A zero-byte read means the connection was closed - by the server, unless
                        // Disconnect() got there first. Show any final text sent without a line ending.
                        if (!state.IsClosed)
                        {
                            if (line.Length > 0)
                            {
                                OnMessageReceived(line.ToString());
                            }

                            Close(state, "The server closed the connection");
                        }

                        return;
                    }

                    int charCount = decoder.GetChars(state.Buffer, 0, byteCount, chars, 0);

                    for (int i = 0; i < charCount; i++)
                    {
                        if (chars[i] != '\n')
                        {
                            line.Append(chars[i]);
                            continue;
                        }

                        // End of a line - drop the '\r' of a "\r\n" ending.
                        if (line.Length > 0 && line[line.Length - 1] == '\r')
                        {
                            line.Length--;
                        }

                        OnMessageReceived(line.ToString());
                        line.Clear();
                    }
                }
            }
            catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException)
            {
                // Closing the socket in Disconnect() also ends a blocked Receive - nothing to report then.
                if (!state.IsClosed)
                {
                    Close(state, "The connection to the server was lost (" + ex.Message + ")");
                }
            }
        }

        /// <summary>
        /// Send thread: waits for queued messages and writes them to the server one at a time.
        /// </summary>
        private void SendLoop(StateObj state)
        {
            try
            {
                // Sleeps until Send() queues a message; ends when Close() completes the queue.
                foreach (string message in state.Outbox.GetConsumingEnumerable())
                {
                    byte[] data = Utf8.GetBytes(message + LineEnding);
                    state.Socket.Send(data);

                    OnMessageSent(message);
                }
            }
            catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException)
            {
                if (!state.IsClosed)
                {
                    Close(state, "The connection to the server was lost (" + ex.Message + ")");
                }
            }
        }

        /// <summary>
        /// Closes a connection exactly once, no matter how many threads call it
        /// (user disconnect, server disconnect and send failure can all happen together).
        /// </summary>
        private void Close(StateObj state, string reason)
        {
            lock (_sync)
            {
                if (state.IsClosed)
                {
                    return;
                }

                state.IsClosed = true;
                state.Outbox.CompleteAdding();

                if (_connection == state)
                {
                    _connection = null;
                }
            }

            // Shutdown tells the server we are leaving; Close releases the socket and
            // wakes the receive thread if it is waiting for data.
            try
            {
                state.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
                // The server already went away.
            }

            state.Socket.Close();

            OnConnectionChanged(ConnectionStatus.Disconnected, reason);
        }

        private static void StartBackgroundThread(ThreadStart work, string name)
        {
            // Background threads never keep the program running after the window closes.
            var thread = new Thread(work)
            {
                IsBackground = true,
                Name = name
            };

            thread.Start();
        }

        private static string DescribeConnectError(Exception ex)
        {
            var socketException = ex as SocketException;

            if (socketException == null)
            {
                return "the server did not respond in time";
            }

            switch (socketException.SocketErrorCode)
            {
                case SocketError.ConnectionRefused:
                    return "no server is listening on that port";
                case SocketError.HostNotFound:
                    return "that host could not be found";
                default:
                    return socketException.Message;
            }
        }

        private void OnMessageReceived(string text)
        {
            MessageReceived?.Invoke(this, new ChatMessageEventArgs(text));
        }

        private void OnMessageSent(string text)
        {
            MessageSent?.Invoke(this, new ChatMessageEventArgs(text));
        }

        private void OnConnectionChanged(ConnectionStatus status, string reason)
        {
            ConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(status, reason));
        }

        private void OnErrorOccurred(string message, Exception exception)
        {
            ErrorOccurred?.Invoke(this, new ChatErrorEventArgs(message, exception));
        }
    }
}
