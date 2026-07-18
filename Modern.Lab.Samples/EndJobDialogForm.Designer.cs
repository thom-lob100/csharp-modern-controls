namespace Modern.Lab.Samples
{
    public partial class EndJobDialogForm
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
            this.gridSlots = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.badgeWarn = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.btnEnd = new Modern.Lab.WinForms.Controls.Input.ModernButton();
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
            // gridSlots
            //
            // 판정 콤보가 열리는 그리드라 컬럼 필터는 끈다 (짧은 고정 목록).
            this.gridSlots.AllowColumnFilters = false;
            this.gridSlots.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridSlots.AutoFitColumns = true;
            this.gridSlots.Location = new System.Drawing.Point(16, 80);
            this.gridSlots.Name = "gridSlots";
            this.gridSlots.Size = new System.Drawing.Size(728, 260);
            this.gridSlots.TabIndex = 4;
            this.gridSlots.Child = null;
            //
            // badgeWarn
            //
            // 확정 검증 경고 — 웨이퍼 슬롯에 미입력 판정이 있으면 표시된다.
            this.badgeWarn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.badgeWarn.Color = "#FEE2E2";
            this.badgeWarn.Location = new System.Drawing.Point(16, 356);
            this.badgeWarn.Name = "badgeWarn";
            this.badgeWarn.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeWarn.Size = new System.Drawing.Size(300, 26);
            this.badgeWarn.TabIndex = 5;
            this.badgeWarn.Text = "";
            this.badgeWarn.Visible = false;
            this.badgeWarn.Child = null;
            //
            // btnEnd
            //
            this.btnEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnd.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Execute;
            this.btnEnd.Location = new System.Drawing.Point(560, 352);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(88, 32);
            this.btnEnd.TabIndex = 6;
            this.btnEnd.Text = "End";
            this.btnEnd.Click += new System.EventHandler(this.OnEndClick);
            this.btnEnd.Child = null;
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Subtle;
            this.btnCancel.Location = new System.Drawing.Point(656, 352);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
            this.btnCancel.Child = null;
            //
            // EndJobDialogForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(760, 400);
            this.Controls.Add(this.lblEqpCaption);
            this.Controls.Add(this.lblEqpValue);
            this.Controls.Add(this.lblLotCaption);
            this.Controls.Add(this.lblLotValue);
            this.Controls.Add(this.gridSlots);
            this.Controls.Add(this.badgeWarn);
            this.Controls.Add(this.btnEnd);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EndJobDialogForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Job End";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblEqpCaption;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblEqpValue;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblLotCaption;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblLotValue;
        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridSlots;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeWarn;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnEnd;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnCancel;
    }
}
