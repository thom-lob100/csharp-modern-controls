using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Modern.Lab.Controls.Wpf.Display;
using Modern.Lab.WinForms.Controls.Display;

namespace Modern.Lab.WinForms.Controls.Layout
{
    /// <summary>
    /// 캡션/값 상세 표 컨테이너 — System.Windows.Forms.TableLayoutPanel의
    /// 드롭인 대체 (Item History의 Selection 카드 같은 "괘선 상세 표"용).
    ///
    /// 기본 CellBorderStyle은 진회색 클래식 선이라 쓰지 않고, 셀 배경/괘선을
    /// ModernTheme 팔레트로 직접 그린다 — 커스텀 페인트는 ModernThemeWinForms.Apply의
    /// 속성 치환이 닿지 않으므로 이렇게 해야 라이트/다크 모두 맞는 색이 나온다.
    ///
    /// 셀 판정은 좌표 하드코딩 없이 "그 셀을 차지한 컨트롤"로 한다:
    /// - 캡션 셀: 주인이 캡션 라벨(ModernLabel, Kind=Label)이면 그리드 헤더 톤
    ///   (SurfaceAlt)으로 칠한다. 값 라벨(Kind=Body 등)은 칠하지 않는다.
    /// - 오른쪽 세로선: 오른쪽 이웃 셀의 주인이 같은 컨트롤(열 병합 내부)이면
    ///   긋지 않는다 — 병합 마지막 셀의 오른쪽 경계는 그린다.
    ///
    /// 사용법: 기존 TableLayoutPanel처럼 디자이너에서 행/열을 잡고 캡션
    /// (ModernLabel Kind=Label)과 값(ModernLabel) 라벨을 배치하면 끝 —
    /// CellPaint 처리 코드를 폼마다 들고 다닐 필요가 없다. 기존 폼 교체는
    /// .Designer.cs의 타입 선언 변경 + CellPaint 연결 제거만 하면 된다.
    /// </summary>
    [ToolboxItem(true)]
    public class ModernDetailTable : TableLayoutPanel
    {
        /// <summary>리사이즈 중 괘선 깜빡임이 없도록 더블 버퍼링으로 생성한다.</summary>
        public ModernDetailTable()
        {
            this.DoubleBuffered = true;
        }

        /// <summary>셀 배경(캡션 헤더 톤)과 괘선을 테마 팔레트 색으로 그린다.</summary>
        protected override void OnCellPaint(TableLayoutCellPaintEventArgs e)
        {
            base.OnCellPaint(e);

            Control owner = this.GetCellOwner(e.Column, e.Row);
            Control rightNeighbor = this.GetCellOwner(e.Column + 1, e.Row);

            ModernLabel captionLabel = owner as ModernLabel;
            bool captionCell = captionLabel != null && captionLabel.Kind == LabelKind.Label;
            bool insideSpan = owner != null && object.ReferenceEquals(owner, rightNeighbor);

            if (captionCell)
            {
                using (SolidBrush headerBrush = new SolidBrush(Modern.Lab.Theming.ModernTheme.SurfaceAlt))
                {
                    e.Graphics.FillRectangle(headerBrush, e.CellBounds);
                }
            }

            using (Pen linePen = new Pen(Modern.Lab.Theming.ModernTheme.BorderSubtle))
            {
                Rectangle cell = e.CellBounds;

                // 오른쪽 세로선: 병합된 값 영역 내부 셀에는 그리지 않는다.
                if (!insideSpan)
                {
                    e.Graphics.DrawLine(linePen, cell.Right - 1, cell.Top, cell.Right - 1, cell.Bottom - 1);
                }

                // 아래 가로선은 모든 셀에, 왼쪽·위 선은 가장자리 셀에만 그려
                // 이웃 셀과 선이 겹치지 않게 한다.
                e.Graphics.DrawLine(linePen, cell.Left, cell.Bottom - 1, cell.Right - 1, cell.Bottom - 1);

                if (e.Column == 0)
                {
                    e.Graphics.DrawLine(linePen, cell.Left, cell.Top, cell.Left, cell.Bottom - 1);
                }

                if (e.Row == 0)
                {
                    e.Graphics.DrawLine(linePen, cell.Left, cell.Top, cell.Right - 1, cell.Top);
                }
            }
        }

        // (column, row) 셀을 차지한 컨트롤을 찾는다 — 열 병합(ColumnSpan) 포함.
        private Control GetCellOwner(int column, int row)
        {
            if (column < 0 || column >= this.ColumnCount)
            {
                return null;
            }

            foreach (Control child in this.Controls)
            {
                TableLayoutPanelCellPosition position = this.GetPositionFromControl(child);
                int span = this.GetColumnSpan(child);

                if (position.Row == row && column >= position.Column && column < position.Column + span)
                {
                    return child;
                }
            }

            return null;
        }
    }
}
