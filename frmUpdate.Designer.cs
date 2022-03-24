namespace OutlookReminder
{
    partial class frmUpdate
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
            OutlookReminder.ColorLinearGradient colorLinearGradient1 = new OutlookReminder.ColorLinearGradient();
            OutlookReminder.ColorLinearGradient colorLinearGradient2 = new OutlookReminder.ColorLinearGradient();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar2 = new OutlookReminder.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(251, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 1;
            this.label1.UseWaitCursor = true;
            // 
            // progressBar2
            // 
            this.progressBar2.BackColor = System.Drawing.Color.Transparent;
            this.progressBar2.BorderShow = true;
            this.progressBar2.BrushStyle = OutlookReminder.ProgressBar.eBrushStyle.Linear2;
            this.progressBar2.FloatValue = false;
            this.progressBar2.Label = null;
            this.progressBar2.Location = new System.Drawing.Point(24, 94);
            this.progressBar2.MaxValue = 100;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.ShowFocus = false;
            this.progressBar2.Size = new System.Drawing.Size(224, 22);
            this.progressBar2.SliderCapEnd = System.Drawing.Drawing2D.LineCap.Flat;
            this.progressBar2.SliderCapStart = System.Drawing.Drawing2D.LineCap.Flat;
            colorLinearGradient1.ColorA = System.Drawing.Color.White;
            colorLinearGradient1.ColorB = System.Drawing.Color.Transparent;
            this.progressBar2.SliderColorHigh = colorLinearGradient1;
            colorLinearGradient2.ColorA = System.Drawing.Color.Black;
            colorLinearGradient2.ColorB = System.Drawing.Color.DarkBlue;
            this.progressBar2.SliderColorLow = colorLinearGradient2;
            this.progressBar2.SliderShape = OutlookReminder.ProgressBar.eShape.Rectangle;
            this.progressBar2.SliderSize = new System.Drawing.Size(1, 22);
            this.progressBar2.SliderWidthHigh = 20F;
            this.progressBar2.SliderWidthLow = 21F;
            this.progressBar2.TabIndex = 0;
            this.progressBar2.TickThickness = 1F;
            this.progressBar2.UpDownShow = false;
            this.progressBar2.UseWaitCursor = true;
            this.progressBar2.Value = 50;
            this.progressBar2.ValueAdjusted = 50F;
            this.progressBar2.ValueDivisor = OutlookReminder.ProgressBar.eValueDivisor.e1;
            this.progressBar2.ValueStrFormat = null;
            // 
            // frmUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.BackgroundImage = global::OutlookReminder.Properties.Resources.Updating;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(414, 148);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmUpdate";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.frmUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private ProgressBar progressBar2;
    }
}