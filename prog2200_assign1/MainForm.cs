using System;
using System.Windows.Forms;
using prog2200_assign1_library;

namespace prog2200_assign1
{
    /// <summary>
    /// The Network Game window. All chat networking lives in prog2200_assign1_library
    /// (<see cref="ChatClient"/> and <see cref="ChatServerLauncher"/>); this form only shows what they report.
    /// </summary>
    public partial class MainForm : Form
    {
        // Client and server run on the same machine.
        private const string ServerHost = "127.0.0.1";
        private const int ServerPort = 1234;

        // Marks the lines this client sent; lines from the server are shown exactly as the server typed them.
        private const string SentTag = ">>";

        private const string ErrorCaption = "Error";

        private readonly ChatClient _chatClient = new ChatClient();
        private readonly ChatServerLauncher _serverLauncher = new ChatServerLauncher();

        private bool _isConnecting;
        private bool _isUserDisconnecting;
        private bool _wasConnected;

        public MainForm()
        {
            InitializeComponent();

            _chatClient.MessageReceived += new ChatMessageEventHandler(ChatClient_MessageReceived);
            _chatClient.MessageSent += new ChatMessageEventHandler(ChatClient_MessageSent);
            _chatClient.ConnectionChanged += new ConnectionChangedEventHandler(ChatClient_ConnectionChanged);
            _chatClient.ErrorOccurred += new ChatErrorEventHandler(Chat_ErrorOccurred);
            _serverLauncher.ErrorOccurred += new ChatErrorEventHandler(Chat_ErrorOccurred);
        }

        /// <summary>
        /// Stops listening to the chat client and closes the connection before the window goes away.
        /// The server window is left running, like a real server.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _chatClient.MessageReceived -= ChatClient_MessageReceived;
            _chatClient.MessageSent -= ChatClient_MessageSent;
            _chatClient.ConnectionChanged -= ChatClient_ConnectionChanged;
            _chatClient.ErrorOccurred -= Chat_ErrorOccurred;
            _serverLauncher.ErrorOccurred -= Chat_ErrorOccurred;

            _chatClient.Dispose();

            base.OnFormClosing(e);
        }

        #region Menu and button handlers

        private async void connectMenuItem_Click(object sender, EventArgs e)
        {
            if (_isConnecting || _chatClient.IsConnected)
            {
                return;
            }

            _isConnecting = true;

            try
            {
                // Opens the server's command prompt window if no server is running yet, then connects.
                // Both steps run in the background; the window stays responsive while we wait.
                if (await _serverLauncher.EnsureServerRunningAsync(ServerPort))
                {
                    await _chatClient.ConnectAsync(ServerHost, ServerPort);
                }
            }
            finally
            {
                _isConnecting = false;
            }
        }

        private void disconnectMenuItem_Click(object sender, EventArgs e)
        {
            _isUserDisconnecting = true;

            try
            {
                _chatClient.Disconnect();
            }
            finally
            {
                _isUserDisconnecting = false;
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (messageTextBox.TextLength == 0)
            {
                return;
            }

            // Send() only queues the message for the send thread, so this returns straight away.
            _chatClient.Send(messageTextBox.Text);

            // Clear the textbox after sending
            messageTextBox.Clear();
            messageTextBox.Focus();
        }

        #endregion

        #region Chat event handlers

        // These handlers are first called on the chat client's background threads. Each one checks
        // InvokeRequired and, if needed, calls itself again on the UI thread before touching controls.

        private void ChatClient_MessageReceived(object sender, ChatMessageEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ChatMessageEventHandler(ChatClient_MessageReceived), sender, e);
                return;
            }

            AddConversationLine(e.Text);
        }

        private void ChatClient_MessageSent(object sender, ChatMessageEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ChatMessageEventHandler(ChatClient_MessageSent), sender, e);
                return;
            }

            AddConversationLine(SentTag + e.Text);
        }

        private void ChatClient_ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ConnectionChangedEventHandler(ChatClient_ConnectionChanged), sender, e);
                return;
            }

            bool isConnected = e.Status == ConnectionStatus.Connected;

            // Update the status bar
            connectionStatusLabel.Text = e.Reason;

            // Tell the user when the server ends the connection (but not when they disconnected themselves).
            if (_wasConnected && e.Status == ConnectionStatus.Disconnected && !_isUserDisconnecting)
            {
                MessageBox.Show(this, e.Reason + ".", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (isConnected)
            {
                messageTextBox.Focus();
            }

            if (e.Status != ConnectionStatus.Connecting)
            {
                _wasConnected = isConnected;
            }
        }

        private void Chat_ErrorOccurred(object sender, ChatErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ChatErrorEventHandler(Chat_ErrorOccurred), sender, e);
                return;
            }

            MessageBox.Show(this, e.Message, ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Queues <paramref name="handler"/> to run on the UI thread. BeginInvoke is used (rather than
        /// Invoke) so the network threads never wait for the window to repaint.
        /// </summary>
        private void InvokeOnUiThread(Delegate handler, params object[] args)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            try
            {
                BeginInvoke(handler, args);
            }
            catch (InvalidOperationException)
            {
                // The window was closed while a message was arriving - nothing left to update.
            }
        }

        /// <summary>
        /// Adds a line to the conversation and scrolls to it.
        /// </summary>
        private void AddConversationLine(string text)
        {
            conversationTextBox.AppendText(text + Environment.NewLine);
        }

        #endregion
    }
}
