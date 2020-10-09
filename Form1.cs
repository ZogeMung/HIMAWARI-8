using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HIMAWARI_8
{
    public partial class Form1 : Form
    {
        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(
        IntPtr hwnd,
        int nIndex,
        uint dwNewLong
        );

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(
        IntPtr hwnd,
        int crKey,
        int bAlpha,
        int dwFlags
        );
        public void SetPenetrate()
        {
            SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
            SetLayeredWindowAttributes(this.Handle, 0, 100, LWA_ALPHA);
        }

        public Form1()
        {
            InitializeComponent();
        }

        public string GetWebImageJson()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp("https://himawari8-dl.nict.go.jp/himawari8/img/FULL_24h/latest.json");
            request.Method = "GET";
            request.ReadWriteTimeout = 5000;
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            return retString;
        }

        public string[] SplitData(string splitbeforedata)
        {
            string[] splitafterdata;
            splitbeforedata = splitbeforedata.Substring(9, 19);
            splitafterdata = splitbeforedata.Split(new char[3] { '-', ' ', ':' });
            return splitafterdata;
        }

        public void InputImage()
        {
            string[] data = SplitData(GetWebImageJson());
            string url = "https://himawari8-dl.nict.go.jp/himawari8/img/D531106/1d/550/" + data[0] + "/" + data[1] + "/" + data[2] + "/" + data[3] + data[4] + data[5] + "_0_0.png";
            pictureBox1.ImageLocation = url;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                RegistryKey r_local = Registry.LocalMachine;
                RegistryKey r_run = r_local.OpenSubKey(@"software\microsoft\windows\currentversion\run", false);
                if (r_run.GetValue("HIMAWARI-8") == null)
                {
                    r_run.Close();
                    r_run = r_local.CreateSubKey(@"software\microsoft\windows\currentversion\run");
                    r_run.SetValue("HIMAWARI-8", Application.ExecutablePath);
                }
                r_run.Close();
                r_local.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("无权限，无法执行开机自启 ！");
            }
            string[] position;
            string positionstr = "";
            try
            {
                StreamReader sr = new StreamReader(Application.StartupPath + "\\HIMAWARI-8.txt", false);
                positionstr = sr.ReadLine().ToString();
                sr.Close();
            }
            catch (Exception)
            {
                StreamWriter sw = new StreamWriter(Application.StartupPath + "\\HIMAWARI-8.txt", false);
                sw.WriteLine("0,0");
                sw.Close();
            }
            if (positionstr != "")
            {
                position = positionstr.Split(',');
                this.Left = Convert.ToInt32(position[0]) - 225;
                this.Top = Convert.ToInt32(position[1]) - 225;
            }
            SetPenetrate();
            timer1.Enabled = true;
            InputImage();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            InputImage();
        }
    }
}
