using System;
using System.Windows.Forms;

namespace Modern.Lab.Samples
{
    /// <summary>Sample entry point. Runs on an STA thread for WPF hosting.</summary>
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // The employee management sample runs standalone for now; switch
            // back to SampleShellForm once multiple samples exist again.
            Application.Run(new EmployeeManagementForm());
        }
    }
}
