using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace prog2200_assign1_library;

/// <summary>
/// An asynchronous TCP chat client.
///
/// All networking happens on background threads:
///   - a receive task reads from the server and raises <see cref="MessageReceived"/>;
///   - a send task drains a thread-safe queue of outgoing messages and raises <see cref="MessageSent"/>.
///
/// IMPORTANT: every event is raised on a background (thread pool) thread, never on the UI thread.
/// A Windows Forms subscriber must use Control.Invoke / BeginInvoke before touching any controls.
/// </summary>
public sealed class ChatClient : IDisposable
{
    /// <summary>Size of the buffer used for each read from the network stream.</summary>
    public const int BufferSize = 1024;

    /// <summary>How long to wait for the server to accept the connection.</summary>
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);

    // ncat (and most line based servers) treat CR LF as "end of message".
    private const string LineTerminator = "\r\n";

    // UTF-8 is identical to ASCII for plain English text, but also keeps accented characters intact.
    private static readonly Encoding TextEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    // Guards every field below. Held only for short, non-blocking sections.
    private readonly object _stateLock = new();

    private ConnectionState _state = ConnectionState.Disconnected;
    private TcpClient? _tcpClient;
    private CancellationTokenSource? _connectionCts;
    private Channel<string>? _outgoingMessages;
    private bool _disposed;

    /// <summary>Raised (on a background thread) for every message received from the server.</summary>
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>Raised (on a background thread) after a message has been written to the server.</summary>
    public event EventHandler<MessageSentEventArgs>? MessageSent;

    /// <summary>Raised (on any thread) whenever the connection state changes.</summary>
    public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;

    /// <summary>Raised (on any thread) when a networking error occurs.</summary>
    public event EventHandler<ChatErrorEventArgs>? ErrorOccurred;

    /// <summary>The current connection state. Safe to read from any thread.</summary>
    public ConnectionState State
    {
        get
        {
            lock (_stateLock)
            {
                return _state;
            }
        }
    }

    /// <summary>True when the client is connected to a server.</summary>
    public bool IsConnected => State == ConnectionState.Connected;

    /// <summary>
    /// Connects to a chat server without blocking the caller, then starts the background
    /// send and receive tasks.
    /// </summary>
    /// <returns>True if the connection was established.</returns>
    public async Task<bool> ConnectAsync(string host, int port)
    {
        lock (_stateLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_state != ConnectionState.Disconnected)
            {
                return false;
            }

            _state = ConnectionState.Connecting;
        }

        OnConnectionStatusChanged(ConnectionState.Connecting, $"Connecting to {host}:{port}...");

        var tcpClient = new TcpClient();

        try
        {
            using var timeout = new CancellationTokenSource(ConnectTimeout);
            await tcpClient.ConnectAsync(host, port, timeout.Token).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is SocketException or OperationCanceledException)
        {
            tcpClient.Dispose();

            lock (_stateLock)
            {
                _state = ConnectionState.Disconnected;
            }

            string reason = ex is OperationCanceledException
                ? "the server did not respond in time"
                : ((SocketException)ex).SocketErrorCode == SocketError.ConnectionRefused
                    ? "no server is listening on that port"
                    : ex.Message;

            OnErrorOccurred($"Could not connect to {host}:{port} - {reason}.", ex);
            OnConnectionStatusChanged(ConnectionState.Disconnected, "Not connected");
            return false;
        }

        var connectionCts = new CancellationTokenSource();
        CancellationToken token = connectionCts.Token;
        var outgoingMessages = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions { SingleReader = true });
        NetworkStream stream = tcpClient.GetStream();

        lock (_stateLock)
        {
            // Dispose() may have run while we were waiting for the server.
            if (_disposed)
            {
                _state = ConnectionState.Disconnected;
                tcpClient.Dispose();
                connectionCts.Dispose();
                return false;
            }

            _tcpClient = tcpClient;
            _connectionCts = connectionCts;
            _outgoingMessages = outgoingMessages;
            _state = ConnectionState.Connected;
        }

        OnConnectionStatusChanged(ConnectionState.Connected, $"Connected to {host}:{port}");

        // Run the send and receive loops on thread pool threads so neither can block the caller.
        _ = Task.Run(() => ReceiveLoopAsync(stream, token));
        _ = Task.Run(() => SendLoopAsync(stream, outgoingMessages.Reader, token));

        return true;
    }

    /// <summary>
    /// Queues a message to be sent to the server. Returns immediately, so it can be
    /// called as often and as quickly as the user likes; messages are sent in order.
    /// </summary>
    /// <returns>True if the message was queued; false if the client is not connected.</returns>
    public bool Send(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Channel<string>? outgoingMessages;

        lock (_stateLock)
        {
            outgoingMessages = _state == ConnectionState.Connected ? _outgoingMessages : null;
        }

        if (outgoingMessages is null || !outgoingMessages.Writer.TryWrite(message))
        {
            OnErrorOccurred("Message not sent - you are not connected to a server.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Closes the connection to the server. Safe to call at any time, from any thread,
    /// any number of times.
    /// </summary>
    public void Disconnect()
    {
        CloseConnection("Disconnected");
    }

    /// <summary>
    /// Disconnects and releases all resources.
    /// </summary>
    public void Dispose()
    {
        lock (_stateLock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        CloseConnection("Disconnected");
    }

    /// <summary>
    /// Background loop: reads bytes from the server, turns them back into text and raises
    /// <see cref="MessageReceived"/> once per line.
    /// </summary>
    private async Task ReceiveLoopAsync(NetworkStream stream, CancellationToken token)
    {
        byte[] buffer = new byte[BufferSize];

        // A Decoder remembers partial multi-byte characters that are split across two reads.
        Decoder decoder = TextEncoding.GetDecoder();
        char[] chars = new char[TextEncoding.GetMaxCharCount(BufferSize)];
        var pending = new StringBuilder();

        try
        {
            while (!token.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, token).ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    // A zero-byte read means the server closed its end of the connection.
                    // Show any final text the server sent without a line ending.
                    if (pending.Length > 0)
                    {
                        OnMessageReceived(pending.ToString());
                    }

                    CloseConnection("The server closed the connection");
                    return;
                }

                // TCP may split one message across several reads (or join several into one),
                // so text is buffered and only complete lines are raised as messages.
                int charCount = decoder.GetChars(buffer, 0, bytesRead, chars, 0);
                pending.Append(chars, 0, charCount);

                RaiseCompleteLines(pending);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Disconnect() was called - nothing to report.
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
            if (!token.IsCancellationRequested)
            {
                OnErrorOccurred("The connection to the server was lost.", ex);
                CloseConnection("Connection lost");
            }
        }
    }

    /// <summary>
    /// Background loop: waits for queued messages and writes them to the server one at a time.
    /// </summary>
    private async Task SendLoopAsync(NetworkStream stream, ChannelReader<string> reader, CancellationToken token)
    {
        try
        {
            await foreach (string message in reader.ReadAllAsync(token).ConfigureAwait(false))
            {
                byte[] data = TextEncoding.GetBytes(message + LineTerminator);
                await stream.WriteAsync(data, token).ConfigureAwait(false);

                OnMessageSent(message);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Disconnect() was called - nothing to report.
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
            if (!token.IsCancellationRequested)
            {
                OnErrorOccurred("A message could not be sent - the connection was lost.", ex);
                CloseConnection("Connection lost");
            }
        }
    }

    /// <summary>
    /// Removes every complete line from <paramref name="pending"/> and raises an event for each.
    /// Handles both "\n" and "\r\n" line endings.
    /// </summary>
    private void RaiseCompleteLines(StringBuilder pending)
    {
        string text = pending.ToString();
        int lineStart = 0;
        int newLine;

        while ((newLine = text.IndexOf('\n', lineStart)) >= 0)
        {
            int lineLength = newLine - lineStart;

            if (lineLength > 0 && text[newLine - 1] == '\r')
            {
                lineLength--;
            }

            OnMessageReceived(text.Substring(lineStart, lineLength));
            lineStart = newLine + 1;
        }

        pending.Remove(0, lineStart);
    }

    /// <summary>
    /// Tears the connection down exactly once, no matter how many threads call it
    /// (user disconnect, server disconnect and send failure can all happen together).
    /// </summary>
    private void CloseConnection(string reason)
    {
        TcpClient? tcpClient;
        CancellationTokenSource? connectionCts;
        Channel<string>? outgoingMessages;

        lock (_stateLock)
        {
            if (_state != ConnectionState.Connected)
            {
                return;
            }

            tcpClient = _tcpClient;
            connectionCts = _connectionCts;
            outgoingMessages = _outgoingMessages;

            _tcpClient = null;
            _connectionCts = null;
            _outgoingMessages = null;
            _state = ConnectionState.Disconnected;
        }

        // Stop both background loops, then close the socket so the server sees a clean disconnect.
        outgoingMessages?.Writer.TryComplete();
        connectionCts?.Cancel();

        try
        {
            tcpClient?.Client.Shutdown(SocketShutdown.Both);
        }
        catch (Exception ex) when (ex is SocketException or ObjectDisposedException)
        {
            // The socket is already closed (e.g. the server went away first).
        }

        tcpClient?.Dispose();
        connectionCts?.Dispose();

        OnConnectionStatusChanged(ConnectionState.Disconnected, reason);
    }

    private void OnMessageReceived(string message) =>
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));

    private void OnMessageSent(string message) =>
        MessageSent?.Invoke(this, new MessageSentEventArgs(message));

    private void OnConnectionStatusChanged(ConnectionState state, string description) =>
        ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(state, description));

    private void OnErrorOccurred(string message, Exception? exception = null) =>
        ErrorOccurred?.Invoke(this, new ChatErrorEventArgs(message, exception));
}
