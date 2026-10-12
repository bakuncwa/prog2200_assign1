using System;

namespace prog2200_assign1_library
{
    /// <summary>
    /// The possible states of a <see cref="ChatClient"/> connection.
    /// </summary>
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected
    }

    /// <summary>
    /// Event data for a message that arrived from the server.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
            ReceivedAt = DateTime.Now;
        }

        /// <summary>The message text exactly as the server sent it (line ending removed).</summary>
        public string Message { get; }

        /// <summary>When the message was received.</summary>
        public DateTime ReceivedAt { get; }
    }

    /// <summary>
    /// Event data for a message that has been written to the server.
    /// </summary>
    public class MessageSentEventArgs : EventArgs
    {
        public MessageSentEventArgs(string message)
        {
            Message = message;
            SentAt = DateTime.Now;
        }

        /// <summary>The message text that was sent.</summary>
        public string Message { get; }

        /// <summary>When the message finished sending.</summary>
        public DateTime SentAt { get; }
    }

    /// <summary>
    /// Event data for a change in the connection state.
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatusChangedEventArgs(ConnectionState state, string description)
        {
            State = state;
            Description = description;
        }

        /// <summary>The new connection state.</summary>
        public ConnectionState State { get; }

        /// <summary>A human readable explanation of the change (e.g. "Server closed the connection").</summary>
        public string Description { get; }
    }

    /// <summary>
    /// Event data for a networking error the client recovered from.
    /// </summary>
    public class ChatErrorEventArgs : EventArgs
    {
        public ChatErrorEventArgs(string message, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }

        /// <summary>A human readable description of the error.</summary>
        public string Message { get; }

        /// <summary>The underlying exception, if there was one.</summary>
        public Exception Exception { get; }
    }
}
