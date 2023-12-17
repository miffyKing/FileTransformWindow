using System;
using System.Windows.Forms;
using System.Threading;

namespace checkFileContent
{
    internal static class Program
    {
        private const string AppMutex = "##MyUniqueWinFormsAppMutexName##";

        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, AppMutex, out bool createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    MessageBox.Show("이미 동일 프로그램이 실행중입니다.");
                }
            }
        }
    }
}
