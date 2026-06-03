using System;
using System.Windows.Forms;

namespace DateManager
{
    internal static class Program
    {
        // OS에게 직접 DPI 인식을 명령하는 Windows API 선언
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            // 👈 이 코드를 맨 첫 줄에 실행하면 뿌연 현상이 거짓말처럼 사라집니다.
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LauncherForm());
        }
    }
}