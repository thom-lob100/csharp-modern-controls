namespace Modern.Lab.Samples
{
    public partial class PrepareDialogForm
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.lblEqpCaption = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.lblEqpValue = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.lblLotCaption = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.lblLotValue = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.lblInPort = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboInPort = new Modern.Lab.WinForms.Controls.Selection.ModernComboBox();
            this.lblOutPort = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboOutPort = new Modern.Lab.WinForms.Controls.Selection.ModernComboBox();
            this.lblCarrier = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboCarrier = new Modern.Lab.WinForms.Controls.Selection.ModernComboBox();
            this.btnPrepare = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnCancel = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.SuspendLayout();
            //
            // lblEqpCaption
            //
            this.lblEqpCaption.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblEqpCaption.Location = new System.Drawing.Point(16, 16);
            this.lblEqpCaption.Name = "lblEqpCaption";
            this.lblEqpCaption.Size = new System.Drawing.Size(84, 24);
            this.lblEqpCaption.TabIndex = 0;
            this.lblEqpCaption.Text = "Equipment";
            this.lblEqpCaption.Child = null;
            //
            // lblEqpValue
            //
            this.lblEqpValue.Location = new System.Drawing.Point(108, 16);
            this.lblEqpValue.Name = "lblEqpValue";
            this.lblEqpValue.Size = new System.Drawing.Size(220, 24);
            this.lblEqpValue.TabIndex = 1;
            this.lblEqpValue.Text = "-";
            this.lblEqpValue.Child = null;
            //
            // lblLotCaption
            //
            this.lblLotCaption.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblLotCaption.Location = new System.Drawing.Point(16, 44);
            this.lblLotCaption.Name = "lblLotCaption";
            this.lblLotCaption.Size = new System.Drawing.Size(84, 24);
            this.lblLotCaption.TabIndex = 2;
            this.lblLotCaption.Text = "Lot";
            this.lblLotCaption.Child = null;
            //
            // lblLotValue
            //
            this.lblLotValue.Location = new System.Drawing.Point(108, 44);
            this.lblLotValue.Name = "lblLotValue";
            this.lblLotValue.Size = new System.Drawing.Size(220, 24);
            this.lblLotValue.TabIndex = 3;
            this.lblLotValue.Text = "-";
            this.lblLotValue.Child = null;
            //
            // lblInPort
            //
            this.lblInPort.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblInPort.Location = new System.Drawing.Point(16, 80);
            this.lblInPort.Name = "lblInPort";
            this.lblInPort.Required = true;
            this.lblInPort.Size = new System.Drawing.Size(84, 32);
            this.lblInPort.TabIndex = 4;
            this.lblInPort.Text = "In Port";
            this.lblInPort.Child = null;
            //
            // cboInPort
            //
            this.cboInPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInPort.Location = new System.Drawing.Point(108, 80);
            this.cboInPort.Name = "cboInPort";
            this.cboInPort.Size = new System.Drawing.Size(140, 32);
            this.cboInPort.TabIndex = 5;
            this.cboInPort.Child = null;
            //
            // lblOutPort
            //
            this.lblOutPort.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblOutPort.Location = new System.Drawing.Point(16, 120);
            this.lblOutPort.Name = "lblOutPort";
            this.lblOutPort.Required = true;
            this.lblOutPort.Size = new System.Drawing.Size(84, 32);
            this.lblOutPort.TabIndex = 6;
            this.lblOutPort.Text = "Out Port";
            this.lblOutPort.Child = null;
            //
            // cboOutPort
            //
            this.cboOutPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutPort.Location = new System.Drawing.Point(108, 120);
            this.cboOutPort.Name = "cboOutPort";
            this.cboOutPort.Size = new System.Drawing.Size(140, 32);
            this.cboOutPort.TabIndex = 7;
            this.cboOutPort.Child = null;
            //
            // lblCarrier
            //
            this.lblCarrier.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblCarrier.Location = new System.Drawing.Point(16, 160);
            this.lblCarrier.Name = "lblCarrier";
            this.lblCarrier.Required = true;
            this.lblCarrier.Size = new System.Drawing.Size(84, 32);
            this.lblCarrier.TabIndex = 8;
            this.lblCarrier.Text = "Out Carrier";
            this.lblCarrier.Child = null;
            //
            // cboCarrier
            //
            // 검색형(기본 DropDown) — 입력하면 빈 캐리어 목록이 필터링된다.
            // 핵심 배정 필드라 Highlight(액센트 테두리)로 주목을 준다.
            this.cboCarrier.Highlight = true;
            this.cboCarrier.Location = new System.Drawing.Point(108, 160);
            this.cboCarrier.Name = "cboCarrier";
            this.cboCarrier.PlaceholderText = "Type to search";
            this.cboCarrier.Size = new System.Drawing.Size(140, 32);
            this.cboCarrier.TabIndex = 9;
            this.cboCarrier.Child = null;
            //
            // btnPrepare
            //
            this.btnPrepare.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Execute;
            this.btnPrepare.Location = new System.Drawing.Point(152, 208);
            this.btnPrepare.Name = "btnPrepare";
            this.btnPrepare.Size = new System.Drawing.Size(88, 32);
            this.btnPrepare.TabIndex = 10;
            this.btnPrepare.Text = "Prepare";
            this.btnPrepare.Click += new System.EventHandler(this.OnPrepareClick);
            this.btnPrepare.Child = null;
            //
            // btnCancel
            //
            this.btnCancel.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Subtle;
            this.btnCancel.Location = new System.Drawing.Point(248, 208);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
            this.btnCancel.Child = null;
            //
            // PrepareDialogForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(352, 256);
            this.Controls.Add(this.lblEqpCaption);
            this.Controls.Add(this.lblEqpValue);
            this.Controls.Add(this.lblLotCaption);
            this.Controls.Add(this.lblLotValue);
            this.Controls.Add(this.lblInPort);
            this.Controls.Add(this.cboInPort);
            this.Controls.Add(this.lblOutPort);
            this.Controls.Add(this.cboOutPort);
            this.Controls.Add(this.lblCarrier);
            this.Controls.Add(this.cboCarrier);
            this.Controls.Add(this.btnPrepare);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrepareDialogForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Job Prepare";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblEqpCaption;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblEqpValue;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblLotCaption;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblLotValue;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblInPort;
        private Modern.Lab.WinForms.Controls.Selection.ModernComboBox cboInPort;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblOutPort;
        private Modern.Lab.WinForms.Controls.Selection.ModernComboBox cboOutPort;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblCarrier;
        private Modern.Lab.WinForms.Controls.Selection.ModernComboBox cboCarrier;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnPrepare;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnCancel;
    }
}
