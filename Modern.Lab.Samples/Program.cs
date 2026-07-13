using System;
using System.Windows.Forms;

namespace Modern.Lab.Samples
{
    /// <summary>샘플 진입점. WPF 호스팅을 위해 STA 스레드에서 실행된다.</summary>
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 다크 테마 확인용: "--dark" 인자로 실행하면 다크로 뜬다.
            // 테마는 반드시 첫 컨트롤(폼) 생성 전에 설정해야 한다 —
            // 실사용 앱도 Program.cs의 이 위치에서 설정하면 된다.
            foreach (string arg in args)
            {
                if (string.Equals(arg, "--dark", StringComparison.OrdinalIgnoreCase))
                {
                    Modern.Lab.Theming.ModernTheme.Mode = Modern.Lab.Theming.ModernTheme.ThemeMode.Dark;
                }
            }

            // 셸이 모든 샘플 화면을 호스팅한다; 새 화면은
            // SampleShellForm.RegisterSamples에서 AddSample 호출 하나로 추가한다.
            Application.Run(new SampleShellForm());
        }
    }
}
