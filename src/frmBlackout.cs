using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OutlookReminder
{
    public partial class frmBlackout : Form
    {

        private Color _transparentColor = Color.Yellow;
        private List<frmBlackout> _otherScreens = new List<frmBlackout>();

        public frmBlackout(Rectangle bounds, bool maximize)
        {
            InitializeComponent();
            Opacity = 0.8D;
            BackColor = Color.Black;
            TransparencyKey = _transparentColor;
            
            this.Bounds = bounds;
            if (maximize)
                WindowState = FormWindowState.Maximized;//this will trigger maximization on the primary screen for any window, on any monitor
            this.TopLevel = true;
        }

        private void frmBlackout_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void ReminderClicked()
        {
            this.Close();
            foreach (var otherScreen in _otherScreens)
            {
                otherScreen.Close();
            }
        }

        public void AddScreen(frmBlackout form)
        {
            _otherScreens.Add(form);
            //hook up events...
            form.Click += frmBlackout_Click;
            this.Click += form.frmBlackout_Click;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            foreach (var otherScreen in _otherScreens)
            {
                otherScreen.Show();
            }
        }
    }
}
