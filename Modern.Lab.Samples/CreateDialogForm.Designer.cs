namespace Modern.Lab.Samples
{
    public partial class CreateDialogForm
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
            this.gridItems = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.btnCreate = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnCancel = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.SuspendLayout();
            //
            // gridItems
            //
            this.gridItems.AutoFitColumns = true;
            this.gridItems.Location = new System.Drawing.Point(16, 16);
            this.gridItems.Name = "gridItems";
            this.gridItems.ShowStatusBar = true;
            this.gridItems.Size = new System.Drawing.Size(408, 264);
            this.gridItems.StatusCountFormat = "{0:N0} item(s) to create";
            this.gridItems.TabIndex = 0;
            this.gridItems.Child = null;
            //
            // btnCreate
            //
            this.btnCreate.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Execute;
            this.btnCreate.Location = new System.Drawing.Point(240, 292);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(88, 32);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Create";
            this.btnCreate.Click += new System.EventHandler(this.OnCreateClick);
            this.btnCreate.Child = null;
            //
            // btnCancel
            //
            this.btnCancel.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Subtle;
            this.btnCancel.Location = new System.Drawing.Point(336, 292);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
            this.btnCancel.Child = null;
            //
            // CreateDialogForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(440, 340);
            this.Controls.Add(this.gridItems);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateDialogForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridItems;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnCreate;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnCancel;
    }
}
