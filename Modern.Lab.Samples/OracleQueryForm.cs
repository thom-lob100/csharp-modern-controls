using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modern.Lab.Controls.Wpf.Data;
using Modern.Lab.Samples.Services;
using Modern.Lab.WinForms.Controls.Data;
using Modern.Lab.WinForms.Controls.Display;
using Modern.Lab.WinForms.Controls.Input;
using Modern.Lab.WinForms.Controls.Layout;
using Modern.Lab.WinForms.Controls.Selection;

namespace Modern.Lab.Samples
{
    /// <summary>
    /// Oracle 클라우드 DB(홈 개발 인스턴스) 연동 검증 화면.
    ///
    /// 두 가지를 실데이터로 확인한다:
    ///  1) 그리드 콤보박스 — ModernComboBox.ConfigureDropDownColumns로 코드+명칭
    ///     멀티컬럼 드롭다운을 구성하고, DB의 MDL_DEPT 테이블을 그대로 바인딩한다.
    ///     (백그라운드 조회 → UI 스레드 할당: 컨트롤 계약의 순서/스레드 허용 검증)
    ///  2) 임의 SQL 실행 — 결과를 ModernDataGrid(자동 컬럼 생성)로 표시한다.
    ///
    /// 컨트롤 계약에 따라 DB 접근은 이 폼(Samples)에서만 수행한다.
    /// </summary>
    public class OracleQueryForm : Form
    {
        private Panel titlePanel;
        private ModernLabel lblPageTitle;
        private ModernStatusBadge badgeDb;
        private Panel spacerTitleGap;
        private ModernCardPanel searchCard;
        private ModernLabel lblDept;
        private ModernComboBox cboDept;
        private ModernLabel lblSelectedInfo;
        private ModernLabel lblSql;
        private TextBox txtSql;
        private ModernButton btnRun;
        private Panel spacerCardGap;
        private ModernDataGrid gridResult;

        public OracleQueryForm()
        {
            this.InitializeLayout();
        }

        private void InitializeLayout()
        {
            this.SuspendLayout();

            this.Text = "Oracle 조회";
            this.ClientSize = new Size(1540, 800);
            this.BackColor = Color.FromArgb(247, 248, 250);
            this.Font = new Font("Segoe UI", 9F);
            this.Padding = new Padding(12);

            // ---- 타이틀 영역 ----
            this.titlePanel = new Panel();
            this.titlePanel.Dock = DockStyle.Top;
            this.titlePanel.Height = 28;

            this.lblPageTitle = new ModernLabel();
            this.lblPageTitle.Dock = DockStyle.Fill;
            this.lblPageTitle.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Title;
            this.lblPageTitle.TitleBar = true;
            this.lblPageTitle.Text = "Oracle 조회";

            this.badgeDb = new ModernStatusBadge();
            this.badgeDb.Dock = DockStyle.Right;
            this.badgeDb.Width = 110;
            this.badgeDb.Color = "#DBEAFE";
            this.badgeDb.Text = "클라우드 DB";

            this.titlePanel.Controls.Add(this.lblPageTitle);
            this.titlePanel.Controls.Add(this.badgeDb);

            this.spacerTitleGap = new Panel();
            this.spacerTitleGap.Dock = DockStyle.Top;
            this.spacerTitleGap.Height = 8;

            // ---- 조회 카드: 그리드 콤보박스 + SQL 입력 ----
            this.searchCard = new ModernCardPanel();
            this.searchCard.Dock = DockStyle.Top;
            this.searchCard.Height = 168;
            this.searchCard.BackColor = Color.White;
            this.searchCard.Padding = new Padding(12, 8, 12, 8);

            this.lblDept = new ModernLabel();
            this.lblDept.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblDept.Location = new Point(12, 12);
            this.lblDept.Size = new Size(40, 32);
            this.lblDept.Text = "부서";

            // 그리드 콤보박스: DB에서 받은 부서 테이블을 코드+부서명 두 컬럼으로
            // 드롭다운에 표시한다. 검색은 코드("D3")로도 명칭("개발")으로도 동작한다.
            this.cboDept = new ModernComboBox();
            this.cboDept.DropDownStyle = ComboBoxStyle.DropDown;
            this.cboDept.Location = new Point(56, 12);
            this.cboDept.Size = new Size(220, 32);
            this.cboDept.PlaceholderText = "부서 선택 (DB 로딩 중...)";
            this.cboDept.SelectedIndexChanged += this.OnDeptSelectionChanged;

            this.lblSelectedInfo = new ModernLabel();
            this.lblSelectedInfo.Location = new Point(292, 12);
            this.lblSelectedInfo.Size = new Size(600, 32);
            this.lblSelectedInfo.Text = "선택 없음 — 드롭다운을 열면 코드/부서명 헤더가 표시됩니다.";

            this.lblSql = new ModernLabel();
            this.lblSql.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;
            this.lblSql.Location = new Point(12, 56);
            this.lblSql.Size = new Size(40, 32);
            this.lblSql.Text = "SQL";

            this.txtSql = new TextBox();
            this.txtSql.Multiline = true;
            this.txtSql.ScrollBars = ScrollBars.Vertical;
            this.txtSql.Font = new Font("Consolas", 10F);
            this.txtSql.BorderStyle = BorderStyle.FixedSingle;
            this.txtSql.Location = new Point(56, 56);
            this.txtSql.Size = new Size(this.searchCard.Width - 56 - 116, 96);
            this.txtSql.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtSql.Text = "SELECT DEPT_CODE, DEPT_NAME, SORT_ORDER FROM MDL_DEPT ORDER BY SORT_ORDER";

            this.btnRun = new ModernButton();
            this.btnRun.Kind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Execute;
            this.btnRun.Location = new Point(this.searchCard.Width - 104, 56);
            this.btnRun.Size = new Size(88, 32);
            this.btnRun.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnRun.Text = "실행";
            this.btnRun.Click += this.OnRunClick;

            this.searchCard.Controls.Add(this.lblDept);
            this.searchCard.Controls.Add(this.cboDept);
            this.searchCard.Controls.Add(this.lblSelectedInfo);
            this.searchCard.Controls.Add(this.lblSql);
            this.searchCard.Controls.Add(this.txtSql);
            this.searchCard.Controls.Add(this.btnRun);

            this.spacerCardGap = new Panel();
            this.spacerCardGap.Dock = DockStyle.Top;
            this.spacerCardGap.Height = 8;

            // ---- 결과 그리드: 임의 결과셋이므로 자동 컬럼 생성으로 표시 ----
            this.gridResult = new ModernDataGrid();
            this.gridResult.Dock = DockStyle.Fill;
            this.gridResult.ShowStatusBar = true;
            this.gridResult.StatusText = "대기 중";

            this.Controls.Add(this.gridResult);
            this.Controls.Add(this.spacerCardGap);
            this.Controls.Add(this.searchCard);
            this.Controls.Add(this.spacerTitleGap);
            this.Controls.Add(this.titlePanel);

            this.Load += this.OnFormLoad;

            this.ResumeLayout(false);
        }

