using prog2200_assign1_library;

namespace prog2200_assign1;

/// <summary>
/// The Network Game window. All chat networking lives in <see cref="ChatClient"/>
/// (prog2200_assign1_library); this form only displays what the library reports.
/// </summary>
public partial class MainForm : Form
{
    // Client and server run on the same machine (start the server with: ncat -l -p 1234 -t -v).
    private const string ServerHost = "127.0.0.1";
    private const int ServerPort = 1234;

    // Tags that show where each line in the conversation came from.
    // Server messages get no tag so they appear exactly as the server typed them.
    private const string SentTag = ">>";
    private const string SystemTag = "***";

    private readonly ChatClient _chatClient = new();
    private ConnectionState _lastState = ConnectionState.Disconnected;

    public MainForm()
    {
        InitializeComponent();

        _chatClient.MessageReceived += ChatClient_MessageReceived;
        _chatClient.MessageSent += ChatClient_MessageSent;
        _chatClient.ConnectionStatusChanged += ChatClient_ConnectionStatusChanged;
        _chatClient.ErrorOccurred += ChatClient_ErrorOccurred;

        UpdateConnectionControls(ConnectionState.Disconnected, "Not connected");
    }

    #region Menu and button handlers

    private async void connectMenuItem_Click(object sender, EventArgs e)
    {
        // Awaiting keeps the UI responsive while the connection is made in the background.
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
        string message = messageTextBox.Text;

        if (message.Length == 0)
        {
            return;
        }

        // Send() only queues the message, so the UI never waits on the network.
        if (_chatClient.Send(message))
        {
            messageTextBox.Clear();
        }

        messageTextBox.Focus();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Stop listening first so no background event tries to update a closing form.
        _chatClient.MessageReceived -= ChatClient_MessageReceived;
        _chatClient.MessageSent -= ChatClient_MessageSent;
        _chatClient.ConnectionStatusChanged -= ChatClient_ConnectionStatusChanged;
        _chatClient.ErrorOccurred -= ChatClient_ErrorOccurred;

        _chatClient.Dispose();
    }

    #endregion

    #region ChatClient event handlers (called on background threads)

    private void ChatClient_MessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        RunOnUiThread(() => AppendToConversation(e.Message));
    }

    private void ChatClient_MessageSent(object? sender, MessageSentEventArgs e)
    {
        RunOnUiThread(() => AppendToConversation(SentTag + e.Message));
    }

    private void ChatClient_ConnectionStatusChanged(object? sender, ConnectionStatusChangedEventArgs e)
    {
        RunOnUiThread(() => UpdateConnectionControls(e.State, e.Description));
    }

    private void ChatClient_ErrorOccurred(object? sender, ChatErrorEventArgs e)
    {
        RunOnUiThread(() => AppendToConversation($"{SystemTag} {e.Message}"));
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Marshals <paramref name="action"/> onto the UI thread. BeginInvoke is used (rather than
    /// Invoke) so the network thread never has to wait for the UI to repaint.
    /// </summary>
    private void RunOnUiThread(Action action)
    {
        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        if (!InvokeRequired)
        {
            action();
            return;
        }

        try
        {
            BeginInvoke(() =>
            {
                if (!IsDisposed)
                {
                    action();
                }
            });
        }
        catch (InvalidOperationException)
        {
            // The window handle was destroyed while the form was closing - nothing left to update.
        }
    }

    private void AppendToConversation(string line)
    {
        // AppendText also scrolls the conversation to the newest line.
        conversationTextBox.AppendText(line + Environment.NewLine);
    }

    private void UpdateConnectionControls(ConnectionState state, string description)
    {
        bool isConnected = state == ConnectionState.Connected;

        connectMenuItem.Enabled = state == ConnectionState.Disconnected;
        disconnectMenuItem.Enabled = isConnected;
        messageTextBox.Enabled = isConnected;
        sendButton.Enabled = isConnected;
        connectionStatusLabel.Text = description;

        // Note connects and disconnects in the conversation (failed attempts are reported as errors).
        if (state == ConnectionState.Connected
            || (state == ConnectionState.Disconnected && _lastState == ConnectionState.Connected))
        {
            AppendToConversation($"{SystemTag} {description} {SystemTag}");
        }

        if (isConnected)
        {
            messageTextBox.Focus();
        }

        _lastState = state;
    }

    #endregion
}
