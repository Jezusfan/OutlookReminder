namespace OutlookReminder
{
    partial class frmSettings
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
            this.components = new System.ComponentModel.Container();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTimeSinceCheck = new System.Windows.Forms.Label();
            this.chkOutlookProcessFound = new System.Windows.Forms.CheckBox();
            this.chkOutlookVersionSupported = new System.Windows.Forms.CheckBox();
            this.txtUnsupportedDetails = new System.Windows.Forms.TextBox();
            this.lblNextCheck = new System.Windows.Forms.Label();
            this.timStatus = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStartupDelay = new System.Windows.Forms.Label();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.lblOutlookPath = new System.Windows.Forms.Label();
            this.btnBrowseFiles = new System.Windows.Forms.Button();
            this.chkStartOutlook = new System.Windows.Forms.CheckBox();
            this.chkBlackout = new System.Windows.Forms.CheckBox();
            this.chkStartAtLogin = new System.Windows.Forms.CheckBox();
            this.txtOutlookPath = new System.Windows.Forms.TextBox();
            this.numStartupDelay = new System.Windows.Forms.NumericUpDown();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartupDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Text = "Outlook reminder";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(117, 48);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // lblTimeSinceCheck
            // 
            this.lblTimeSinceCheck.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblTimeSinceCheck, 2);
            this.lblTimeSinceCheck.ForeColor = System.Drawing.Color.White;
            this.lblTimeSinceCheck.Location = new System.Drawing.Point(3, 3);
            this.lblTimeSinceCheck.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblTimeSinceCheck.Name = "lblTimeSinceCheck";
            this.lblTimeSinceCheck.Size = new System.Drawing.Size(190, 13);
            this.lblTimeSinceCheck.TabIndex = 1;
            this.lblTimeSinceCheck.Text = "Time since last Outlook process check";
            // 
            // chkOutlookProcessFound
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.chkOutlookProcessFound, 2);
            this.chkOutlookProcessFound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkOutlookProcessFound.Enabled = false;
            this.chkOutlookProcessFound.ForeColor = System.Drawing.Color.White;
            this.chkOutlookProcessFound.Location = new System.Drawing.Point(3, 100);
            this.chkOutlookProcessFound.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkOutlookProcessFound.Name = "chkOutlookProcessFound";
            this.chkOutlookProcessFound.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkOutlookProcessFound.Size = new System.Drawing.Size(239, 20);
            this.chkOutlookProcessFound.TabIndex = 4;
            this.chkOutlookProcessFound.Text = "Outlook process found";
            this.chkOutlookProcessFound.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOutlookProcessFound.UseVisualStyleBackColor = true;
            // 
            // chkOutlookVersionSupported
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.chkOutlookVersionSupported, 2);
            this.chkOutlookVersionSupported.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkOutlookVersionSupported.Enabled = false;
            this.chkOutlookVersionSupported.ForeColor = System.Drawing.Color.White;
            this.chkOutlookVersionSupported.Location = new System.Drawing.Point(3, 120);
            this.chkOutlookVersionSupported.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkOutlookVersionSupported.Name = "chkOutlookVersionSupported";
            this.chkOutlookVersionSupported.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkOutlookVersionSupported.Size = new System.Drawing.Size(239, 20);
            this.chkOutlookVersionSupported.TabIndex = 4;
            this.chkOutlookVersionSupported.Text = "Outlook version supported";
            this.chkOutlookVersionSupported.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOutlookVersionSupported.UseVisualStyleBackColor = true;
            // 
            // txtUnsupportedDetails
            // 
            this.txtUnsupportedDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txtUnsupportedDetails, 4);
            this.txtUnsupportedDetails.Location = new System.Drawing.Point(3, 143);
            this.txtUnsupportedDetails.Multiline = true;
            this.txtUnsupportedDetails.Name = "txtUnsupportedDetails";
            this.txtUnsupportedDetails.Size = new System.Drawing.Size(524, 72);
            this.txtUnsupportedDetails.TabIndex = 6;
            this.txtUnsupportedDetails.Visible = false;
            // 
            // lblNextCheck
            // 
            this.lblNextCheck.AutoSize = true;
            this.lblNextCheck.Location = new System.Drawing.Point(245, 3);
            this.lblNextCheck.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblNextCheck.Name = "lblNextCheck";
            this.lblNextCheck.Size = new System.Drawing.Size(24, 13);
            this.lblNextCheck.TabIndex = 7;
            this.lblNextCheck.Text = "10s";
            // 
            // timStatus
            // 
            this.timStatus.Tick += new System.EventHandler(this.timStatus_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 262F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.Controls.Add(this.lblOutlookPath, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnBrowseFiles, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtUnsupportedDetails, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.lblNextCheck, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblTimeSinceCheck, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkStartOutlook, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkOutlookVersionSupported, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.chkBlackout, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkOutlookProcessFound, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.chkStartAtLogin, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtOutlookPath, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(530, 218);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.numStartupDelay);
            this.panel1.Controls.Add(this.lblSeconds);
            this.panel1.Controls.Add(this.lblStartupDelay);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(242, 60);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(262, 20);
            this.panel1.TabIndex = 11;
            // 
            // lblStartupDelay
            // 
            this.lblStartupDelay.AutoSize = true;
            this.lblStartupDelay.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::OutlookReminder.Properties.Settings.Default, "StartOutlook", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblStartupDelay.ForeColor = System.Drawing.Color.White;
            this.lblStartupDelay.Location = new System.Drawing.Point(21, 4);
            this.lblStartupDelay.Name = "lblStartupDelay";
            this.lblStartupDelay.Size = new System.Drawing.Size(72, 13);
            this.lblStartupDelay.TabIndex = 0;
            this.lblStartupDelay.Text = "Startup delay:";
            this.lblStartupDelay.Visible = global::OutlookReminder.Properties.Settings.Default.StartOutlook;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::OutlookReminder.Properties.Settings.Default, "StartOutlook", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblSeconds.ForeColor = System.Drawing.Color.White;
            this.lblSeconds.Location = new System.Drawing.Point(172, 3);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(47, 13);
            this.lblSeconds.TabIndex = 0;
            this.lblSeconds.Text = "seconds";
            this.lblSeconds.Visible = global::OutlookReminder.Properties.Settings.Default.StartOutlook;
            // 
            // lblOutlookPath
            // 
            this.lblOutlookPath.AutoSize = true;
            this.lblOutlookPath.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::OutlookReminder.Properties.Settings.Default, "StartOutlook", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblOutlookPath.ForeColor = System.Drawing.Color.White;
            this.lblOutlookPath.Location = new System.Drawing.Point(3, 83);
            this.lblOutlookPath.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblOutlookPath.Name = "lblOutlookPath";
            this.lblOutlookPath.Size = new System.Drawing.Size(81, 13);
            this.lblOutlookPath.TabIndex = 10;
            this.lblOutlookPath.Text = "Path to Outlook";
            this.lblOutlookPath.Visible = global::OutlookReminder.Properties.Settings.Default.StartOutlook;
            // 
            // btnBrowseFiles
            // 
            this.btnBrowseFiles.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::OutlookReminder.Properties.Settings.Default, "StartOutlook", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.btnBrowseFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseFiles.Location = new System.Drawing.Point(504, 80);
            this.btnBrowseFiles.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowseFiles.Name = "btnBrowseFiles";
            this.btnBrowseFiles.Size = new System.Drawing.Size(26, 20);
            this.btnBrowseFiles.TabIndex = 0;
            this.btnBrowseFiles.Text = "...";
            this.btnBrowseFiles.UseVisualStyleBackColor = true;
            this.btnBrowseFiles.Visible = global::OutlookReminder.Properties.Settings.Default.StartOutlook;
            this.btnBrowseFiles.Click += new System.EventHandler(this.btnBrowseFiles_Click);
            // 
            // chkStartOutlook
            // 
            this.chkStartOutlook.Checked = global::OutlookReminder.Properties.Settings.Default.StartOutlook;
            this.chkStartOutlook.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.chkStartOutlook, 2);
            this.chkStartOutlook.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::OutlookReminder.Properties.Settings.Default, "StartOutlook", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkStartOutlook.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkStartOutlook.ForeColor = System.Drawing.Color.White;
            this.chkStartOutlook.Location = new System.Drawing.Point(3, 60);
            this.chkStartOutlook.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkStartOutlook.Name = "chkStartOutlook";
            this.chkStartOutlook.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkStartOutlook.Size = new System.Drawing.Size(239, 20);
            this.chkStartOutlook.TabIndex = 8;
            this.chkStartOutlook.Text = "Start Outlook";
            this.chkStartOutlook.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStartOutlook.UseVisualStyleBackColor = true;
            // 
            // chkBlackout
            // 
            this.chkBlackout.Checked = global::OutlookReminder.Properties.Settings.Default.Blackout;
            this.chkBlackout.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.chkBlackout, 2);
            this.chkBlackout.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::OutlookReminder.Properties.Settings.Default, "Blackout", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkBlackout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkBlackout.ForeColor = System.Drawing.Color.White;
            this.chkBlackout.Location = new System.Drawing.Point(3, 20);
            this.chkBlackout.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkBlackout.Name = "chkBlackout";
            this.chkBlackout.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkBlackout.Size = new System.Drawing.Size(239, 20);
            this.chkBlackout.TabIndex = 4;
            this.chkBlackout.Text = "Emphasize Reminder Window using blackout";
            this.chkBlackout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkBlackout.UseVisualStyleBackColor = true;
            // 
            // chkStartAtLogin
            // 
            this.chkStartAtLogin.Checked = global::OutlookReminder.Properties.Settings.Default.StartAtLogon;
            this.chkStartAtLogin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.chkStartAtLogin, 2);
            this.chkStartAtLogin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::OutlookReminder.Properties.Settings.Default, "StartAtLogon", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkStartAtLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkStartAtLogin.ForeColor = System.Drawing.Color.White;
            this.chkStartAtLogin.Location = new System.Drawing.Point(3, 40);
            this.chkStartAtLogin.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chkStartAtLogin.Name = "chkStartAtLogin";
            this.chkStartAtLogin.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkStartAtLogin.Size = new System.Drawing.Size(239, 20);
            this.chkStartAtLogin.TabIndex = 4;
            this.chkStartAtLogin.Text = "Startup at login";
            this.chkStartAtLogin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStartAtLogin.UseVisualStyleBackColor = true;
            // 
            // txtOutlookPath
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtOutlookPath, 2);
            this.txtOutlookPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::OutlookReminder.Properties.Settings.Default, "OutlookPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, null, "N0"));
            this.txtOutlookPath.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::OutlookReminder.Properties.Settings.Default, "StartOutlook", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtOutlookPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutlookPath.Location = new System.Drawing.Point(106, 80);
            this.txtOutlookPath.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.txtOutlookPath.Name = "txtOutlookPath";
            this.txtOutlookPath.Size = new System.Drawing.Size(395, 20);
            this.txtOutlookPath.TabIndex = 1;
            this.txtOutlookPath.Text = global::OutlookReminder.Properties.Settings.Default.OutlookPath;
            this.txtOutlookPath.Visible = global::OutlookReminder.Properties.Settings.Default.StartOutlook;
            // 
            // numStartupDelay
            // 
            this.numStartupDelay.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::OutlookReminder.Properties.Settings.Default, "StartupDelaySec", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numStartupDelay.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::OutlookReminder.Properties.Settings.Default, "StartOutlook", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numStartupDelay.Location = new System.Drawing.Point(106, 1);
            this.numStartupDelay.Name = "numStartupDelay";
            this.numStartupDelay.Size = new System.Drawing.Size(50, 20);
            this.numStartupDelay.TabIndex = 2;
            this.numStartupDelay.Value = global::OutlookReminder.Properties.Settings.Default.StartupDelaySec;
            this.numStartupDelay.Visible = global::OutlookReminder.Properties.Settings.Default.StartOutlook;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(530, 218);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.ShowInTaskbar = false;
            this.Text = "Outlook reminder settings";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSettings_FormClosed);
            this.Resize += new System.EventHandler(this.frmSettings_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartupDelay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label lblTimeSinceCheck;
        private System.Windows.Forms.CheckBox chkBlackout;
        private System.Windows.Forms.CheckBox chkStartAtLogin;
        private System.Windows.Forms.CheckBox chkOutlookProcessFound;
        private System.Windows.Forms.CheckBox chkOutlookVersionSupported;
        private System.Windows.Forms.TextBox txtUnsupportedDetails;
        private System.Windows.Forms.Label lblNextCheck;
        private System.Windows.Forms.Timer timStatus;
        private System.Windows.Forms.CheckBox chkStartOutlook;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblOutlookPath;
        private System.Windows.Forms.TextBox txtOutlookPath;
        private System.Windows.Forms.Button btnBrowseFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.Label lblStartupDelay;
        private System.Windows.Forms.NumericUpDown numStartupDelay;
    }
}