using System;
using System.Threading;
using System.Windows.Forms;

namespace HIMAWARI_8
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(false, Application.ProductName, out bool created);
            if (!created)
            {
                MessageBox.Show("程序已运行！");
                Application.Exit();
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
