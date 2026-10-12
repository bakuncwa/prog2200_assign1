using System;

namespace prog2200_assign1_library
{
    /// <summary>The stages of a connection to the chat server.</summary>
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected
    }

    // Event handler declarations - the signatures a subscriber's methods must match.
    public delegate void ChatMessageEventHandler(object sender, ChatMessageEventArgs e);
    public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs e);
    public delegate void ChatErrorEventHandler(object sender, ChatErrorEventArgs e);

    /// <summary>
    /// Event data for a chat message, used for both received and sent messages.
    /// </summary>
    public class ChatMessageEventArgs : EventArgs
    {
        public ChatMessageEventArgs(string text)
        {
            Text = text;
            Time = DateTime.Now;
        }

        /// <summary>The message exactly as it was typed (line ending removed).</summary>
        public string Text { get; }

        /// <summary>When the message was received or sent.</summary>
        public DateTime Time { get; }
    }

    /// <summary>
    /// Event data for a change in the connection.
    /// </summary>
    public class ConnectionChangedEventArgs : EventArgs
    {
        public ConnectionChangedEventArgs(ConnectionStatus status, string reason)
        {
            Status = status;
            Reason = reason;
        }

        /// <summary>The new connection status.</summary>
        public ConnectionStatus Status { get; }

        /// <summary>A human readable explanation, e.g. "The server closed the connection".</summary>
        public string Reason { get; }
    }

    /// <summary>
    /// Event data for a networking error the client recovered from.
    /// </summary>
    public class ChatErrorEventArgs : EventArgs
    {
        public ChatErrorEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        /// <summary>A human readable description of the error.</summary>
        public string Message { get; }

        /// <summary>The underlying exception, or null.</summary>
        public Exception Exception { get; }
    }
}
