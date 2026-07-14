using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Modern.Lab.WinForms.Controls.Layout
{
    /// <summary>
    /// 좌/우(또는 상/하) 영역 크기를 드래그로 조절하는 모던 스플리터 —
    /// System.Windows.Forms.SplitContainer의 대체.
    ///
    /// ModernCardPanel과 마찬가지로 **순수 WinForms 컨테이너**다 (계약 규칙 5:
    /// 영역 레이아웃은 WinForms 담당; ElementHost는 WinForms 자식을 담을 수 없다).
    /// 시각만 토큰을 따른다:
    ///   - 거터(스플리터 띠)는 부모 배경색으로 칠해 "깨끗한 간격"으로 보이게 하고,
    ///   - 가운데에 짧은 그립 필(pill)만 그린다 — 평상시 BorderSubtle,
    ///     마우스 오버/드래그 중에는 Accent로 강조.
    ///   - 클릭/드래그 후 남는 점선 포커스 사각형을 제거한다.
    /// 마우스 동작(드래그 리사이즈, Panel1/2 MinSize)은 SplitContainer 그대로.
    ///
    /// 드래그 방식은 <see cref="DeferredDrag"/>로 고른다 (기본 true):
    ///   - true  = 드래그 중 가는 액센트 가이드 라인만 움직이고 놓을 때 한 번 적용.
    ///             WPF 섬(ElementHost)이 많은 화면은 스텝당 수백 ms가 들기 때문에
    ///             (실측: Item History 폼 스텝당 ~320ms vs 빈 폼 ~4ms) 이쪽이 기본값.
    ///   - false = 실시간(라이브) 적용 — 가벼운 화면에서 즉각적인 리플로우를 원할 때.
    /// </summary>
    [ToolboxItem(true)]
    public class ModernSplitContainer : SplitContainer
    {
        // 그립 필 크기 (거터 방향과 직교로 길게 놓인다)
        private const int GripLength = 48;
        private const int GripThickness = 4;

        // 지연 드래그 가이드 라인 두께
        private const int GuideThickness = 2;

        private bool hoverSplitter;
        private bool dragging;
        private int dragDistance;        // 지연 드래그 중 제안 위치 (놓을 때 적용)
        private GuideWindow guideLine;   // 지연 드래그 가이드 (독립 오버레이 창)

        /// <summary>모던 거터 기본값(포커스 없음, 테두리 없음, 12px 폭)으로 생성한다.</summary>
        public ModernSplitContainer()
        {
            this.TabStop = false;
            this.BorderStyle = BorderStyle.None;
            this.SplitterWidth = 12;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            // 드래그 중에도 거터가 계속 현재 색으로 다시 그려지게 한다.
            this.SplitterMoved += (sender, args) => { this.Invalidate(this.SplitterRectangle); };
        }

        /// <summary>
        /// true(기본)면 드래그 중 가이드 라인만 움직이고 마우스를 놓을 때 한 번만
        /// 레이아웃을 적용한다 — ElementHost가 많은 무거운 화면에서도 드래그가 매끄럽다.
        /// false면 드래그에 따라 실시간으로 리사이즈한다 (가벼운 화면용).
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("드래그 중 가이드 라인만 표시하고 놓을 때 한 번 적용 (무거운 화면 권장). false면 실시간 리사이즈")]
        [DefaultValue(true)]
        public bool DeferredDrag { get; set; } = true;

        /// <summary>
        /// 거터에서 시작하는 드래그를 직접 처리한다. base의 기본 드래그는 반투명
        /// XOR 미리보기 띠(두꺼운 회색 띠)를 그려 선이 두 개로 보이므로 태우지 않고,
        /// 드래그 동안 SplitterDistance를 실시간으로 갱신한다(라이브 리사이즈).
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.IsSplitterFixed
                    && e.Button == MouseButtons.Left
                    && this.SplitterRectangle.Contains(e.Location))
            {
                this.dragging = true;
                this.dragDistance = this.SplitterDistance;
                this.Capture = true;

                if (this.DeferredDrag)
                {
                    this.ShowGuide();
                }

                this.Invalidate(this.SplitterRectangle);
                return;   // base 호출 안 함 — 기본 미리보기 드래그 차단
            }

            base.OnMouseDown(e);
        }

        /// <summary>드래그 중이면 스플리터를 실시간 이동, 아니면 그립 강조만 갱신.</summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.dragging)
            {
                int total = this.Orientation == Orientation.Vertical ? this.Width : this.Height;
                int position = this.Orientation == Orientation.Vertical ? e.X : e.Y;
                int distance = position - this.SplitterWidth / 2;

                int min = this.Panel1MinSize;
                int max = total - this.Panel2MinSize - this.SplitterWidth;
                if (max < min)
                {
                    return;   // 창이 너무 작으면 이동 불가
                }

                distance = Math.Max(min, Math.Min(max, distance));

                if (this.DeferredDrag)
                {
                    // 레이아웃은 건드리지 않고 가이드 라인만 이동 (스텝당 ~1ms).
                    if (distance != this.dragDistance)
                    {
                        this.dragDistance = distance;
                        this.UpdateGuide();
                    }
                }
                else if (distance != this.SplitterDistance)
                {
                    this.SplitterDistance = distance;
                    this.dragDistance = distance;
                }
                return;
            }

            base.OnMouseMove(e);

            bool over = this.SplitterRectangle.Contains(e.Location);
            if (over != this.hoverSplitter)
            {
                this.hoverSplitter = over;
                this.Invalidate(this.SplitterRectangle);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.hoverSplitter)
            {
                this.hoverSplitter = false;
                this.Invalidate(this.SplitterRectangle);
            }
        }

        /// <summary>
        /// 스플리터 클릭/드래그 후 포커스를 내려놓아 점선 포커스 사각형이
        /// 남지 않게 한다 (SplitContainer 기본 동작 보정).
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;
                this.Capture = false;
                this.HideGuide();

                // 지연 드래그: 놓는 순간 한 번만 실제 레이아웃을 적용한다.
                if (this.DeferredDrag && this.dragDistance != this.SplitterDistance)
                {
                    this.SplitterDistance = this.dragDistance;
                }

                this.Invalidate(this.SplitterRectangle);
            }
            else
            {
                base.OnMouseUp(e);
            }

            Form form = this.FindForm();
            if (form != null)
            {
                form.ActiveControl = null;
            }
        }

        /// <summary>거터를 부모 배경색으로 칠하고 가운데 그립 필을 그린다.</summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle gutter = this.SplitterRectangle;
            if (gutter.Width <= 0 || gutter.Height <= 0)
            {
                return;
            }

            // 거터 배경: 부모 색을 따라가 어떤 화면(라이트/다크/틴트)에서도 간격처럼 보인다
            // (ModernCardPanel의 모서리 처리와 같은 접근).
            Color canvas = this.Parent != null ? this.Parent.BackColor : this.BackColor;
            using (SolidBrush canvasBrush = new SolidBrush(canvas))
            {
                e.Graphics.FillRectangle(canvasBrush, gutter);
            }

            // 그립 필: 평상시 옅은 구분선 색, 오버/드래그 중 액센트.
            bool active = this.hoverSplitter || this.dragging;
            Color gripColor = active
                    ? Modern.Lab.Theming.ModernTheme.Accent
                    : Modern.Lab.Theming.ModernTheme.BorderSubtle;

            Rectangle grip;
            if (this.Orientation == Orientation.Vertical)
            {
                // 세로 거터(좌/우 분할): 필은 세로로 길게.
                int length = Math.Min(GripLength, gutter.Height - 8);
                grip = new Rectangle(
                        gutter.X + (gutter.Width - GripThickness) / 2,
                        gutter.Y + (gutter.Height - length) / 2,
                        GripThickness,
                        length);
            }
            else
            {
                // 가로 거터(상/하 분할): 필은 가로로 길게.
                int length = Math.Min(GripLength, gutter.Width - 8);
                grip = new Rectangle(
                        gutter.X + (gutter.Width - length) / 2,
                        gutter.Y + (gutter.Height - GripThickness) / 2,
                        length,
                        GripThickness);
            }

            SmoothingMode originalMode = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath pill = CreatePillPath(grip))
            using (SolidBrush gripBrush = new SolidBrush(gripColor))
            {
                e.Graphics.FillPath(gripBrush, pill);
            }

            e.Graphics.SmoothingMode = originalMode;
        }

        // ---- 지연 드래그 가이드 라인 ----
        // WinForms 형제 컨트롤로 얹으면 WPF 섬(ElementHost) 위를 지나갈 때 벗어난
        // 자리를 WPF가 즉시 다시 그리지 않아 잔상(파란 선 자국)이 남는다. 그래서
        // DWM이 독립적으로 합성하는 **클릭-스루 오버레이 창**을 쓴다 — 창이 움직여도
        // 아래 내용을 다시 그릴 필요가 없어 잔상이 생기지 않는다.

        /// <summary>지연 드래그 가이드 — 활성화되지 않고 마우스를 통과시키는 얇은 오버레이 창.</summary>
        private sealed class GuideWindow : Form
        {
            public GuideWindow()
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.ShowInTaskbar = false;
                this.StartPosition = FormStartPosition.Manual;
                this.MinimumSize = new Size(1, 1);

                // WS_EX_LAYERED 창은 알파를 지정해야 그려진다 — Opacity로 WinForms가
                // SetLayeredWindowAttributes를 관리하게 한다 (살짝 투명해 가이드답게 보임).
                this.Opacity = 0.85;
            }

            /// <summary>표시할 때 포커스를 훔치지 않는다 (드래그 캡처 유지).</summary>
            protected override bool ShowWithoutActivation
            {
                get { return true; }
            }

            /// <summary>클릭-스루 + 비활성 + 툴윈도(Alt-Tab 제외) 스타일.</summary>
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    // WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE
                    cp.ExStyle |= 0x00080000 | 0x00000020 | 0x00000080 | 0x08000000;
                    return cp;
                }
            }
        }

        /// <summary>드래그 시작 시 가이드 창을 만들어 현재 위치에 표시한다.</summary>
        private void ShowGuide()
        {
            if (this.guideLine == null)
            {
                this.guideLine = new GuideWindow();
            }

            this.guideLine.BackColor = Modern.Lab.Theming.ModernTheme.Accent;
            this.guideLine.Owner = this.FindForm();
            this.UpdateGuide();
            this.guideLine.Show();
        }

        /// <summary>제안 위치(dragDistance)에 맞춰 가이드 창을 옮긴다 (화면 좌표계).</summary>
        private void UpdateGuide()
        {
            if (this.guideLine == null)
            {
                return;
            }

            if (this.Orientation == Orientation.Vertical)
            {
                int clientX = this.dragDistance + (this.SplitterWidth - GuideThickness) / 2;
                Point screen = this.PointToScreen(new Point(clientX, 0));
                this.guideLine.SetBounds(screen.X, screen.Y, GuideThickness, this.Height);
            }
            else
            {
                int clientY = this.dragDistance + (this.SplitterWidth - GuideThickness) / 2;
                Point screen = this.PointToScreen(new Point(0, clientY));
                this.guideLine.SetBounds(screen.X, screen.Y, this.Width, GuideThickness);
            }
        }

        /// <summary>드래그 종료 시 가이드 창을 숨긴다.</summary>
        private void HideGuide()
        {
            if (this.guideLine != null)
            {
                this.guideLine.Hide();
            }
        }

        /// <summary>가이드 라인 리소스를 정리한다.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.guideLine != null)
            {
                this.HideGuide();
                this.guideLine.Dispose();
                this.guideLine = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>양 끝이 둥근 알약 모양 경로.</summary>
        private static GraphicsPath CreatePillPath(Rectangle bounds)
        {
            int diameter = Math.Min(bounds.Width, bounds.Height);
            GraphicsPath path = new GraphicsPath();

            if (diameter <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180f, 90f);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270f, 90f);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0f, 90f);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90f, 90f);
            path.CloseFigure();

            return path;
        }
    }
}
