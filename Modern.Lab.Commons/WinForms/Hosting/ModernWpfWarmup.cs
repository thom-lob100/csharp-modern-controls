using System;

namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// WPF 첫 화면 지연을 줄이는 워밍업 — 메인 프레임 시작 시(로그인 직후 등)
    /// 한 줄 호출한다.
    ///
    /// 모던 컨트롤이 있는 첫 화면을 열 때 WPF 어셈블리 로드·JIT·토큰 사전
    /// (Tokens.xaml) 파싱 비용이 한꺼번에 들어 느린 PC에서는 몇 초씩 걸린다
    /// (두 번째 화면부터 빠른 이유). 이 헬퍼는 화면을 열기 전에 대표 WPF
    /// 컨트롤을 미리 생성·레이아웃해 그 비용을 앞당겨 치른다 — 토큰 사전은
    /// SharedResourceDictionary 캐시에 적재되어 이후 모든 컨트롤이 재사용한다.
    ///
    /// 사용법 — 메인 프레임 시작 시(테마 <see cref="Modern.Lab.Theming.ModernTheme.Mode"/>
    /// 설정 **이후**, 첫 화면을 열기 전) UI(STA) 스레드에서 1회:
    /// <code>
    /// Modern.Lab.WinForms.Controls.Hosting.ModernWpfWarmup.Run();
    /// </code>
    /// </summary>
    public static class ModernWpfWarmup
    {
        private static bool completed;

        /// <summary>워밍업을 1회 실행한다 — 재호출은 무시된다.</summary>
        public static void Run()
        {
            if (completed)
            {
                return;
            }

            completed = true;

            try
            {
                // 대표 컨트롤 두 개를 생성 + 레이아웃한다:
                // - 오버레이: PresentationFramework 로드 + Tokens.xaml 파싱(캐시 적재)
                // - 그리드: 가장 무거운 컨트롤(DataGrid 템플릿) JIT
                // 화면에 붙이지 않으므로 그리는 비용은 없고, 끝나면 그대로 버린다.
                Modern.Lab.Controls.Wpf.Display.ModernBusyOverlayControl overlay =
                        new Modern.Lab.Controls.Wpf.Display.ModernBusyOverlayControl();
                overlay.Measure(new System.Windows.Size(300d, 180d));
                overlay.Arrange(new System.Windows.Rect(0d, 0d, 300d, 180d));

                Modern.Lab.Controls.Wpf.Data.ModernDataGridControl grid =
                        new Modern.Lab.Controls.Wpf.Data.ModernDataGridControl();
                grid.Measure(new System.Windows.Size(400d, 300d));
                grid.Arrange(new System.Windows.Rect(0d, 0d, 400d, 300d));
            }
            catch (Exception)
            {
                // 워밍업 실패는 무해하다 — 첫 화면이 원래 속도로 열릴 뿐이다.
            }
        }
    }
}
