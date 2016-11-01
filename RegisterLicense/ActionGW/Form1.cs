using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace action_gw
{
    public partial class Form1 : Form
    {
        string[] args;

        Log Log = Log.getInstance();
        DB DB = new DB();
        WebServer ws;

        IniFile config = new IniFile(Application.StartupPath + @"\config.ini");

        public Form1(string[] args)
        {
            this.args = args;
            InitializeComponent();
        }

        public string SendResponse(HttpListenerRequest request)
        {
            string Method;
            string[] RequestString;

            string[] Req = request.RawUrl.Split("?".ToCharArray());
            Req[0] = Req[0].Replace("/", "");

            Method = Req[0];
            RequestString = Req[1].Split("&".ToCharArray());

            return string.Format("<HTML><BODY>XML Server.<br>{0}<hr/>Method: {1}<br>Request: {2}</BODY></HTML>", DateTime.Now, Method);
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            this.startServer();
        }

        public void writeLog(String text)
        {
            Log.add(text);
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            this.stopServer();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();

                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "Сервер все еще работает...";
                notifyIcon1.BalloonTipTitle = "RegisterLicense Service";
                notifyIcon1.ShowBalloonTip(500);
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.startServer();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.stopServer();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        protected void startServer()
        {
            string Preffix = "http://" + config.IniReadValue("Service", "RegisterLicense", "localhost:8181") + "/";

            writeLog("Запуск http сервера");

            ws = new WebServer(SendResponse, Preffix);
            ws.Run();

            StartBtn.Enabled = false;
            StopBtn.Enabled = true;
            InterfaceBox.Enabled = false;
            StartServerMnu.Enabled = false;
            StopServerMnu.Enabled = true;

            notifyIcon1.Icon = Properties.Resources.play_ico;

            writeLog("Сервер успешно запущен и доступен по адресу " + Preffix);
        }

        protected void stopServer()
        {
            writeLog("Остановка сервера");

            ws.Stop();

            StartBtn.Enabled = true;
            StopBtn.Enabled = false;
            InterfaceBox.Enabled = true;
            StartServerMnu.Enabled = true;
            StopServerMnu.Enabled = false;

            notifyIcon1.Icon = Properties.Resources.stop_ico;

            writeLog("Сервер успешно остановлен");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InterfaceBox.Text = config.IniReadValue("Service", "RegisterLicense", "localhost:8181");

            if (this.args.Length > 0 && this.args[0] == "--autostart")
            {
                this.startServer();

                this.WindowState = FormWindowState.Minimized;
                this.Hide();

                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "Сервер все еще работает...";
                notifyIcon1.BalloonTipTitle = "PhoneCalls Service";
            }
        }
    }
}
