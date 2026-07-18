namespace Modern.Lab.Samples
{
    public partial class ReceiveDialogForm
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
            this.radioMode = new Modern.Lab.WinForms.Controls.Selection.ModernRadioGroup();
            this.pnlChecked = new System.Windows.Forms.Panel();
            this.gridItems = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.pnlManual = new System.Windows.Forms.Panel();
            this.lblItemId = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.txtItemId = new Modern.Lab.WinForms.Controls.Input.ModernTextBox();
            this.lblSendFac = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboSendFac = new Modern.Lab.WinForms.Controls.Selection.ModernComboBox();
            this.lblHint = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.badgeResult = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.btnReceive = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnCancel = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.pnlChecked.SuspendLayout();
            this.pnlManual.SuspendLayout();
            this.SuspendLayout();
            //
            // radioMode
            //
            this.radioMode.Location = new System.Drawing.Point(16, 16);
            this.radioMode.Name = "radioMode";
            this.radioMode.Size = new System.Drawing.Size(408, 24);
            this.radioMode.TabIndex = 0;
            this.radioMode.SelectedValueChanged += new System.EventHandler(this.OnModeChanged);
            this.radioMode.Child = null;
            //
            // pnlChecked
            //
            this.pnlChecked.Controls.Add(this.gridItems);
            this.pnlChecked.Location = new System.Drawing.Point(16, 52);
            this.pnlChecked.Name = "pnlChecked";
            this.pnlChecked.Size = new System.Drawing.Size(408, 248);
            this.pnlChecked.TabIndex = 1;
            //
            // gridItems
            //
            this.gridItems.AutoFitColumns = true;
            this.gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItems.Location = new System.Drawing.Point(0, 0);
            this.gridItems.Name = "gridItems";
            this.gridItems.ShowStatusBar = true;
            this.gridItems.Size = new System.Drawing.Size(408, 248);
            this.gridItems.StatusCountFormat = "{0:N0} item(s) to receive";
            this.gridItems.TabIndex = 0;
            this.gridItems.Child = null;
            //
            // pnlManual
            //
            this.pnlManual.Controls.Add(this.lblItemId);
            this.pnlManual.Controls.Add(this.txtItemId);
            this.pnlManual.Controls.Add(this.lblSendFac);
            this.pnlManual.Controls.Add(this.cboSendFac);
            this.pnlManual.Controls.Add(this.lblHint);
            this.pnlManual.Location = new System.Drawing.Point(16, 52);
            this.pnlManual.Name = "pnlManual";
            this.pnlManual.Size = new System.Drawing.Size(408, 248);
            this.pnlManual.TabIndex = 2;
            this.pnlManual.Visible = false;
            //
            // lblItemId
            //
            this.lblItemId.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblItemId.Location = new System.Drawing.Point(0, 8);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Required = true;
            this.lblItemId.Size = new System.Drawing.Size(84, 32);
            this.lblItemId.TabIndex = 0;
            this.lblItemId.Text = "Item ID";
            this.lblItemId.Child = null;
            //
            // txtItemId
            //
            this.txtItemId.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtItemId.Location = new System.Drawing.Point(92, 8);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.PlaceholderText = "Item ID not on the board";
            this.txtItemId.Size = new System.Drawing.Size(300, 32);
            this.txtItemId.TabIndex = 1;
            this.txtItemId.TextChanged += new System.EventHandler(this.OnInputChanged);
            this.txtItemId.Child = null;
            //
            // lblSendFac
            //
            this.lblSendFac.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblSendFac.Location = new System.Drawing.Point(0, 48);
            this.lblSendFac.Name = "lblSendFac";
            this.lblSendFac.Required = true;
            this.lblSendFac.Size = new System.Drawing.Size(84, 32);
            this.lblSendFac.TabIndex = 2;
            this.lblSendFac.Text = "Send Fac";
            this.lblSendFac.Child = null;
            //
            // cboSendFac
            //
            this.cboSendFac.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSendFac.Location = new System.Drawing.Point(92, 48);
            this.cboSendFac.Name = "cboSendFac";
            this.cboSendFac.Size = new System.Drawing.Size(160, 32);
            this.cboSendFac.TabIndex = 3;
            this.cboSendFac.SelectedIndexChanged += new System.EventHandler(this.OnInputChanged);
            this.cboSendFac.Child = null;
            //
            // lblHint
            //
            this.lblHint.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Helper;
            this.lblHint.Location = new System.Drawing.Point(0, 88);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(392, 20);
            this.lblHint.TabIndex = 4;
            this.lblHint.Text = "Force receive for an item that does not show up on the board.";
            this.lblHint.Child = null;
            //
            // badgeResult
            //
            this.badgeResult.Color = "#FEE2E2";
            this.badgeResult.Location = new System.Drawing.Point(16, 308);
            this.badgeResult.Name = "badgeResult";
            this.badgeResult.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeResult.Size = new System.Drawing.Size(408, 26);
            this.badgeResult.TabIndex = 3;
            this.badgeResult.Text = "-";
            this.badgeResult.Visible = false;
            this.badgeResult.Child = null;
            //
            // btnReceive
            //
            this.btnReceive.Location = new System.Drawing.Point(240, 348);
            this.btnReceive.Name = "btnReceive";
            this.btnReceive.Size = new System.Drawing.Size(88, 32);
            this.btnReceive.TabIndex = 4;
            this.btnReceive.Text = "Receive";
            this.btnReceive.Click += new System.EventHandler(this.OnReceiveClick);
            this.btnReceive.Child = null;
            //
            // btnCancel
            //
            this.btnCancel.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Subtle;
            this.btnCancel.Location = new System.Drawing.Point(336, 348);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
            this.btnCancel.Child = null;
            //
            // ReceiveDialogForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(440, 396);
            this.Controls.Add(this.radioMode);
            this.Controls.Add(this.pnlChecked);
            this.Controls.Add(this.pnlManual);
            this.Controls.Add(this.badgeResult);
            this.Controls.Add(this.btnReceive);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReceiveDialogForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Receive";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.pnlChecked.ResumeLayout(false);
            this.pnlManual.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Modern.Lab.WinForms.Controls.Selection.ModernRadioGroup radioMode;
        private System.Windows.Forms.Panel pnlChecked;
        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridItems;
        private System.Windows.Forms.Panel pnlManual;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblItemId;
        private Modern.Lab.WinForms.Controls.Input.ModernTextBox txtItemId;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblSendFac;
        private Modern.Lab.WinForms.Controls.Selection.ModernComboBox cboSendFac;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblHint;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeResult;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnReceive;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnCancel;
    }
}
