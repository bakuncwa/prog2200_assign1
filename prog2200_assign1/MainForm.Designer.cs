namespace prog2200_assign1
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.gameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gamePanel = new System.Windows.Forms.Panel();
            this.gameLabel = new System.Windows.Forms.Label();
            this.chatPanel = new System.Windows.Forms.Panel();
            this.conversationGroupBox = new System.Windows.Forms.GroupBox();
            this.conversationBox = new System.Windows.Forms.RichTextBox();
            this.spacerPanel = new System.Windows.Forms.Panel();
            this.messageGroupBox = new System.Windows.Forms.GroupBox();
            this.messageLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip.SuspendLayout();
            this.gamePanel.SuspendLayout();
            this.chatPanel.SuspendLayout();
            this.conversationGroupBox.SuspendLayout();
            this.messageGroupBox.SuspendLayout();
            this.messageLayoutPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // mainMenuStrip
            //
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameMenuItem,
            this.networkMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(624, 24);
            this.mainMenuStrip.TabIndex = 0;
            //
            // gameMenuItem
            //
            this.gameMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitMenuItem});
            this.gameMenuItem.Name = "gameMenuItem";
            this.gameMenuItem.Size = new System.Drawing.Size(50, 20);
            this.gameMenuItem.Text = "&Game";
            //
            // exitMenuItem
            //
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            //
            // networkMenuItem
            //
            this.networkMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectMenuItem,
            this.disconnectMenuItem});
            this.networkMenuItem.Name = "networkMenuItem";
            this.networkMenuItem.Size = new System.Drawing.Size(64, 20);
            this.networkMenuItem.Text = "&Network";
            //
            // connectMenuItem
            //
            this.connectMenuItem.Name = "connectMenuItem";
            this.connectMenuItem.Size = new System.Drawing.Size(133, 22);
            this.connectMenuItem.Text = "&Connect";
            this.connectMenuItem.Click += new System.EventHandler(this.connectMenuItem_Click);
            //
            // disconnectMenuItem
            //
            this.disconnectMenuItem.Name = "disconnectMenuItem";
            this.disconnectMenuItem.Size = new System.Drawing.Size(133, 22);
            this.disconnectMenuItem.Text = "&Disconnect";
            this.disconnectMenuItem.Click += new System.EventHandler(this.disconnectMenuItem_Click);
            //
            // gamePanel
            //
            this.gamePanel.BackColor = System.Drawing.Color.Black;
            this.gamePanel.Controls.Add(this.gameLabel);
            this.gamePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.gamePanel.Location = new System.Drawing.Point(0, 24);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(624, 300);
            this.gamePanel.TabIndex = 1;
            //
            // gameLabel
            //
            this.gameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameLabel.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gameLabel.ForeColor = System.Drawing.Color.White;
            this.gameLabel.Location = new System.Drawing.Point(0, 0);
            this.gameLabel.Name = "gameLabel";
            this.gameLabel.Size = new System.Drawing.Size(624, 300);
            this.gameLabel.TabIndex = 0;
            this.gameLabel.Text = "Game Goes Here";
            this.gameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // chatPanel
            //
            this.chatPanel.Controls.Add(this.conversationGroupBox);
            this.chatPanel.Controls.Add(this.spacerPanel);
            this.chatPanel.Controls.Add(this.messageGroupBox);
            this.chatPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatPanel.Location = new System.Drawing.Point(0, 324);
            this.chatPanel.Name = "chatPanel";
            this.chatPanel.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.chatPanel.Size = new System.Drawing.Size(624, 315);
            this.chatPanel.TabIndex = 2;
            //
            // conversationGroupBox
            //
            this.conversationGroupBox.Controls.Add(this.conversationBox);
            this.conversationGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conversationGroupBox.Location = new System.Drawing.Point(12, 82);
            this.conversationGroupBox.Name = "conversationGroupBox";
            this.conversationGroupBox.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.conversationGroupBox.Size = new System.Drawing.Size(600, 223);
            this.conversationGroupBox.TabIndex = 1;
            this.conversationGroupBox.TabStop = false;
            this.conversationGroupBox.Text = "Conversation";
            //
            // conversationBox
            //
            this.conversationBox.BackColor = System.Drawing.SystemColors.Window;
            this.conversationBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.conversationBox.DetectUrls = false;
            this.conversationBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conversationBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conversationBox.Location = new System.Drawing.Point(10, 22);
            this.conversationBox.Name = "conversationBox";
            this.conversationBox.ReadOnly = true;
            this.conversationBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.conversationBox.Size = new System.Drawing.Size(580, 191);
            this.conversationBox.TabIndex = 0;
            this.conversationBox.TabStop = false;
            this.conversationBox.Text = "";
            //
            // spacerPanel
            //
            this.spacerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.spacerPanel.Location = new System.Drawing.Point(12, 72);
            this.spacerPanel.Name = "spacerPanel";
            this.spacerPanel.Size = new System.Drawing.Size(600, 10);
            this.spacerPanel.TabIndex = 2;
            //
            // messageGroupBox
            //
            this.messageGroupBox.Controls.Add(this.messageLayoutPanel);
            this.messageGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.messageGroupBox.Location = new System.Drawing.Point(12, 10);
            this.messageGroupBox.Name = "messageGroupBox";
            this.messageGroupBox.Padding = new System.Windows.Forms.Padding(10, 6, 10, 8);
            this.messageGroupBox.Size = new System.Drawing.Size(600, 62);
            this.messageGroupBox.TabIndex = 0;
            this.messageGroupBox.TabStop = false;
            this.messageGroupBox.Text = "Type your messages here.";
            //
            // messageLayoutPanel
            //
            this.messageLayoutPanel.ColumnCount = 2;
            this.messageLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.messageLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.messageLayoutPanel.Controls.Add(this.messageTextBox, 0, 0);
            this.messageLayoutPanel.Controls.Add(this.sendButton, 1, 0);
            this.messageLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageLayoutPanel.Location = new System.Drawing.Point(10, 22);
            this.messageLayoutPanel.Name = "messageLayoutPanel";
            this.messageLayoutPanel.RowCount = 1;
            this.messageLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.messageLayoutPanel.Size = new System.Drawing.Size(580, 32);
            this.messageLayoutPanel.TabIndex = 0;
            //
            // messageTextBox
            //
            this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.messageTextBox.Location = new System.Drawing.Point(3, 4);
            this.messageTextBox.MaxLength = 1000;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(478, 23);
            this.messageTextBox.TabIndex = 0;
            //
            // sendButton
            //
            this.sendButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.sendButton.Location = new System.Drawing.Point(487, 3);
            this.sendButton.Margin = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(90, 26);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "&Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 639);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(624, 22);
            this.statusStrip.TabIndex = 3;
            //
            // connectionStatusLabel
            //
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(85, 17);
            this.connectionStatusLabel.Text = "Not connected";
            //
            // MainForm
            //
            this.AcceptButton = this.sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 661);
            this.Controls.Add(this.chatPanel);
            this.Controls.Add(this.gamePanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.mainMenuStrip;
            this.MinimumSize = new System.Drawing.Size(480, 560);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Network Game";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.gamePanel.ResumeLayout(false);
            this.chatPanel.ResumeLayout(false);
            this.conversationGroupBox.ResumeLayout(false);
            this.messageGroupBox.ResumeLayout(false);
            this.messageLayoutPanel.ResumeLayout(false);
            this.messageLayoutPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem gameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectMenuItem;
        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.Label gameLabel;
        private System.Windows.Forms.Panel chatPanel;
        private System.Windows.Forms.GroupBox conversationGroupBox;
        private System.Windows.Forms.RichTextBox conversationBox;
        private System.Windows.Forms.Panel spacerPanel;
        private System.Windows.Forms.GroupBox messageGroupBox;
        private System.Windows.Forms.TableLayoutPanel messageLayoutPanel;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
    }
}
