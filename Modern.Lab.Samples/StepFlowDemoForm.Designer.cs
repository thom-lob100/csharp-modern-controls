namespace Modern.Lab.Samples
{
    partial class StepFlowDemoForm
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
            this.cardStd = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.stepStd = new Modern.Lab.WinForms.Controls.Display.ModernStepIndicator();
            this.sp1 = new System.Windows.Forms.Panel();
            this.cardLong = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.stepLong = new Modern.Lab.WinForms.Controls.Display.ModernStepIndicator();
            this.sp2 = new System.Windows.Forms.Panel();
            this.cardVeryLong = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.stepVeryLong = new Modern.Lab.WinForms.Controls.Display.ModernStepIndicator();
            this.sp3 = new System.Windows.Forms.Panel();
            this.cardFailed = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.stepFailed = new Modern.Lab.WinForms.Controls.Display.ModernStepIndicator();
            this.sp4 = new System.Windows.Forms.Panel();
            this.cardResize = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.hostPanel = new System.Windows.Forms.Panel();
            this.stepResize = new Modern.Lab.WinForms.Controls.Display.ModernStepIndicator();
            this.trackWidth = new System.Windows.Forms.TrackBar();
            this.cardStd.SuspendLayout();
            this.cardLong.SuspendLayout();
            this.cardVeryLong.SuspendLayout();
            this.cardFailed.SuspendLayout();
            this.cardResize.SuspendLayout();
            this.hostPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackWidth)).BeginInit();
            this.SuspendLayout();
            //
            // cardStd
            //
            this.cardStd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardStd.Controls.Add(this.stepStd);
            this.cardStd.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardStd.Location = new System.Drawing.Point(12, 12);
            this.cardStd.Name = "cardStd";
            this.cardStd.Padding = new System.Windows.Forms.Padding(12, 38, 12, 8);
            this.cardStd.Size = new System.Drawing.Size(1516, 98);
            this.cardStd.TabIndex = 0;
            this.cardStd.Text = "Standard — 5 steps";
            //
            // stepStd
            //
            this.stepStd.DisplayMember = "LABEL";
            this.stepStd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stepStd.Location = new System.Drawing.Point(12, 38);
            this.stepStd.Name = "stepStd";
            this.stepStd.Size = new System.Drawing.Size(1492, 52);
            this.stepStd.StateMember = "STATE";
            this.stepStd.TabIndex = 0;
            this.stepStd.Child = null;
            //
            // sp1
            //
            this.sp1.Dock = System.Windows.Forms.DockStyle.Top;
            this.sp1.Location = new System.Drawing.Point(12, 110);
            this.sp1.Name = "sp1";
            this.sp1.Size = new System.Drawing.Size(1516, 8);
            this.sp1.TabIndex = 1;
            //
            // cardLong
            //
            this.cardLong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardLong.Controls.Add(this.stepLong);
            this.cardLong.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardLong.Location = new System.Drawing.Point(12, 118);
            this.cardLong.Name = "cardLong";
            this.cardLong.Padding = new System.Windows.Forms.Padding(12, 38, 12, 8);
            this.cardLong.Size = new System.Drawing.Size(1516, 98);
            this.cardLong.TabIndex = 2;
            this.cardLong.Text = "Long flow — 12 steps";
            //
            // stepLong
            //
            this.stepLong.DisplayMember = "LABEL";
            this.stepLong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stepLong.Location = new System.Drawing.Point(12, 38);
            this.stepLong.Name = "stepLong";
            this.stepLong.Size = new System.Drawing.Size(1492, 52);
            this.stepLong.StateMember = "STATE";
            this.stepLong.TabIndex = 0;
            this.stepLong.Child = null;
            //
            // sp2
            //
            this.sp2.Dock = System.Windows.Forms.DockStyle.Top;
            this.sp2.Location = new System.Drawing.Point(12, 216);
            this.sp2.Name = "sp2";
            this.sp2.Size = new System.Drawing.Size(1516, 8);
            this.sp2.TabIndex = 3;
            //
            // cardVeryLong
            //
            this.cardVeryLong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardVeryLong.Controls.Add(this.stepVeryLong);
            this.cardVeryLong.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardVeryLong.Location = new System.Drawing.Point(12, 224);
            this.cardVeryLong.Name = "cardVeryLong";
            this.cardVeryLong.Padding = new System.Windows.Forms.Padding(12, 38, 12, 8);
            this.cardVeryLong.Size = new System.Drawing.Size(1516, 98);
            this.cardVeryLong.TabIndex = 4;
            this.cardVeryLong.Text = "Very long flow — 24 steps";
            //
            // stepVeryLong
            //
            this.stepVeryLong.DisplayMember = "LABEL";
            this.stepVeryLong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stepVeryLong.Location = new System.Drawing.Point(12, 38);
            this.stepVeryLong.Name = "stepVeryLong";
            this.stepVeryLong.Size = new System.Drawing.Size(1492, 52);
            this.stepVeryLong.StateMember = "STATE";
            this.stepVeryLong.TabIndex = 0;
            this.stepVeryLong.Child = null;
            //
            // sp3
            //
            this.sp3.Dock = System.Windows.Forms.DockStyle.Top;
            this.sp3.Location = new System.Drawing.Point(12, 322);
            this.sp3.Name = "sp3";
            this.sp3.Size = new System.Drawing.Size(1516, 8);
            this.sp3.TabIndex = 5;
            //
            // cardFailed
            //
            this.cardFailed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardFailed.Controls.Add(this.stepFailed);
            this.cardFailed.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardFailed.Location = new System.Drawing.Point(12, 330);
            this.cardFailed.Name = "cardFailed";
            this.cardFailed.Padding = new System.Windows.Forms.Padding(12, 38, 12, 8);
            this.cardFailed.Size = new System.Drawing.Size(1516, 98);
            this.cardFailed.TabIndex = 6;
            this.cardFailed.Text = "Failed flow — 16 steps, failed at 11";
            //
            // stepFailed
            //
            this.stepFailed.DisplayMember = "LABEL";
            this.stepFailed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stepFailed.Location = new System.Drawing.Point(12, 38);
            this.stepFailed.Name = "stepFailed";
            this.stepFailed.Size = new System.Drawing.Size(1492, 52);
            this.stepFailed.StateMember = "STATE";
            this.stepFailed.TabIndex = 0;
            this.stepFailed.Child = null;
            //
            // sp4
            //
            this.sp4.Dock = System.Windows.Forms.DockStyle.Top;
            this.sp4.Location = new System.Drawing.Point(12, 428);
            this.sp4.Name = "sp4";
            this.sp4.Size = new System.Drawing.Size(1516, 8);
            this.sp4.TabIndex = 7;
            //
            // cardResize
            //
            this.cardResize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cardResize.Controls.Add(this.hostPanel);
            this.cardResize.Controls.Add(this.trackWidth);
            this.cardResize.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardResize.Location = new System.Drawing.Point(12, 436);
            this.cardResize.Name = "cardResize";
            this.cardResize.Padding = new System.Windows.Forms.Padding(12, 38, 12, 8);
            this.cardResize.Size = new System.Drawing.Size(1516, 184);
            this.cardResize.TabIndex = 8;
            this.cardResize.Text = "Width test — 14 steps, drag the slider";
            //
            // trackWidth
            //
            this.trackWidth.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackWidth.Location = new System.Drawing.Point(12, 38);
            this.trackWidth.Maximum = 1490;
            this.trackWidth.Minimum = 140;
            this.trackWidth.Name = "trackWidth";
            this.trackWidth.Size = new System.Drawing.Size(1492, 45);
            this.trackWidth.TabIndex = 0;
            this.trackWidth.TickFrequency = 100;
            this.trackWidth.Value = 1490;
            //
            // hostPanel
            //
            this.hostPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hostPanel.Controls.Add(this.stepResize);
            this.hostPanel.Location = new System.Drawing.Point(12, 92);
            this.hostPanel.Name = "hostPanel";
            this.hostPanel.Size = new System.Drawing.Size(1490, 78);
            this.hostPanel.TabIndex = 1;
            //
            // stepResize
            //
            this.stepResize.DisplayMember = "LABEL";
            this.stepResize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stepResize.Location = new System.Drawing.Point(0, 0);
            this.stepResize.Name = "stepResize";
            this.stepResize.Size = new System.Drawing.Size(1488, 76);
            this.stepResize.StateMember = "STATE";
            this.stepResize.TabIndex = 0;
            this.stepResize.Child = null;
            //
            // StepFlowDemoForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1540, 800);
            this.Controls.Add(this.cardResize);
            this.Controls.Add(this.sp4);
            this.Controls.Add(this.cardFailed);
            this.Controls.Add(this.sp3);
            this.Controls.Add(this.cardVeryLong);
            this.Controls.Add(this.sp2);
            this.Controls.Add(this.cardLong);
            this.Controls.Add(this.sp1);
            this.Controls.Add(this.cardStd);
            this.Name = "StepFlowDemoForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Text = "Step Indicator";
            this.cardStd.ResumeLayout(false);
            this.cardLong.ResumeLayout(false);
            this.cardVeryLong.ResumeLayout(false);
            this.cardFailed.ResumeLayout(false);
            this.cardResize.ResumeLayout(false);
            this.hostPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackWidth)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox cardStd;
        private Modern.Lab.WinForms.Controls.Display.ModernStepIndicator stepStd;
        private System.Windows.Forms.Panel sp1;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox cardLong;
        private Modern.Lab.WinForms.Controls.Display.ModernStepIndicator stepLong;
        private System.Windows.Forms.Panel sp2;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox cardVeryLong;
        private Modern.Lab.WinForms.Controls.Display.ModernStepIndicator stepVeryLong;
        private System.Windows.Forms.Panel sp3;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox cardFailed;
        private Modern.Lab.WinForms.Controls.Display.ModernStepIndicator stepFailed;
        private System.Windows.Forms.Panel sp4;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox cardResize;
        private System.Windows.Forms.TrackBar trackWidth;
        private System.Windows.Forms.Panel hostPanel;
        private Modern.Lab.WinForms.Controls.Display.ModernStepIndicator stepResize;
    }
}
