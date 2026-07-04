using System;
using System.Windows.Forms;

namespace Modern.Lab.Samples
{
    /// <summary>샘플 진입점. WPF 호스팅을 위해 STA 스레드에서 실행된다.</summary>
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 셸이 모든 샘플 화면을 호스팅한다; 새 화면은
            // SampleShellForm.RegisterSamples에서 AddSample 호출 하나로 추가한다.
            Application.Run(new SampleShellForm());
        }
    }
}
