using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using OutlookReminder.Properties;

namespace OutlookReminder
{
    internal class OutlookMonitor : IDisposable
    {
        private static DateTime _startupTime = DateTime.Now;
        private object Application;
        private int _outlookProcessID;
        private Timer _timer;
        private WindowHook _windowHook;
        private IntPtr _reminderWindow = IntPtr.Zero;
        private bool? _outlookFound = false;
        private DateTime? _outlookStart;
        private Exception _failedReason;
        private bool _credentialmismatch;
        private bool _manualShutdownOutlookDetected;
        private bool _disposing;

        public event Action<IntPtr> ReminderShown;
        public event Action ReminderClosed;
        private event Action _connectedStateChanged;

        public event Action ConnectedStateChanged
        {
            add
            {
                _connectedStateChanged += value;
                _connectedStateChanged();
            }
            remove { _connectedStateChanged -= value; }
        }

        public OutlookMonitor()
        {
            int interval = TimeSpan.FromSeconds(Settings.Default.OutlookPollingProcessInterval).TotalMilliseconds.ToInt32();
            _timer = new Timer(OnTimerTick, null, interval, interval);
            OnTimerTick(null);
            if (string.IsNullOrWhiteSpace(Settings.Default.OutlookPath))
            {
                SearchForOutlook();
            }
            else
            {
                Log.Write(EventLogEntryType.Information, $"Outlook executable is located in: '{Settings.Default.OutlookPath}'");
            }
        }

        public bool CredentialMismatch
        {
            get { return _credentialmismatch; }
        }

        public bool OutlookProcessFound
        {
            get
            {
                return _outlookFound.HasValue && _outlookFound.Value;
            }
        }

        public bool OutlookVersionSupported
        {
            get
            {
                return _failedReason == null;
            }
        }

        public string UnsupportedReason
        {
            get
            {
                return _failedReason == null ? string.Empty : _failedReason.Message;
            }
        }

        
        public bool ShowSettingAllowed
        {
            get
            {
                

                var secondsRunning = DateTime.Now.Subtract(_startupTime).TotalSeconds;
                if (Settings.Default.StartOutlook)
                { 
                    if (!_outlookStart.HasValue)
                        return false;
                    secondsRunning = DateTime.Now.Subtract(_outlookStart.Value).TotalSeconds;
                    secondsRunning -=Convert.ToDouble(Settings.Default.StartupDelaySec);
                }
                return secondsRunning > 60;
            }
        }

