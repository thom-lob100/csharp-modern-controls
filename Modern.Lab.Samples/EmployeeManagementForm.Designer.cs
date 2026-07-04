namespace Modern.Lab.Samples
{
    public partial class EmployeeManagementForm
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
            this.lblPageTitle = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.spacerTitleGap = new System.Windows.Forms.Panel();
            this.searchCard = new Modern.Lab.WinForms.Controls.Layout.ModernCardPanel();
            this.lblName = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.txtName = new Modern.Lab.WinForms.Controls.Input.ModernTextBox();
            this.lblDept = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboDept = new Modern.Lab.WinForms.Controls.Selection.ModernComboBox();
            this.lblRank = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboRank = new Modern.Lab.WinForms.Controls.Selection.ModernCheckComboBox();
            this.lblHireDate = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.dtHireFrom = new Modern.Lab.WinForms.Controls.Input.ModernDatePicker();
            this.lblHireTilde = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.dtHireTo = new Modern.Lab.WinForms.Controls.Input.ModernDatePicker();
            this.btnSearch = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnReset = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.gridEmployee = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.spacerSearchGap = new System.Windows.Forms.Panel();
            this.spacerBottomGap = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.countCard = new Modern.Lab.WinForms.Controls.Layout.ModernCardPanel();
            this.spacerCountGap = new System.Windows.Forms.Panel();
            this.statsCard = new Modern.Lab.WinForms.Controls.Layout.ModernCardPanel();
            this.spacerBtnGap = new System.Windows.Forms.Panel();
            this.buttonsCard = new Modern.Lab.WinForms.Controls.Layout.ModernCardPanel();
            this.cardCount = new Modern.Lab.WinForms.Controls.Display.ModernKpiCard();
            this.listDeptCount = new Modern.Lab.WinForms.Controls.Display.ModernSummaryList();
            this.listRankCount = new Modern.Lab.WinForms.Controls.Display.ModernSummaryList();
            this.btnExecute = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnNew = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnSave = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnDelete = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.btnExcel = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.searchCard.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.countCard.SuspendLayout();
            this.statsCard.SuspendLayout();
            this.buttonsCard.SuspendLayout();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPageTitle.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Title;
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(936, 28);
            this.lblPageTitle.TabIndex = 5;
            this.lblPageTitle.Text = "직원관리";
            this.lblPageTitle.TitleBar = true;
            this.lblPageTitle.Child = null;
            //
            // spacerTitleGap
            //
            this.spacerTitleGap.Dock = System.Windows.Forms.DockStyle.Top;
            this.spacerTitleGap.Name = "spacerTitleGap";
            this.spacerTitleGap.Size = new System.Drawing.Size(936, 8);
            this.spacerTitleGap.TabIndex = 6;
            //
            // searchCard
            //
            this.searchCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.searchCard.Controls.Add(this.lblName);
            this.searchCard.Controls.Add(this.txtName);
            this.searchCard.Controls.Add(this.lblDept);
            this.searchCard.Controls.Add(this.cboDept);
            this.searchCard.Controls.Add(this.lblRank);
            this.searchCard.Controls.Add(this.cboRank);
            this.searchCard.Controls.Add(this.lblHireDate);
            this.searchCard.Controls.Add(this.dtHireFrom);
            this.searchCard.Controls.Add(this.lblHireTilde);
            this.searchCard.Controls.Add(this.dtHireTo);
            this.searchCard.Controls.Add(this.btnSearch);
            this.searchCard.Controls.Add(this.btnReset);
            this.searchCard.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchCard.Name = "searchCard";
            this.searchCard.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.searchCard.Size = new System.Drawing.Size(936, 100);
            this.searchCard.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblName.Location = new System.Drawing.Point(12, 12);
            this.lblName.Name = "lblName";
            this.lblName.Required = true;
            this.lblName.Size = new System.Drawing.Size(40, 32);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "이름";
            this.lblName.Child = null;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(56, 12);
            this.txtName.Name = "txtName";
            this.txtName.PlaceholderText = "이름 검색";
            this.txtName.Required = true;
            this.txtName.Size = new System.Drawing.Size(160, 32);
            this.txtName.TabIndex = 1;
            this.txtName.EnterPressed += new System.EventHandler(this.OnSearchClick);
            this.txtName.Child = null;
            // 
            // lblDept
            // 
            this.lblDept.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblDept.Location = new System.Drawing.Point(232, 12);
            this.lblDept.Name = "lblDept";
            this.lblDept.Size = new System.Drawing.Size(40, 32);
            this.lblDept.TabIndex = 2;
            this.lblDept.Text = "부서";
            this.lblDept.Child = null;
            // 
            // cboDept
            // 
            this.cboDept.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cboDept.Location = new System.Drawing.Point(276, 12);
            this.cboDept.Name = "cboDept";
            this.cboDept.PlaceholderText = "부서 전체";
            this.cboDept.Size = new System.Drawing.Size(140, 32);
            this.cboDept.TabIndex = 3;
            this.cboDept.Child = null;
            // 
            // lblRank
            // 
            this.lblRank.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblRank.Location = new System.Drawing.Point(432, 12);
            this.lblRank.Name = "lblRank";
            this.lblRank.Size = new System.Drawing.Size(40, 32);
            this.lblRank.TabIndex = 4;
            this.lblRank.Text = "직급";
            this.lblRank.Child = null;
            // 
            // cboRank
            // 
            this.cboRank.Location = new System.Drawing.Point(476, 12);
            this.cboRank.Name = "cboRank";
            this.cboRank.PlaceholderText = "직급 전체";
            this.cboRank.Size = new System.Drawing.Size(150, 32);
            this.cboRank.TabIndex = 5;
            this.cboRank.Child = null;
            //
            // lblHireDate
            //
            this.lblHireDate.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblHireDate.Location = new System.Drawing.Point(12, 56);
            this.lblHireDate.Name = "lblHireDate";
            this.lblHireDate.Size = new System.Drawing.Size(40, 32);
            this.lblHireDate.TabIndex = 8;
            this.lblHireDate.Text = "입사일";
            this.lblHireDate.Child = null;
            //
            // dtHireFrom
            //
            this.dtHireFrom.Location = new System.Drawing.Point(56, 56);
            this.dtHireFrom.Name = "dtHireFrom";
            this.dtHireFrom.Size = new System.Drawing.Size(130, 32);
            this.dtHireFrom.TabIndex = 9;
            this.dtHireFrom.Child = null;
            //
            // lblHireTilde
            //
            this.lblHireTilde.Location = new System.Drawing.Point(192, 56);
            this.lblHireTilde.Name = "lblHireTilde";
            this.lblHireTilde.Size = new System.Drawing.Size(16, 32);
            this.lblHireTilde.TabIndex = 10;
            this.lblHireTilde.Text = "~";
            this.lblHireTilde.Child = null;
            //
            // dtHireTo
            //
            this.dtHireTo.Location = new System.Drawing.Point(214, 56);
            this.dtHireTo.Name = "dtHireTo";
            this.dtHireTo.Size = new System.Drawing.Size(130, 32);
            this.dtHireTo.TabIndex = 11;
            this.dtHireTo.Child = null;
            //
            // btnSearch
            //
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(756, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 32);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "조회";
            this.btnSearch.Click += new System.EventHandler(this.OnSearchClick);
            this.btnSearch.Child = null;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Subtle;
            this.btnReset.Location = new System.Drawing.Point(844, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 32);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "초기화";
            this.btnReset.Click += new System.EventHandler(this.OnResetClick);
            this.btnReset.Child = null;
            // 
            // gridEmployee
            // 
            this.gridEmployee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridEmployee.Name = "gridEmployee";
            this.gridEmployee.Size = new System.Drawing.Size(936, 432);
            this.gridEmployee.TabIndex = 1;
            this.gridEmployee.Child = null;
            //
            // spacerSearchGap
            //
            this.spacerSearchGap.Dock = System.Windows.Forms.DockStyle.Top;
            this.spacerSearchGap.Name = "spacerSearchGap";
            this.spacerSearchGap.Size = new System.Drawing.Size(936, 8);
            this.spacerSearchGap.TabIndex = 3;
            //
            // spacerBottomGap
            //
            this.spacerBottomGap.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.spacerBottomGap.Name = "spacerBottomGap";
            this.spacerBottomGap.Size = new System.Drawing.Size(936, 8);
            this.spacerBottomGap.TabIndex = 4;
            //
            // bottomPanel
            //
            this.bottomPanel.Controls.Add(this.statsCard);
            this.bottomPanel.Controls.Add(this.spacerBtnGap);
            this.bottomPanel.Controls.Add(this.buttonsCard);
            this.bottomPanel.Controls.Add(this.spacerCountGap);
            this.bottomPanel.Controls.Add(this.countCard);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(936, 92);
            this.bottomPanel.TabIndex = 2;
            //
            // countCard
            //
            this.countCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.countCard.Controls.Add(this.cardCount);
            this.countCard.Dock = System.Windows.Forms.DockStyle.Left;
            this.countCard.Name = "countCard";
            this.countCard.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.countCard.Size = new System.Drawing.Size(100, 92);
            this.countCard.TabIndex = 0;
            //
            // spacerCountGap
            //
            this.spacerCountGap.Dock = System.Windows.Forms.DockStyle.Left;
            this.spacerCountGap.Name = "spacerCountGap";
            this.spacerCountGap.Size = new System.Drawing.Size(12, 92);
            this.spacerCountGap.TabIndex = 1;
            //
            // statsCard
            //
            this.statsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.statsCard.Controls.Add(this.listRankCount);
            this.statsCard.Controls.Add(this.listDeptCount);
            this.statsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsCard.Name = "statsCard";
            this.statsCard.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.statsCard.Size = new System.Drawing.Size(532, 92);
            this.statsCard.TabIndex = 2;
            //
            // spacerBtnGap
            //
            this.spacerBtnGap.Dock = System.Windows.Forms.DockStyle.Right;
            this.spacerBtnGap.Name = "spacerBtnGap";
            this.spacerBtnGap.Size = new System.Drawing.Size(12, 92);
            this.spacerBtnGap.TabIndex = 3;
            //
            // buttonsCard
            //
            this.buttonsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.buttonsCard.Controls.Add(this.btnExecute);
            this.buttonsCard.Controls.Add(this.btnNew);
            this.buttonsCard.Controls.Add(this.btnSave);
            this.buttonsCard.Controls.Add(this.btnDelete);
            this.buttonsCard.Controls.Add(this.btnExcel);
            this.buttonsCard.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonsCard.Name = "buttonsCard";
            this.buttonsCard.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.buttonsCard.Size = new System.Drawing.Size(280, 92);
            this.buttonsCard.TabIndex = 4;
            // 
            // cardCount
            // 
            this.cardCount.Flat = true;
            this.cardCount.Location = new System.Drawing.Point(12, 12);
            this.cardCount.Name = "cardCount";
            this.cardCount.Size = new System.Drawing.Size(76, 68);
            this.cardCount.TabIndex = 0;
            this.cardCount.Title = "조회 건수";
            this.cardCount.Child = null;
            // 
            // listDeptCount
            // 
            this.listDeptCount.Dock = System.Windows.Forms.DockStyle.Top;
            this.listDeptCount.Flat = true;
            this.listDeptCount.Name = "listDeptCount";
            this.listDeptCount.Size = new System.Drawing.Size(508, 38);
            this.listDeptCount.TabIndex = 1;
            this.listDeptCount.Title = "부서별";
            this.listDeptCount.Child = null;
            // 
            // listRankCount
            // 
            this.listRankCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listRankCount.Flat = true;
            this.listRankCount.Name = "listRankCount";
            this.listRankCount.TabIndex = 2;
            this.listRankCount.Title = "직급별";
            this.listRankCount.Child = null;
            // 
            // btnExecute
            // 
            this.btnExecute.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Execute;
            this.btnExecute.Location = new System.Drawing.Point(12, 13);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(80, 32);
            this.btnExecute.TabIndex = 3;
            this.btnExecute.Text = "실행";
            this.btnExecute.Click += new System.EventHandler(this.OnExecuteClick);
            this.btnExecute.Child = null;
            // 
            // btnNew
            // 
            this.btnNew.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Secondary;
            this.btnNew.Location = new System.Drawing.Point(100, 13);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(80, 32);
            this.btnNew.TabIndex = 4;
            this.btnNew.Text = "신규";
            this.btnNew.Click += new System.EventHandler(this.OnNewClick);
            this.btnNew.Child = null;
            // 
            // btnSave
            // 
            this.btnSave.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Secondary;
            this.btnSave.Location = new System.Drawing.Point(188, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 32);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "저장";
            this.btnSave.Click += new System.EventHandler(this.OnSaveClick);
            this.btnSave.Child = null;
            // 
            // btnDelete
            // 
            this.btnDelete.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Danger;
            this.btnDelete.Location = new System.Drawing.Point(12, 49);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 32);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "삭제";
            this.btnDelete.Click += new System.EventHandler(this.OnDeleteClick);
            this.btnDelete.Child = null;
            // 
            // btnExcel
            // 
            this.btnExcel.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Subtle;
            this.btnExcel.Location = new System.Drawing.Point(100, 49);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(80, 32);
            this.btnExcel.TabIndex = 7;
            this.btnExcel.Text = "엑셀";
            this.btnExcel.Click += new System.EventHandler(this.OnExcelClick);
            this.btnExcel.Child = null;
            // 
            // EmployeeManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(248)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(960, 620);
            this.Controls.Add(this.gridEmployee);
            this.Controls.Add(this.spacerBottomGap);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.spacerSearchGap);
            this.Controls.Add(this.searchCard);
            this.Controls.Add(this.spacerTitleGap);
            this.Controls.Add(this.lblPageTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(760, 480);
            this.Name = "EmployeeManagementForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "직원관리";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.searchCard.ResumeLayout(false);
            this.countCard.ResumeLayout(false);
            this.statsCard.ResumeLayout(false);
            this.buttonsCard.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblPageTitle;
        private System.Windows.Forms.Panel spacerTitleGap;
        private Modern.Lab.WinForms.Controls.Layout.ModernCardPanel searchCard;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblName;
        private Modern.Lab.WinForms.Controls.Input.ModernTextBox txtName;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblDept;
        private Modern.Lab.WinForms.Controls.Selection.ModernComboBox cboDept;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblRank;
        private Modern.Lab.WinForms.Controls.Selection.ModernCheckComboBox cboRank;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblHireDate;
        private Modern.Lab.WinForms.Controls.Input.ModernDatePicker dtHireFrom;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblHireTilde;
        private Modern.Lab.WinForms.Controls.Input.ModernDatePicker dtHireTo;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnSearch;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnReset;
        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridEmployee;
        private System.Windows.Forms.Panel spacerSearchGap;
        private System.Windows.Forms.Panel spacerBottomGap;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel spacerCountGap;
        private Modern.Lab.WinForms.Controls.Layout.ModernCardPanel countCard;
        private Modern.Lab.WinForms.Controls.Layout.ModernCardPanel statsCard;
        private System.Windows.Forms.Panel spacerBtnGap;
        private Modern.Lab.WinForms.Controls.Layout.ModernCardPanel buttonsCard;
        private Modern.Lab.WinForms.Controls.Display.ModernKpiCard cardCount;
        private Modern.Lab.WinForms.Controls.Display.ModernSummaryList listDeptCount;
        private Modern.Lab.WinForms.Controls.Display.ModernSummaryList listRankCount;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnExecute;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnNew;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnSave;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnDelete;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnExcel;
    }
}
