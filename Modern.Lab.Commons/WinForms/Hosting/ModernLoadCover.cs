using System;
using System.Windows.Forms;

namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// 폼 오픈 시 로딩 깜빡임을 "한 줄"로 가리는 헬퍼.
    ///
    /// 모던 컨트롤이 많은 폼은 처음 표시될 때 ElementHost의 WPF 콘텐츠 생성과
    /// 그리드 AutoFit 컬럼 계산이 화면에 노출되어, 컨트롤들이 크기를 잡아가는
    /// 중간 레이아웃이 프레임마다 깜빡인다. 이 헬퍼는 폼 배경색 커버 패널을
    /// 폼 위에 덮어 두었다가, 폼이 표시되고 WPF 초기 레이아웃이 끝나는 시점
    /// (디스패처 유휴 — 행 생성 등 Background 우선순위 작업보다 늦게 실행)에
    /// 걷어서 완성된 화면만 한 번에 보이게 한다.
    ///
    /// 사용법 — 각 폼 생성자에서 <c>InitializeComponent()</c> 직후:
    /// <code>
    /// Modern.Lab.WinForms.Controls.Hosting.ModernLoadCover.Attach(this);
    /// </code>
    ///
    /// 폼 스스로 커버를 덮으므로 **메인 프레임(폼을 여는 쪽)을 고칠 수 없어도**
    /// 적용된다 — 별도 창(Show/ShowDialog)이든 패널 임베드(TopLevel=false)든
    /// 여는 방식과 무관하게 동작한다. 커버는 Load(폼이 처음 표시되는 시점)
    /// 이후의 디스패처 유휴 시점에 제거되므로, 폼을 만들어 두고 나중에 표시하는
    /// 프레임에서도 너무 일찍 걷히지 않는다.
    /// </summary>
    public static class ModernLoadCover
    {
        /// <summary>폼에 로딩 커버를 덮는다 — 생성자에서 1회 호출.</summary>
        public static void Attach(Form form)
        {
            if (form == null)
            {
                return;
            }

            // 폼 배경색 커버 — Dock은 다른 도킹 컨트롤이 자리를 깎은 나머지만
            // 받으므로 쓰지 않고, 전체 클라이언트 영역을 Bounds + 4방향 Anchor로
            // 덮는다 (프레임이 폼 크기를 바꿔도 앵커가 따라간다).
            Panel loadCover = new Panel();
            loadCover.BackColor = form.BackColor;
            loadCover.Bounds = form.ClientRectangle;
            loadCover.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                    | AnchorStyles.Left | AnchorStyles.Right;
            form.Controls.Add(loadCover);
            loadCover.BringToFront();

            // Load(처음 표시) 후 디스패처가 유휴 상태가 되면 걷는다 — 이때가
            // 초기 데이터 바인딩과 WPF 레이아웃 패스가 모두 끝난 시점이다.
            form.Load += delegate
            {
                System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                        new Action(delegate
                        {
                            if (!loadCover.IsDisposed && loadCover.Parent != null)
                            {
                                loadCover.Parent.Controls.Remove(loadCover);
                                loadCover.Dispose();
                            }
                        }));
            };
        }
    }
}