        private void OnTimerTick(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            var startOutlookDelay = false;
            try
            {
                // Check whether there is an Outlook process running.
                var pp = Process.GetProcessesByName("OUTLOOK");
                if (pp.Any())
                {
                    _outlookFound = true;
                    var process = pp.First();
                    if (_outlookProcessID != process.Id)
                    {
                        _reminderWindow = IntPtr.Zero;
                        _outlookProcessID = process.Id;
                        _credentialmismatch = false;
                        if (_windowHook != null)
                            _windowHook.Dispose();
                        _windowHook = new WindowHook(_outlookProcessID, OnReminderClosed);

                        Debug.WriteLine("Waiting for Marshal.GetActiveObject");
                        try
                        {
                            // This FAILS while Outlook is the active application.
                            // As soon as you hit ALT-TAB or click another app, this succeeds.
                            //// If so, use the GetActiveObject method to obtain the process and cast it to an Application object.
                            Application = System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application");
                            Debug.WriteLine("SUCCESS");
                            if (Application != null)
                            {
                                if (_outlookStart == null)
                                    _outlookStart = DateTime.Now;
                                WireUp(Application, "Reminder", nameof(Application_Reminder));
                                OnConnectedChanged(null);
                                Log.Write(EventLogEntryType.Information, "Outlook-process found with processid: {0}", _outlookProcessID);
                            }
                            else
                            {
                                OnConnectedChanged(new Exception("Outlook.Application does not seem to be a registered COM object."));
                            }
                        }
                        catch (System.Runtime.InteropServices.COMException exp)
                        {
                            _outlookProcessID = 0;
                            var outlookProcess = GetProcessInfo(process);
                            var thisProcess = GetProcessInfo(Process.GetCurrentProcess());
                            Exception ex;
                            if (outlookProcess != thisProcess)
                            {
                                _credentialmismatch = true;
                                ex = new Exception(string.Format("Outlook is running using credentials: {0}{1}Outlook reminder is running using credentials: {2}{1}Make sure Outlook Reminder is running with similar rights!", outlookProcess, Environment.NewLine, thisProcess), exp);
                                OnConnectedChanged(ex);
                            }
                            else
                            {
                                ex = new Exception("COM exception occured trying to connect to Outlook.", exp);
                            }
                            if (_failedReason == null || _failedReason.InnerException?.Message != ex.InnerException?.Message)
                                Log.Write(EventLogEntryType.Error, exp.ToString());
                            OnConnectedChanged(ex);
                        }
                        catch (Exception ex)
                        {
                            Log.Write(EventLogEntryType.Error, ex.ToString());
                            OnConnectedChanged(ex);
                        }
                    }
                    else
                    {
                        OnConnectedChanged(null);
                    }
                }
                else
                {
                    _outlookFound = false;
                    if (Settings.Default.StartOutlook && !string.IsNullOrEmpty(Settings.Default.OutlookPath))
                    {
                        if (_outlookStart != null)
                        {
                            _manualShutdownOutlookDetected = true;
                        }
                        if (!_manualShutdownOutlookDetected)
                        {
                            if (!_outlookStart.HasValue)
                            {
                                string outlookProcess = Settings.Default.OutlookPath;
                                if (!File.Exists(outlookProcess))
                                {
                                    OnConnectedChanged(new Exception($"Outlook process could not be found at: {outlookProcess}!"));
                                }
                                else
                                {
                                    if (DateTime.Now.Subtract(TimeSpan.FromSeconds(Convert.ToDouble(Settings.Default.StartupDelaySec))) > _startupTime)
                                    {
                                        _outlookProcessID = 0;
                                        _outlookStart = DateTime.Now;
                                        if (!_disposing)
                                        {
                                            Process.Start(outlookProcess);
                                            startOutlookDelay = true;
                                            OnConnectedChanged(new Exception("Outlook process has just been started by Outlook Reminder..."));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                OnConnectedChanged(new Exception("Outlook process has not been found...preparing to auto start Outlook"));
                            }
                        }
                        else
                        {
                            //auto start should not happen if user shutdown Outlook on purpose...
                            OnConnectedChanged(new Exception("Outlook process could not be found. Is it actually running?"));
                        }
                    }
                    else
                    {
                        OnConnectedChanged(new Exception("Outlook process could not be found. Is it actually running?"));
                    }
                }
            }
            finally
            {
                int interval = Settings.Default.OutlookPollingProcessInterval;
                if (!startOutlookDelay && !OutlookProcessFound || !OutlookVersionSupported)
                    interval = 1;
                int intervalMs = TimeSpan.FromSeconds(interval).TotalMilliseconds.ToInt32();
                if (!_disposing)
                    _timer.Change(intervalMs, intervalMs);
            }

        }

        void OnConnectedChanged(Exception ex)
        {
            _failedReason = ex;
            if (_connectedStateChanged != null)
                _connectedStateChanged();
        }

        private void OnReminderClosed()
        {
            if (ReminderClosed != null) ReminderClosed();
        }

        void OnReminderShown()
        {
            if (ReminderShown != null) ReminderShown(_reminderWindow);
        }

        void WireUp(object o, string eventname, string methodname)
        {
            EventInfo ei = o.GetType().GetEvent(eventname);

            MethodInfo mi = this.GetType().GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            Delegate del = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);

            ei.AddEventHandler(o, del);
        }
        
        private void SearchForOutlook()
        {
            Log.Write(EventLogEntryType.Information, "Searching for Outlook executable...");
            var mainFolders = new[] { Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolder.CommonApplicationData};
            var lastPath = string.Empty;
            foreach (var specialFolder in mainFolders)
            {
                try
                {
                    var path = Environment.GetFolderPath(specialFolder);
                    if (path == lastPath)
                        path = path.Replace(" (x86)", string.Empty);
                    lastPath = path;
                    Log.Write(EventLogEntryType.Information, $"Searching for Outlook executable in: {path}");
                    foreach (var subfolder in Directory.GetDirectories(path,"*", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            foreach (var outlookPath in Directory.GetFiles(subfolder,"outlook.exe", SearchOption.AllDirectories))
                            {
                                Log.Write(EventLogEntryType.Information, $"Found Outlook executable in: {outlookPath}");
                                if (outlookPath.IndexOf("download", StringComparison.InvariantCultureIgnoreCase) == -1)
                                {
                                    try
                                    {
                                        Log.Write(EventLogEntryType.Information, $"Saving Outlook executable: {outlookPath}");
                                        Settings.Default.OutlookPath = outlookPath;
                                        Settings.Default.Save();
                                        return;
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Write(EventLogEntryType.FailureAudit, $"Failed saving Outlook executable location {outlookPath}. reason: {e.Message}");
                                    }
                                }
                            }
                            Log.Write(EventLogEntryType.Information, $"Failed to find Outlook executable in: {subfolder}");
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Log.Write(EventLogEntryType.FailureAudit, $"Failed searching for Outlook executable in: {subfolder}. reason: {e.Message}");
                        }
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Log.Write(EventLogEntryType.FailureAudit, $"Failed searching for Outlook executable in: {specialFolder}. reason: {e.Message}");
                }
                
            }
            Log.Write(EventLogEntryType.Information, $"Finished searching for Outlook executable");
        }


        private void Application_Reminder(dynamic item)
        {
            var existingWindows = _windowHook.GetChildWindows();
            Task.Factory.StartNew(() =>
            {
                //wait for a bit to let outlook show its reminder window
                int reminderFound = 0;
                do
                {
                    var newWindows = _windowHook.GetChildWindows();
                    IntPtr windowsHandle = IntPtr.Zero; ;
                    if (_reminderWindow == IntPtr.Zero && newWindows.Count > existingWindows.Count)
                    {
                        if (newWindows.Count == existingWindows.Count + 1)
                        {
                            windowsHandle = newWindows.First(w => !existingWindows.Any(e => e.Handle == w.Handle)).Handle;
                            _windowHook.AddWindowCloseHandler(windowsHandle);
                        }
                        else
                        {
                            foreach (var windowInfo in newWindows)
                            {
                                var explorer = item.Application.ActiveExplorer();
                                string caption = explorer.Caption;
                                if (caption != windowInfo.Caption)
                                {
                                    Console.WriteLine(windowInfo.Caption);
                                    //var r = windowInfo.Rectangle;
                                    //Point p = new Point(r.Left, r.Top);
                                    //Size s = new Size(r.Right - r.Left, r.Bottom - r.Top);
                                    if (windowInfo.Caption.ToLower().Contains("reminder"))
                                    {
                                        windowsHandle = windowInfo.Handle;
                                        _windowHook.AddWindowCloseHandler(windowsHandle);
                                    }
                                }
                            }
                        }
                    }
                    else if (_reminderWindow != IntPtr.Zero && newWindows.Any(w => w.Handle == _reminderWindow))
                    {
                        windowsHandle = _reminderWindow;
                        _windowHook.AddWindowCloseHandler(windowsHandle);
                    }
                    if (windowsHandle != IntPtr.Zero)
                    {
                        _reminderWindow = windowsHandle;
                        reminderFound = 10;
                        OnReminderShown();
                    }
                    reminderFound++;
                    if (reminderFound < 10)
                        Thread.Sleep(50);
                } while (reminderFound < 10);
            });
        }

        private string IsElevated(IntPtr processHandle)
        {
            var elevated = ProcessInfo.IsProcessElevated(processHandle);
            return elevated == null ? "unknown" : elevated.Value.ToString();
        }

        private string GetProcessInfo(Process process)
        {
            string stringSID = String.Empty;
            string s = ProcessInfo.ExGetProcessInfoByPID(process, out stringSID);
            string account = new SecurityIdentifier(stringSID).Translate(typeof(NTAccount)).ToString();

            return string.Format("{0}, elevated: {1}", account, IsElevated(process.Handle));
        }

        public void Dispose()
        {
            Dispose(true);
        }



        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposing = true;
                // get rid of managed resources
                _timer.Dispose();
                if (_windowHook != null)
                    _windowHook.Dispose();
            }
            // get rid of unmanaged resources
        }

    }
}