        // 폼 표시 시 부서 콤보 데이터와 기본 SQL 결과를 백그라운드에서 적재한다.
        private async void OnFormLoad(object sender, EventArgs e)
        {
            await this.LoadDeptComboAsync();
            await this.RunSqlAsync();
        }

        // MDL_DEPT를 백그라운드에서 조회해 그리드 콤보박스에 바인딩한다.
        // ConfigureDropDownColumns는 DataSource 할당 전에 호출한다(컨트롤 계약).
        private async Task LoadDeptComboAsync()
        {
            try
            {
                DataTable deptTable = await Task.Run(
                    delegate { return OracleDb.ExecuteTable("SELECT DEPT_CODE, DEPT_NAME FROM MDL_DEPT ORDER BY SORT_ORDER"); });

                this.cboDept.ConfigureDropDownColumns(
                    new ModernDataGridColumn("DEPT_CODE", "코드", 60),
                    new ModernDataGridColumn("DEPT_NAME", "부서명", 120));
                this.cboDept.DisplayMember = "DEPT_NAME";
                this.cboDept.ValueMember = "DEPT_CODE";
                this.cboDept.DataSource = deptTable;
                this.cboDept.SelectedIndex = -1;
                this.cboDept.PlaceholderText = "부서 선택";
            }
            catch (Exception ex)
            {
                this.lblSelectedInfo.Text = "부서 로딩 실패: " + ex.Message;
            }
        }

        // 콤보 선택이 바뀌면 SelectedValue(코드)가 서버 호출에 쓸 수 있는
        // 값 그대로 나오는지 확인용으로 표시한다.
        private void OnDeptSelectionChanged(object sender, EventArgs e)
        {
            object selectedValue = this.cboDept.SelectedValue;

            if (selectedValue == null)
            {
                this.lblSelectedInfo.Text = "선택 없음";
                return;
            }

            DataRowView rowView = this.cboDept.SelectedItem as DataRowView;
            string deptName = rowView != null ? Convert.ToString(rowView["DEPT_NAME"]) : string.Empty;
            this.lblSelectedInfo.Text = string.Format("SelectedValue = {0} ({1})", selectedValue, deptName);
        }

        private async void OnRunClick(object sender, EventArgs e)
        {
            await this.RunSqlAsync();
        }

        // SQL을 백그라운드에서 실행하고 결과를 그리드에 표시한다.
        // SELECT/WITH 외의 문장은 ExecuteNonQuery로 처리한다.
        private async Task RunSqlAsync()
        {
            string sql = this.txtSql.Text.Trim().TrimEnd(';');

            if (sql.Length == 0)
            {
                this.gridResult.StatusText = "SQL을 입력하세요.";
                return;
            }

            this.btnRun.Enabled = false;
            this.gridResult.StatusText = "실행 중...";

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                if (IsQuery(sql))
                {
                    DataTable table = await Task.Run(delegate { return OracleDb.ExecuteTable(sql); });
                    stopwatch.Stop();
                    this.gridResult.DataSource = table;
                    this.gridResult.StatusText = string.Format("{0}행 · {1:N0} ms", table.Rows.Count, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    int affected = await Task.Run(delegate { return OracleDb.ExecuteNonQuery(sql); });
                    stopwatch.Stop();
                    this.gridResult.DataSource = null;
                    this.gridResult.StatusText = string.Format("완료 · {0}행 영향 · {1:N0} ms", affected, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                this.gridResult.DataSource = null;
                this.gridResult.StatusText = "오류: " + ex.Message;
            }
            finally
            {
                this.btnRun.Enabled = true;
            }
        }

        // 결과셋을 반환하는 문장인지 여부(SELECT/WITH로 시작하면 조회로 간주).
        private static bool IsQuery(string sql)
        {
            string upper = sql.TrimStart().ToUpperInvariant();
            return upper.StartsWith("SELECT") || upper.StartsWith("WITH");
        }
    }
}
