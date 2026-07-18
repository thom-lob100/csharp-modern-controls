namespace Modern.Lab.Samples
{
    public partial class EquipmentLotForm
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
            this.components = new System.ComponentModel.Container();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblTitle = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.badgeEnv = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.spTitle = new System.Windows.Forms.Panel();
            this.searchCard = new Modern.Lab.WinForms.Controls.Layout.ModernCardPanel();
            this.lblGroup = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboGroup = new Modern.Lab.WinForms.Controls.Selection.ModernComboBox();
            this.lblAuto = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
            this.cboRefresh = new Modern.Lab.WinForms.Controls.Selection.ModernComboBox();
            this.btnRefresh = new Modern.Lab.WinForms.Controls.Input.ModernButton();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.timerCountdown = new System.Windows.Forms.Timer(this.components);
            this.menuEqp = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuPort = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuLot = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAssign = new System.Windows.Forms.ToolStripMenuItem();
            this.gapSearch = new System.Windows.Forms.Panel();
            this.midPanel = new System.Windows.Forms.Panel();
            this.splitMid = new Modern.Lab.WinForms.Controls.Layout.ModernSplitContainer();
            this.splitLeft = new Modern.Lab.WinForms.Controls.Layout.ModernSplitContainer();
            this.eqpCard = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.gridEqp = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.runCard = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.gridRun = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.portCard = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.gridPorts = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.ddbPortActions = new Modern.Lab.WinForms.Controls.Input.ModernDropDownButton();
            this.gapPort = new System.Windows.Forms.Panel();
            this.lotCard = new Modern.Lab.WinForms.Controls.Layout.ModernGroupBox();
            this.gridLots = new Modern.Lab.WinForms.Controls.Data.ModernDataGrid();
            this.gapBottom = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.kpiStrip = new Modern.Lab.WinForms.Controls.Layout.ModernCardPanel();
            this.gapAction = new System.Windows.Forms.Panel();
            this.actionCard = new Modern.Lab.WinForms.Controls.Layout.ModernCardPanel();
            this.ddbActions = new Modern.Lab.WinForms.Controls.Input.ModernDropDownButton();
            this.badgeRun = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.badgeIdle = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.badgeDown = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.badgeFreeIn = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.badgeWaiting = new Modern.Lab.WinForms.Controls.Display.ModernStatusBadge();
            this.toastMain = new Modern.Lab.WinForms.Controls.Display.ModernToast();
            this.titlePanel.SuspendLayout();
            this.searchCard.SuspendLayout();
            this.midPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMid)).BeginInit();
            this.splitMid.Panel1.SuspendLayout();
            this.splitMid.Panel2.SuspendLayout();
            this.splitMid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitLeft)).BeginInit();
            this.splitLeft.Panel1.SuspendLayout();
            this.splitLeft.Panel2.SuspendLayout();
            this.splitLeft.SuspendLayout();
            this.eqpCard.SuspendLayout();
            this.runCard.SuspendLayout();
            this.portCard.SuspendLayout();
            this.lotCard.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.kpiStrip.SuspendLayout();
            this.actionCard.SuspendLayout();
            this.SuspendLayout();
            //
            // titlePanel
            //
            this.titlePanel.Controls.Add(this.lblTitle);
            this.titlePanel.Controls.Add(this.badgeEnv);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(12, 12);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(1516, 28);
            this.titlePanel.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Title;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1456, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Equipment / Lots";
            this.lblTitle.TitleBar = true;
            this.lblTitle.Child = null;
            //
            // badgeEnv
            //
            this.badgeEnv.Color = "#DBEAFE";
            this.badgeEnv.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeEnv.Dock = System.Windows.Forms.DockStyle.Right;
            this.badgeEnv.Location = new System.Drawing.Point(1456, 0);
            this.badgeEnv.Name = "badgeEnv";
            this.badgeEnv.Size = new System.Drawing.Size(60, 28);
            this.badgeEnv.TabIndex = 1;
            this.badgeEnv.Text = "AOS";
            this.badgeEnv.Child = null;
            //
            // spTitle
            //
            this.spTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.spTitle.Location = new System.Drawing.Point(12, 40);
            this.spTitle.Name = "spTitle";
            this.spTitle.Size = new System.Drawing.Size(1516, 8);
            this.spTitle.TabIndex = 1;
            //
            // searchCard
            //
            this.searchCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.searchCard.Controls.Add(this.lblGroup);
            this.searchCard.Controls.Add(this.cboGroup);
            this.searchCard.Controls.Add(this.lblAuto);
            this.searchCard.Controls.Add(this.cboRefresh);
            this.searchCard.Controls.Add(this.btnRefresh);
            this.searchCard.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchCard.Location = new System.Drawing.Point(12, 48);
            this.searchCard.Name = "searchCard";
            this.searchCard.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.searchCard.Size = new System.Drawing.Size(1516, 56);
            this.searchCard.TabIndex = 2;
            //
            // lblGroup
            //
            this.lblGroup.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblGroup.Location = new System.Drawing.Point(12, 12);
            this.lblGroup.Name = "lblGroup";
            this.lblGroup.Size = new System.Drawing.Size(48, 32);
            this.lblGroup.TabIndex = 0;
            this.lblGroup.Text = "Group";
            this.lblGroup.Child = null;
            //
            // cboGroup
            //
            this.cboGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGroup.Location = new System.Drawing.Point(64, 12);
            this.cboGroup.Name = "cboGroup";
            this.cboGroup.Size = new System.Drawing.Size(160, 32);
            this.cboGroup.TabIndex = 1;
            this.cboGroup.SelectedIndexChanged += new System.EventHandler(this.OnGroupChanged);
            this.cboGroup.Child = null;
            //
            // menuEqp
            //
            // 항목은 디자이너가 아니라 폼 코드(BuildEquipmentActions →
            // PopulateEquipmentEntryPoints)가 액션 정의에서 생성한다 —
            // 하단 Actions 드롭다운과 목록·실행 로직을 공유하기 위해서다.
            this.menuEqp.Name = "menuEqp";
            this.menuEqp.Size = new System.Drawing.Size(200, 142);
            this.menuEqp.Opening += new System.ComponentModel.CancelEventHandler(this.OnMenuEqpOpening);
            //
            // menuPort
            //
            // 항목은 장비 메뉴와 마찬가지로 폼 코드(BuildPortActions →
            // PopulatePortEntryPoints)가 액션 정의에서 생성한다 — 포트 카드의
            // Port Actions 드롭다운과 목록·실행 로직을 공유하기 위해서다.
            this.menuPort.Name = "menuPort";
            this.menuPort.Size = new System.Drawing.Size(140, 26);
            this.menuPort.Opening += new System.ComponentModel.CancelEventHandler(this.OnMenuPortOpening);
            //
            // menuLot
            //
            this.menuLot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAssign});
            this.menuLot.Name = "menuLot";
            this.menuLot.Size = new System.Drawing.Size(180, 26);
            this.menuLot.Opening += new System.ComponentModel.CancelEventHandler(this.OnMenuLotOpening);
            //
            // menuAssign
            //
            this.menuAssign.Name = "menuAssign";
            this.menuAssign.Size = new System.Drawing.Size(179, 22);
            this.menuAssign.Text = "Assign";
            this.menuAssign.Click += new System.EventHandler(this.OnMenuAssignClick);
            //
            // lblAuto
            //
            this.lblAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAuto.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblAuto.Location = new System.Drawing.Point(1256, 12);
            this.lblAuto.Name = "lblAuto";
            this.lblAuto.Size = new System.Drawing.Size(40, 32);
            this.lblAuto.TabIndex = 2;
            this.lblAuto.Text = "Auto";
            this.lblAuto.Child = null;
            //
            // cboRefresh
            //
            this.cboRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRefresh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRefresh.Location = new System.Drawing.Point(1300, 12);
            this.cboRefresh.Name = "cboRefresh";
            this.cboRefresh.Size = new System.Drawing.Size(110, 32);
            this.cboRefresh.TabIndex = 3;
            this.cboRefresh.SelectedIndexChanged += new System.EventHandler(this.OnRefreshIntervalChanged);
            this.cboRefresh.Child = null;
            //
            // timerRefresh
            //
            this.timerRefresh.Tick += new System.EventHandler(this.OnAutoRefreshTick);
            //
            // timerCountdown
            //
            this.timerCountdown.Interval = 1000;
            this.timerCountdown.Tick += new System.EventHandler(this.OnCountdownTick);
            //
            // btnRefresh
            //
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(1424, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 32);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.OnRefreshClick);
            this.btnRefresh.Child = null;
            //
            // gapSearch
            //
            this.gapSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.gapSearch.Location = new System.Drawing.Point(12, 104);
            this.gapSearch.Name = "gapSearch";
            this.gapSearch.Size = new System.Drawing.Size(1516, 8);
            this.gapSearch.TabIndex = 7;
            //
            // midPanel
            //
            this.midPanel.Controls.Add(this.splitMid);
            this.midPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.midPanel.Location = new System.Drawing.Point(12, 112);
            this.midPanel.Name = "midPanel";
            this.midPanel.Size = new System.Drawing.Size(1516, 612);
            this.midPanel.TabIndex = 3;
            //
            // splitMid
            //
            this.splitMid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMid.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMid.Location = new System.Drawing.Point(0, 0);
            this.splitMid.Name = "splitMid";
            this.splitMid.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.splitMid.Panel1.Controls.Add(this.splitLeft);
            this.splitMid.Panel1MinSize = 620;
            this.splitMid.Panel2.Controls.Add(this.lotCard);
            this.splitMid.Panel2.Controls.Add(this.gapPort);
            this.splitMid.Panel2.Controls.Add(this.portCard);
            this.splitMid.Panel2.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.splitMid.Panel2MinSize = 380;
            this.splitMid.Size = new System.Drawing.Size(1516, 612);
            this.splitMid.SplitterDistance = 960;
            this.splitMid.TabIndex = 0;
            //
            // eqpCard
            //
            //
            // splitLeft
            //
            // 장비(위, 그룹당 5대 안팎)와 진행 Lot(아래, 보통 10건 이상)의
            // 높이를 스플리터로 조절한다 — 기본은 장비 280 + 나머지 진행 Lot.
            this.splitLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLeft.Location = new System.Drawing.Point(0, 0);
            this.splitLeft.Name = "splitLeft";
            this.splitLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitLeft.Panel1.Controls.Add(this.eqpCard);
            this.splitLeft.Panel1MinSize = 150;
            this.splitLeft.Panel2.Controls.Add(this.runCard);
            this.splitLeft.Panel2.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.splitLeft.Panel2MinSize = 150;
            this.splitLeft.Size = new System.Drawing.Size(960, 612);
            this.splitLeft.SplitterDistance = 280;
            this.splitLeft.TabIndex = 0;
            //
            // eqpCard
            //
            this.eqpCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.eqpCard.Controls.Add(this.gridEqp);
            this.eqpCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eqpCard.Location = new System.Drawing.Point(0, 0);
            this.eqpCard.Name = "eqpCard";
            this.eqpCard.Padding = new System.Windows.Forms.Padding(8, 40, 8, 8);
            this.eqpCard.Size = new System.Drawing.Size(960, 280);
            this.eqpCard.TabIndex = 0;
            this.eqpCard.Text = "Equipments";
            this.eqpCard.TitleAccent = true;
            //
            // gridEqp
            //
            this.gridEqp.AllowColumnFilters = true;
            this.gridEqp.AutoFitColumns = true;
            this.gridEqp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridEqp.Location = new System.Drawing.Point(8, 40);
            this.gridEqp.Name = "gridEqp";
            this.gridEqp.ShowStatusBar = true;
            this.gridEqp.Size = new System.Drawing.Size(1014, 232);
            this.gridEqp.StatusCountFormat = "{0:N0} equipments";
            this.gridEqp.TabIndex = 0;
            this.gridEqp.ContextMenuStrip = this.menuEqp;
            this.gridEqp.SelectionChanged += new System.EventHandler(this.OnEqpSelectionChanged);
            this.gridEqp.Child = null;
            //
            // runCard
            //
            this.runCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.runCard.Controls.Add(this.gridRun);
            this.runCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runCard.Location = new System.Drawing.Point(0, 8);
            this.runCard.Name = "runCard";
            this.runCard.Padding = new System.Windows.Forms.Padding(8, 40, 8, 8);
            this.runCard.Size = new System.Drawing.Size(960, 320);
            this.runCard.TabIndex = 0;
            this.runCard.Text = "Lots in Progress";
            //
            // gridRun
            //
            this.gridRun.AllowColumnFilters = true;
            this.gridRun.AutoFitColumns = true;
            this.gridRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridRun.Location = new System.Drawing.Point(8, 40);
            this.gridRun.Name = "gridRun";
            this.gridRun.ShowStatusBar = true;
            this.gridRun.Size = new System.Drawing.Size(1014, 276);
            this.gridRun.StatusCountFormat = "{0:N0} lots in progress";
            this.gridRun.TabIndex = 0;
            this.gridRun.Child = null;
            //
            // portCard
            //
            this.portCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.portCard.Controls.Add(this.gridPorts);
            this.portCard.Dock = System.Windows.Forms.DockStyle.Top;
            this.portCard.Location = new System.Drawing.Point(8, 0);
            this.portCard.Name = "portCard";
            this.portCard.Padding = new System.Windows.Forms.Padding(8, 40, 8, 8);
            this.portCard.Size = new System.Drawing.Size(478, 216);
            this.portCard.TabIndex = 1;
            this.portCard.Text = "Ports";
            //
            // gridPorts
            //
            this.gridPorts.AutoFitColumns = true;
            this.gridPorts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPorts.Location = new System.Drawing.Point(8, 40);
            this.gridPorts.Name = "gridPorts";
            this.gridPorts.ContextMenuStrip = this.menuPort;
            this.gridPorts.Size = new System.Drawing.Size(462, 168);
            this.gridPorts.TabIndex = 0;
            this.gridPorts.CellButtonClick += new System.EventHandler<Modern.Lab.Controls.Wpf.Data.GridButtonClickEventArgs>(this.OnPortCellButtonClick);
            this.gridPorts.SelectionChanged += new System.EventHandler(this.OnPortSelectionChanged);
            this.gridPorts.Child = null;
            //
            // gapPort
            //
            this.gapPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.gapPort.Location = new System.Drawing.Point(8, 216);
            this.gapPort.Name = "gapPort";
            this.gapPort.Size = new System.Drawing.Size(478, 8);
            this.gapPort.TabIndex = 2;
            //
            // lotCard
            //
            this.lotCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lotCard.Controls.Add(this.gridLots);
            this.lotCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lotCard.Location = new System.Drawing.Point(8, 224);
            this.lotCard.Name = "lotCard";
            this.lotCard.Padding = new System.Windows.Forms.Padding(8, 40, 8, 8);
            this.lotCard.Size = new System.Drawing.Size(478, 388);
            this.lotCard.TabIndex = 0;
            this.lotCard.Text = "Waiting Lots";
            //
            // gridLots
            //
            this.gridLots.AllowColumnFilters = true;
            this.gridLots.AutoFitColumns = true;
            this.gridLots.ContextMenuStrip = this.menuLot;
            this.gridLots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLots.Location = new System.Drawing.Point(8, 40);
            this.gridLots.Name = "gridLots";
            this.gridLots.ShowStatusBar = true;
            this.gridLots.Size = new System.Drawing.Size(462, 564);
            this.gridLots.StatusCountFormat = "{0:N0} lots waiting";
            this.gridLots.TabIndex = 0;
            this.gridLots.CellButtonClick += new System.EventHandler<Modern.Lab.Controls.Wpf.Data.GridButtonClickEventArgs>(this.OnLotCellButtonClick);
            this.gridLots.Child = null;
            //
            // gapBottom
            //
            this.gapBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gapBottom.Location = new System.Drawing.Point(12, 724);
            this.gapBottom.Name = "gapBottom";
            this.gapBottom.Size = new System.Drawing.Size(1516, 8);
            this.gapBottom.TabIndex = 4;
            //
            // bottomPanel
            //
            this.bottomPanel.Controls.Add(this.kpiStrip);
            this.bottomPanel.Controls.Add(this.gapAction);
            this.bottomPanel.Controls.Add(this.actionCard);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(12, 732);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(1516, 56);
            this.bottomPanel.TabIndex = 5;
            //
            // kpiStrip
            //
            this.kpiStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.kpiStrip.Controls.Add(this.badgeRun);
            this.kpiStrip.Controls.Add(this.badgeIdle);
            this.kpiStrip.Controls.Add(this.badgeDown);
            this.kpiStrip.Controls.Add(this.badgeFreeIn);
            this.kpiStrip.Controls.Add(this.badgeWaiting);
            this.kpiStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpiStrip.Location = new System.Drawing.Point(0, 0);
            this.kpiStrip.Name = "kpiStrip";
            this.kpiStrip.Size = new System.Drawing.Size(1314, 56);
            this.kpiStrip.TabIndex = 0;
            //
            // gapAction
            //
            this.gapAction.Dock = System.Windows.Forms.DockStyle.Right;
            this.gapAction.Location = new System.Drawing.Point(1314, 0);
            this.gapAction.Name = "gapAction";
            this.gapAction.Size = new System.Drawing.Size(8, 56);
            this.gapAction.TabIndex = 1;
            //
            // actionCard
            //
            this.actionCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.actionCard.Controls.Add(this.ddbPortActions);
            this.actionCard.Controls.Add(this.ddbActions);
            this.actionCard.Dock = System.Windows.Forms.DockStyle.Right;
            this.actionCard.Location = new System.Drawing.Point(1322, 0);
            this.actionCard.Name = "actionCard";
            this.actionCard.Size = new System.Drawing.Size(360, 56);
            this.actionCard.TabIndex = 2;
            //
            // ddbPortActions
            //
            // 포트 처리 진입점 — 장비 Actions 좌측에 같은 스타일로 나란히
            // 배치한다 (포트 카드 안에 두면 카드 높이를 잡아먹는다).
            this.ddbPortActions.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Execute;
            this.ddbPortActions.Location = new System.Drawing.Point(12, 12);
            this.ddbPortActions.Name = "ddbPortActions";
            this.ddbPortActions.Size = new System.Drawing.Size(160, 32);
            this.ddbPortActions.TabIndex = 0;
            this.ddbPortActions.Text = "Port Actions";
            this.ddbPortActions.ItemClicked += new System.EventHandler<Modern.Lab.Controls.Wpf.Input.DropDownItemClickedEventArgs>(this.OnPortActionMenuClicked);
            this.ddbPortActions.Child = null;
            //
            // ddbActions
            //
            this.ddbActions.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Execute;
            this.ddbActions.Location = new System.Drawing.Point(180, 12);
            this.ddbActions.Name = "ddbActions";
            this.ddbActions.Size = new System.Drawing.Size(168, 32);
            this.ddbActions.TabIndex = 1;
            this.ddbActions.Text = "Actions";
            this.ddbActions.ItemClicked += new System.EventHandler<Modern.Lab.Controls.Wpf.Input.DropDownItemClickedEventArgs>(this.OnActionMenuClicked);
            this.ddbActions.Child = null;
            //
            // badgeRun
            //
            this.badgeRun.Color = "#DCFCE7";
            this.badgeRun.Location = new System.Drawing.Point(12, 16);
            this.badgeRun.Name = "badgeRun";
            this.badgeRun.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeRun.Size = new System.Drawing.Size(90, 24);
            this.badgeRun.TabIndex = 0;
            this.badgeRun.Text = "-";
            this.badgeRun.Child = null;
            //
            // badgeIdle
            //
            this.badgeIdle.Color = "#E5E7EB";
            this.badgeIdle.Location = new System.Drawing.Point(110, 16);
            this.badgeIdle.Name = "badgeIdle";
            this.badgeIdle.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeIdle.Size = new System.Drawing.Size(90, 24);
            this.badgeIdle.TabIndex = 1;
            this.badgeIdle.Text = "-";
            this.badgeIdle.Child = null;
            //
            // badgeDown
            //
            this.badgeDown.Color = "#FEE2E2";
            this.badgeDown.Location = new System.Drawing.Point(208, 16);
            this.badgeDown.Name = "badgeDown";
            this.badgeDown.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeDown.Size = new System.Drawing.Size(90, 24);
            this.badgeDown.TabIndex = 2;
            this.badgeDown.Text = "-";
            this.badgeDown.Child = null;
            //
            // badgeFreeIn
            //
            this.badgeFreeIn.Color = "#DBEAFE";
            this.badgeFreeIn.Location = new System.Drawing.Point(306, 16);
            this.badgeFreeIn.Name = "badgeFreeIn";
            this.badgeFreeIn.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeFreeIn.Size = new System.Drawing.Size(130, 24);
            this.badgeFreeIn.TabIndex = 3;
            this.badgeFreeIn.Text = "-";
            this.badgeFreeIn.Child = null;
            //
            // badgeWaiting
            //
            this.badgeWaiting.Color = "#FEF3C7";
            this.badgeWaiting.Location = new System.Drawing.Point(444, 16);
            this.badgeWaiting.Name = "badgeWaiting";
            this.badgeWaiting.Shape = Modern.Lab.WinForms.Controls.Display.BadgeShape.Rounded;
            this.badgeWaiting.Size = new System.Drawing.Size(110, 24);
            this.badgeWaiting.TabIndex = 4;
            this.badgeWaiting.Text = "-";
            this.badgeWaiting.Child = null;
            //
            // toastMain
            //
            this.toastMain.Location = new System.Drawing.Point(1220, 720);
            this.toastMain.Name = "toastMain";
            this.toastMain.Size = new System.Drawing.Size(280, 44);
            this.toastMain.TabIndex = 6;
            this.toastMain.Visible = false;
            this.toastMain.Child = null;
            //
            // EquipmentLotForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(248)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1540, 800);
            this.Controls.Add(this.toastMain);
            this.Controls.Add(this.midPanel);
            this.Controls.Add(this.gapBottom);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.gapSearch);
            this.Controls.Add(this.searchCard);
            this.Controls.Add(this.spTitle);
            this.Controls.Add(this.titlePanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1240, 660);
            this.Name = "EquipmentLotForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Equipment / Lots";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.titlePanel.ResumeLayout(false);
            this.searchCard.ResumeLayout(false);
            this.midPanel.ResumeLayout(false);
            this.splitMid.Panel1.ResumeLayout(false);
            this.splitMid.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMid)).EndInit();
            this.splitMid.ResumeLayout(false);
            this.splitLeft.Panel1.ResumeLayout(false);
            this.splitLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitLeft)).EndInit();
            this.splitLeft.ResumeLayout(false);
            this.eqpCard.ResumeLayout(false);
            this.runCard.ResumeLayout(false);
            this.portCard.ResumeLayout(false);
            this.lotCard.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.kpiStrip.ResumeLayout(false);
            this.actionCard.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel titlePanel;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblTitle;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeEnv;
        private System.Windows.Forms.Panel spTitle;
        private Modern.Lab.WinForms.Controls.Layout.ModernCardPanel searchCard;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblGroup;
        private Modern.Lab.WinForms.Controls.Selection.ModernComboBox cboGroup;
        private Modern.Lab.WinForms.Controls.Display.ModernLabel lblAuto;
        private Modern.Lab.WinForms.Controls.Selection.ModernComboBox cboRefresh;
        private Modern.Lab.WinForms.Controls.Input.ModernButton btnRefresh;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Timer timerCountdown;
        private System.Windows.Forms.ContextMenuStrip menuEqp;
        private System.Windows.Forms.ContextMenuStrip menuPort;
        private System.Windows.Forms.ContextMenuStrip menuLot;
        private System.Windows.Forms.ToolStripMenuItem menuAssign;
        private System.Windows.Forms.Panel gapSearch;
        private System.Windows.Forms.Panel midPanel;
        private Modern.Lab.WinForms.Controls.Layout.ModernSplitContainer splitMid;
        private Modern.Lab.WinForms.Controls.Layout.ModernSplitContainer splitLeft;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox eqpCard;
        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridEqp;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox runCard;
        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridRun;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox portCard;
        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridPorts;
        private Modern.Lab.WinForms.Controls.Input.ModernDropDownButton ddbPortActions;
        private System.Windows.Forms.Panel gapPort;
        private Modern.Lab.WinForms.Controls.Layout.ModernGroupBox lotCard;
        private Modern.Lab.WinForms.Controls.Data.ModernDataGrid gridLots;
        private System.Windows.Forms.Panel gapBottom;
        private System.Windows.Forms.Panel bottomPanel;
        private Modern.Lab.WinForms.Controls.Layout.ModernCardPanel kpiStrip;
        private System.Windows.Forms.Panel gapAction;
        private Modern.Lab.WinForms.Controls.Layout.ModernCardPanel actionCard;
        private Modern.Lab.WinForms.Controls.Input.ModernDropDownButton ddbActions;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeRun;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeIdle;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeDown;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeFreeIn;
        private Modern.Lab.WinForms.Controls.Display.ModernStatusBadge badgeWaiting;
        private Modern.Lab.WinForms.Controls.Display.ModernToast toastMain;
    }
}
