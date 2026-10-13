using System;
using System.Drawing;
using System.Windows.Forms;
using prog2200_assign1_library;

namespace prog2200_assign1
{
    /// <summary>
    /// The Network Game window. All chat networking lives in <see cref="ChatClient"/>
    /// (prog2200_assign1_library); this form only shows what the library reports.
    /// </summary>
    public partial class MainForm : Form
    {
        // Client and server run on the same machine (start the server with: ncat -l -p 1234 -t -v).
        private const string ServerHost = "127.0.0.1";
        private const int ServerPort = 1234;

        // Tags that show where each line in the conversation came from.
        private const string SentTag = ">> ";
        private const string ReceivedTag = "<< ";
        private const string SystemTag = "*** ";

        private static readonly Color TimeColor = Color.Gray;
        private static readonly Color SentColor = Color.RoyalBlue;
        private static readonly Color ReceivedColor = Color.ForestGreen;
        private static readonly Color SystemColor = Color.DimGray;
        private static readonly Color ErrorColor = Color.Firebrick;

        private readonly ChatClient _chatClient = new ChatClient();
        private bool _wasConnected;

        public MainForm()
        {
            InitializeComponent();

            _chatClient.MessageReceived += new ChatMessageEventHandler(ChatClient_MessageReceived);
            _chatClient.MessageSent += new ChatMessageEventHandler(ChatClient_MessageSent);
            _chatClient.ConnectionChanged += new ConnectionChangedEventHandler(ChatClient_ConnectionChanged);
            _chatClient.ErrorOccurred += new ChatErrorEventHandler(ChatClient_ErrorOccurred);

            ShowConnectionStatus(ConnectionStatus.Disconnected, "Not connected");
        }

        /// <summary>
        /// Stops listening to the chat client and closes the connection before the window goes away.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
#if DEBUG
            // TEMPORARY DIAGNOSTIC: shows what closed the window. Remove once the early-exit problem is found.
            MessageBox.Show("Closing because: " + e.CloseReason + Environment.NewLine + Environment.NewLine
                + Environment.StackTrace, "DEBUG - window closing");
#endif

            _chatClient.MessageReceived -= ChatClient_MessageReceived;
            _chatClient.MessageSent -= ChatClient_MessageSent;
            _chatClient.ConnectionChanged -= ChatClient_ConnectionChanged;
            _chatClient.ErrorOccurred -= ChatClient_ErrorOccurred;

            _chatClient.Dispose();

            base.OnFormClosing(e);
        }

        #region Menu and button handlers

        private async void connectMenuItem_Click(object sender, EventArgs e)
        {
            // The connection is made in the background; the window stays responsive while we wait.
            await _chatClient.ConnectAsync(ServerHost, ServerPort);
        }

        private void disconnectMenuItem_Click(object sender, EventArgs e)
        {
            _chatClient.Disconnect();
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
            if (_chatClient.Send(messageTextBox.Text))
            {
                messageTextBox.Clear();
            }

            messageTextBox.Focus();
        }

        #endregion

        #region Chat client event handlers

        // These handlers are first called on the chat client's background threads. Each one checks
        // InvokeRequired and, if needed, calls itself again on the UI thread before touching controls.

        private void ChatClient_MessageReceived(object sender, ChatMessageEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ChatMessageEventHandler(ChatClient_MessageReceived), sender, e);
                return;
            }

            AddConversationLine(e.Time, ReceivedTag + e.Text, ReceivedColor);
        }

        private void ChatClient_MessageSent(object sender, ChatMessageEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ChatMessageEventHandler(ChatClient_MessageSent), sender, e);
                return;
            }

            AddConversationLine(e.Time, SentTag + e.Text, SentColor);
        }

        private void ChatClient_ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ConnectionChangedEventHandler(ChatClient_ConnectionChanged), sender, e);
                return;
            }

            ShowConnectionStatus(e.Status, e.Reason);
        }

        private void ChatClient_ErrorOccurred(object sender, ChatErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeOnUiThread(new ChatErrorEventHandler(ChatClient_ErrorOccurred), sender, e);
                return;
            }

            AddConversationLine(DateTime.Now, SystemTag + e.Message, ErrorColor);
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
        /// Adds a time-stamped, coloured line to the conversation and scrolls to it.
        /// </summary>
        private void AddConversationLine(DateTime time, string text, Color color)
        {
            AppendColoredText("[" + time.ToString("HH:mm:ss") + "] ", TimeColor);
            AppendColoredText(text + Environment.NewLine, color);

            conversationBox.ScrollToCaret();
        }

        private void AppendColoredText(string text, Color color)
        {
            conversationBox.SelectionStart = conversationBox.TextLength;
            conversationBox.SelectionLength = 0;
            conversationBox.SelectionColor = color;
            conversationBox.AppendText(text);
        }

        /// <summary>
        /// Enables only the controls that make sense for the connection status.
        /// </summary>
        private void ShowConnectionStatus(ConnectionStatus status, string reason)
        {
            bool isConnected = status == ConnectionStatus.Connected;

            connectMenuItem.Enabled = status == ConnectionStatus.Disconnected;
            disconnectMenuItem.Enabled = isConnected;
            messageTextBox.Enabled = isConnected;
            sendButton.Enabled = isConnected;
            connectionStatusLabel.Text = reason;

            // Note connects and disconnects in the conversation (failed attempts are shown as errors).
            if (isConnected || (status == ConnectionStatus.Disconnected && _wasConnected))
            {
                AddConversationLine(DateTime.Now, SystemTag + reason, SystemColor);
            }

            if (isConnected)
            {
                messageTextBox.Focus();
            }

            _wasConnected = isConnected;
        }

        #endregion
    }
}
