namespace prog2200_assign1;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        mainMenuStrip = new MenuStrip();
        gameMenuItem = new ToolStripMenuItem();
        exitMenuItem = new ToolStripMenuItem();
        networkMenuItem = new ToolStripMenuItem();
        connectMenuItem = new ToolStripMenuItem();
        disconnectMenuItem = new ToolStripMenuItem();
        gamePanel = new Panel();
        gameLabel = new Label();
        chatPanel = new Panel();
        conversationGroupBox = new GroupBox();
        conversationTextBox = new TextBox();
        spacerPanel = new Panel();
        messageGroupBox = new GroupBox();
        messageLayoutPanel = new TableLayoutPanel();
        messageTextBox = new TextBox();
        sendButton = new Button();
        statusStrip = new StatusStrip();
        connectionStatusLabel = new ToolStripStatusLabel();
        mainMenuStrip.SuspendLayout();
        gamePanel.SuspendLayout();
        chatPanel.SuspendLayout();
        conversationGroupBox.SuspendLayout();
        messageGroupBox.SuspendLayout();
        messageLayoutPanel.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // mainMenuStrip
        //
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { gameMenuItem, networkMenuItem });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(624, 24);
        mainMenuStrip.TabIndex = 0;
        //
        // gameMenuItem
        //
        gameMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitMenuItem });
        gameMenuItem.Name = "gameMenuItem";
        gameMenuItem.Size = new Size(50, 20);
        gameMenuItem.Text = "&Game";
        //
        // exitMenuItem
        //
        exitMenuItem.Name = "exitMenuItem";
        exitMenuItem.Size = new Size(180, 22);
        exitMenuItem.Text = "E&xit";
        exitMenuItem.Click += exitMenuItem_Click;
        //
        // networkMenuItem
        //
        networkMenuItem.DropDownItems.AddRange(new ToolStripItem[] { connectMenuItem, disconnectMenuItem });
        networkMenuItem.Name = "networkMenuItem";
        networkMenuItem.Size = new Size(64, 20);
        networkMenuItem.Text = "&Network";
        //
        // connectMenuItem
        //
        connectMenuItem.Name = "connectMenuItem";
        connectMenuItem.Size = new Size(180, 22);
        connectMenuItem.Text = "&Connect";
        connectMenuItem.Click += connectMenuItem_Click;
        //
        // disconnectMenuItem
        //
        disconnectMenuItem.Enabled = false;
        disconnectMenuItem.Name = "disconnectMenuItem";
        disconnectMenuItem.Size = new Size(180, 22);
        disconnectMenuItem.Text = "&Disconnect";
        disconnectMenuItem.Click += disconnectMenuItem_Click;
        //
        // gamePanel
        //
        gamePanel.BackColor = Color.Black;
        gamePanel.Controls.Add(gameLabel);
        gamePanel.Dock = DockStyle.Top;
        gamePanel.Location = new Point(0, 24);
        gamePanel.Name = "gamePanel";
        gamePanel.Size = new Size(624, 300);
        gamePanel.TabIndex = 1;
        //
        // gameLabel
        //
        gameLabel.Dock = DockStyle.Fill;
        gameLabel.Font = new Font("Arial", 20F, FontStyle.Regular, GraphicsUnit.Point);
        gameLabel.ForeColor = Color.White;
        gameLabel.Location = new Point(0, 0);
        gameLabel.Name = "gameLabel";
        gameLabel.Size = new Size(624, 300);
        gameLabel.TabIndex = 0;
        gameLabel.Text = "Game Goes Here";
        gameLabel.TextAlign = ContentAlignment.MiddleCenter;
        //
        // chatPanel
        //
        chatPanel.Controls.Add(conversationGroupBox);
        chatPanel.Controls.Add(spacerPanel);
        chatPanel.Controls.Add(messageGroupBox);
        chatPanel.Dock = DockStyle.Fill;
        chatPanel.Location = new Point(0, 324);
        chatPanel.Name = "chatPanel";
        chatPanel.Padding = new Padding(12, 10, 12, 10);
        chatPanel.Size = new Size(624, 315);
        chatPanel.TabIndex = 2;
        //
        // conversationGroupBox
        //
        conversationGroupBox.Controls.Add(conversationTextBox);
        conversationGroupBox.Dock = DockStyle.Fill;
        conversationGroupBox.Location = new Point(12, 82);
        conversationGroupBox.Name = "conversationGroupBox";
        conversationGroupBox.Padding = new Padding(10, 6, 10, 10);
        conversationGroupBox.Size = new Size(600, 223);
        conversationGroupBox.TabIndex = 1;
        conversationGroupBox.TabStop = false;
        conversationGroupBox.Text = "Conversation";
        //
        // conversationTextBox
        //
        conversationTextBox.BackColor = SystemColors.Window;
        conversationTextBox.Dock = DockStyle.Fill;
        conversationTextBox.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
        conversationTextBox.Location = new Point(10, 22);
        conversationTextBox.Multiline = true;
        conversationTextBox.Name = "conversationTextBox";
        conversationTextBox.ReadOnly = true;
        conversationTextBox.ScrollBars = ScrollBars.Vertical;
        conversationTextBox.Size = new Size(580, 191);
        conversationTextBox.TabIndex = 0;
        conversationTextBox.TabStop = false;
        //
        // spacerPanel
        //
        spacerPanel.Dock = DockStyle.Top;
        spacerPanel.Location = new Point(12, 72);
        spacerPanel.Name = "spacerPanel";
        spacerPanel.Size = new Size(600, 10);
        spacerPanel.TabIndex = 2;
        //
        // messageGroupBox
        //
        messageGroupBox.Controls.Add(messageLayoutPanel);
        messageGroupBox.Dock = DockStyle.Top;
        messageGroupBox.Location = new Point(12, 10);
        messageGroupBox.Name = "messageGroupBox";
        messageGroupBox.Padding = new Padding(10, 6, 10, 8);
        messageGroupBox.Size = new Size(600, 62);
        messageGroupBox.TabIndex = 0;
        messageGroupBox.TabStop = false;
        messageGroupBox.Text = "Type your messages here.";
        //
        // messageLayoutPanel
        //
        messageLayoutPanel.ColumnCount = 2;
        messageLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        messageLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        messageLayoutPanel.Controls.Add(messageTextBox, 0, 0);
        messageLayoutPanel.Controls.Add(sendButton, 1, 0);
        messageLayoutPanel.Dock = DockStyle.Fill;
        messageLayoutPanel.Location = new Point(10, 22);
        messageLayoutPanel.Name = "messageLayoutPanel";
        messageLayoutPanel.RowCount = 1;
        messageLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        messageLayoutPanel.Size = new Size(580, 32);
        messageLayoutPanel.TabIndex = 0;
        //
        // messageTextBox
        //
        messageTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        messageTextBox.Location = new Point(3, 4);
        messageTextBox.MaxLength = 1000;
        messageTextBox.Name = "messageTextBox";
        messageTextBox.Size = new Size(478, 23);
        messageTextBox.TabIndex = 0;
        //
        // sendButton
        //
        sendButton.Anchor = AnchorStyles.Right;
        sendButton.Location = new Point(487, 3);
        sendButton.Margin = new Padding(8, 3, 0, 3);
        sendButton.Name = "sendButton";
        sendButton.Size = new Size(90, 26);
        sendButton.TabIndex = 1;
        sendButton.Text = "&Send";
        sendButton.UseVisualStyleBackColor = true;
        sendButton.Click += sendButton_Click;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { connectionStatusLabel });
        statusStrip.Location = new Point(0, 639);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(624, 22);
        statusStrip.TabIndex = 3;
        //
        // connectionStatusLabel
        //
        connectionStatusLabel.Name = "connectionStatusLabel";
        connectionStatusLabel.Size = new Size(85, 17);
        connectionStatusLabel.Text = "Not connected";
        //
        // MainForm
        //
        AcceptButton = sendButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(624, 661);
        Controls.Add(chatPanel);
        Controls.Add(gamePanel);
        Controls.Add(statusStrip);
        Controls.Add(mainMenuStrip);
        MainMenuStrip = mainMenuStrip;
        MinimumSize = new Size(480, 560);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Network Game";
        FormClosing += MainForm_FormClosing;
        mainMenuStrip.ResumeLayout(false);
        mainMenuStrip.PerformLayout();
        gamePanel.ResumeLayout(false);
        chatPanel.ResumeLayout(false);
        conversationGroupBox.ResumeLayout(false);
        conversationGroupBox.PerformLayout();
        messageGroupBox.ResumeLayout(false);
        messageLayoutPanel.ResumeLayout(false);
        messageLayoutPanel.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip mainMenuStrip;
    private ToolStripMenuItem gameMenuItem;
    private ToolStripMenuItem exitMenuItem;
    private ToolStripMenuItem networkMenuItem;
    private ToolStripMenuItem connectMenuItem;
    private ToolStripMenuItem disconnectMenuItem;
    private Panel gamePanel;
    private Label gameLabel;
    private Panel chatPanel;
    private GroupBox conversationGroupBox;
    private TextBox conversationTextBox;
    private Panel spacerPanel;
    private GroupBox messageGroupBox;
    private TableLayoutPanel messageLayoutPanel;
    private TextBox messageTextBox;
    private Button sendButton;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel connectionStatusLabel;
}
