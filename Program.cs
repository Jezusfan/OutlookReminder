using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace OutlookReminder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainOnUnhandledException;
            if (!AllreadyRunning(1))
            {
                Log.Write(EventLogEntryType.Information, "Started, checking for update");
                frmUpdate splash;
                bool flag = true;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.DoEvents();
                try
                {
                    if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.CheckForUpdate())
                    {
                        Log.Write(EventLogEntryType.Information, "Update found, initializing update process");
                        splash = new frmUpdate();
                        flag = false;
                        Application.Run(splash);
                        Log.Write(EventLogEntryType.Information, "Update installed, restarting application");
                        Application.Restart();
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(EventLogEntryType.Error, ex.ToString());
                    MessageBox.Show(ex.Message);
                    flag = false;
                }
                if (flag)
                {
                    try
                    {
                        Log.Write(EventLogEntryType.Information, "No update found, resume startup");
                        Application.Run(new frmSettings());
                    }
                    catch (Exception ex)
                    {
                        Log.Write(EventLogEntryType.Error, ex.ToString());
                        MessageBox.Show(ex.Message);
                    }
                    
                }
            }
            else
                Log.Write(EventLogEntryType.Information, "Attemted to start another instance");
        }

        private static bool AllreadyRunning(int @try)
        {
            if (@try > 4)
            {
                return true;
            }
            Process currentProcess = Process.GetCurrentProcess();
            foreach (Process process2 in Process.GetProcesses())
            {
                if ((string.Compare(process2.ProcessName, currentProcess.ProcessName) == 0) && (process2.Id != currentProcess.Id))
                {
                    Thread.Sleep(200);
                    @try++;
                    if (!AllreadyRunning(@try))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
       
        private static void AppDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            MessageBox.Show(unhandledExceptionEventArgs.ExceptionObject.ToString());
        }
    }
}
