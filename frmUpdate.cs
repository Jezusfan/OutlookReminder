using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Threading;
using System.Windows.Forms;

namespace OutlookReminder
{
    public partial class frmUpdate : Form
    {
        public frmUpdate()
        {
            InitializeComponent();

        }

        public void ReportProgress(object sender, DeploymentProgressChangedEventArgs e)
        {
            if (InvokeRequired) Invoke(new Action<object, DeploymentProgressChangedEventArgs>(ReportProgress), sender, e);
            else
            {
                HandleProgressChanged(e.ProgressPercentage, e.BytesCompleted, e.BytesTotal);
            }

        }

        private void HandleProgressChanged(int percentage, long bytescompleted, long bytestotal)
        {
            progressBar2.Value = percentage;
            label1.Text = (bytescompleted/ 1024) + " of " + (bytestotal/1024) + " kb downloaded";
        }

        public void UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.DoEvents();
            Thread.Sleep(1000);
            Close();
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            //mark next startup as if it's the first only when there's a new property that matters
            //Properties.Settings.Default.FirstTime = true;
            //Properties.Settings.Default.Save();
            Application.DoEvents();
            try
            {
                ApplicationDeployment.CurrentDeployment.UpdateProgressChanged += ReportProgress;
                ApplicationDeployment.CurrentDeployment.UpdateCompleted += UpdateCompleted;
                ApplicationDeployment.CurrentDeployment.UpdateAsync();
            }
            catch (Exception)
            {

            }
        }


    }
}
