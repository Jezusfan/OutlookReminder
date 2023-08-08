using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.Win32;
using OutlookReminder.Properties;

namespace OutlookReminder
{
    public partial class frmSettings : Form
    {
        private OutlookMonitor _outlookMonitor;
        private frmBlackout _frmBlackout;
        private System.Threading.Timer _timer;
        private DateTime _lastCheck;
        //private DateTime _startup = DateTime.Now;
        private bool _suppressError;
        private bool _showDueToError;

        public frmSettings()
        {
            InitializeComponent();
            this.Icon = Resources.TaskIcon;
            this.notifyIcon1.Icon = Resources.TaskIcon;
            _outlookMonitor = new OutlookMonitor();
            _outlookMonitor.ReminderShown += _outlookMonitor_ReminderShown;
            _outlookMonitor.ReminderClosed += _outlookMonitor_ReminderClosed;
            _outlookMonitor.ConnectedStateChanged += _outlookMonitor_ConnectedStateChanged;
        }

        private bool ShowSettings
        {
            get { return (!_suppressError && ((!_outlookMonitor.OutlookVersionSupported || !_outlookMonitor.OutlookProcessFound) && _outlookMonitor.ShowSettingAllowed || _outlookMonitor.CredentialMismatch) || Settings.Default.FirstTime); }
        }

        private void _outlookMonitor_ConnectedStateChanged()
        {
            if (InvokeRequired)
                this.Invoke(new Action(_outlookMonitor_ConnectedStateChanged));
            else
            {
                chkOutlookProcessFound.Checked = _outlookMonitor.OutlookProcessFound;
                chkOutlookVersionSupported.Checked = _outlookMonitor.OutlookVersionSupported;
                txtUnsupportedDetails.Visible = !chkOutlookVersionSupported.Checked;
                txtUnsupportedDetails.Text = _outlookMonitor.UnsupportedReason;
                _lastCheck = DateTime.Now;
                if (ShowSettings)
                    ToggleWindowState(FormWindowState.Normal, true);
                else
                {
                    if (_showDueToError)
                        ToggleWindowState(FormWindowState.Minimized, false);
                }
            }
        }

        private void _outlookMonitor_ReminderClosed()
        {
            if (InvokeRequired)
                this.Invoke(new Action(_outlookMonitor_ReminderClosed));
            else
            {
                CloseExistingBlackoutForm();
                _timer?.Dispose();
                _timer = null;
            }
        }

        private bool CloseExistingBlackoutForm()
        {
            if (_frmBlackout != null)
            {
                _frmBlackout.ReminderClicked();
                _frmBlackout.Dispose();
                _frmBlackout = null;
                return true;
            }
            return false;
        }

        private void _outlookMonitor_ReminderShown(IntPtr handle)
        {
            if (InvokeRequired)
                this.Invoke( new Action<IntPtr>(_outlookMonitor_ReminderShown), handle);
            else
            {
                if (_frmBlackout != null && _frmBlackout.IsDisposed)
                    _frmBlackout = null;
                if (_frmBlackout == null && Settings.Default.Blackout)
                {
                    foreach (var screen in Screen.AllScreens)
                    {
                        if (_frmBlackout == null)
                            _frmBlackout = new frmBlackout(screen.Bounds, screen.Primary);
                        else
                        {
                            _frmBlackout.AddScreen(new frmBlackout(screen.Bounds, screen.Primary));
                        }
                    }
                    _frmBlackout.Show(this);
                }
                if (_frmBlackout != null && Settings.Default.Blackout)
                {
                    WindowHook.FocusWindow(_frmBlackout.Handle);
                    Application.DoEvents();
                } 
                //always put reminder window on top
                WindowHook.FocusWindow(handle);
                if (_timer == null)
                    _timer = new System.Threading.Timer(timer_tick, handle, 1000, 1000);
            }
        }
        
        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && DialogResult != DialogResult.OK)
            {
                Settings.Default.FirstTime = false;
                Properties.Settings.Default.Save();
                
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                //Path to launch shortcut
                
                string runRegKey = "Outlook Reminder";
                string startPath = Application.ExecutablePath;
                Log.Write(EventLogEntryType.Information, $"Setting startup path to: {startPath}");
                var existing = rkApp.GetValue(runRegKey);
                if (Settings.Default.StartAtLogon)
                {
                    if (existing == null || existing != startPath)
                        rkApp.SetValue(runRegKey, startPath);
                }
                else
                {
                    if (existing != null)
                        rkApp.DeleteValue(runRegKey);
                }
                
                _suppressError = !_outlookMonitor.OutlookVersionSupported || !_outlookMonitor.OutlookProcessFound || _outlookMonitor.CredentialMismatch;
                ToggleWindowState(FormWindowState.Minimized, false);
                e.Cancel = true;
            }
        }

        private void timer_tick(object state)
        {
            WindowHook.FocusWindow((IntPtr)state);
            Application.DoEvents();
        }

        private void frmSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseExistingBlackoutForm();
            _outlookMonitor.Dispose();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleWindowState(FormWindowState.Normal, false);
        }

        private void ToggleWindowState(FormWindowState state, bool showDueToError)
        {
            _showDueToError = showDueToError;
            if (state == FormWindowState.Normal)
            {
                timStatus.Enabled = true;
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
            }
            else
            {
                timStatus.Enabled = false;
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void frmSettings_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }
        
        private void timStatus_Tick(object sender, EventArgs e)
        {
            var span = DateTime.Now.Subtract(_lastCheck);
            lblNextCheck.Text = string.Format("{0}.{1} sec", span.Seconds, span.Milliseconds/100);
        }

        private void btnBrowseFiles_Click(object sender, EventArgs e)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            try
            {
                if (string.IsNullOrEmpty(txtOutlookPath.Text))
                    path = Path.GetDirectoryName(txtOutlookPath.Text);
            }
            catch (Exception)
            {
                //swallow
            }
            
            openFileDialog1.InitialDirectory = path;
            openFileDialog1.FileName = "Outlook.exe";
            openFileDialog1.Filter = "Outlook |outlook.exe";
            DialogResult result = openFileDialog1.ShowDialog();
            // OK button was pressed.
            if (result == DialogResult.OK)
            {
                txtOutlookPath.Text = openFileDialog1.FileName;
            }
        }
    }
}
