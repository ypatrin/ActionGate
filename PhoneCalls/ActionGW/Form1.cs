using action_gw.PhoneCalls;
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
                notifyIcon1.BalloonTipTitle = "PhoneCalls Service";
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
            writeLog("Загрузка конфигурации сервиса");

            Properties.Settings.Default.ActionUser = config.IniReadValue("Action", "Login", "976625");
            Properties.Settings.Default.ActionPassword = config.IniReadValue("Action", "Pass", "SADFASDF0980982354LKJ");
            Properties.Settings.Default.ActionDefaultCompanyId = config.IniReadValue("Action", "DefaultCompanyGUID", "84016bef-1846-4ab3-9cab-810cbf1ba3e0");

            Properties.Settings.Default.MysqlUser = config.IniReadValue("MySQL", "User", "root");
            Properties.Settings.Default.MysqlPass = config.IniReadValue("MySQL", "Pass", "");
            Properties.Settings.Default.MysqlHost = config.IniReadValue("MySQL", "Host", "localhost");
            Properties.Settings.Default.MysqlDB = config.IniReadValue("MySQL", "DB", "action");

            string Preffix = "http://" + config.IniReadValue("Service", "PhoneCallsIface", "localhost:8181") + "/";

            writeLog("Проверяем подключение к БД");

            if (DB.connect())
            {
                ws = new WebServer(Preffix);
                ws.Run();

                StartBtn.Enabled = false;
                StopBtn.Enabled = true;
                StartServerMnu.Enabled = false;
                StopServerMnu.Enabled = true;

                updater_phone_calls.Enabled = true;
                UPCbtn.Enabled = true;

                notifyIcon1.Icon = Properties.Resources.play_ico;

                writeLog("Сервер успешно запущен и доступен по адресу " + Preffix);
            }
            else
            {
                writeLog("Неудается запустить сервер из-за ошибки БД");
                MessageBox.Show("Неудается запустить сервер из-за ошибки БД", "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void stopServer()
        {
            writeLog("Остановка сервера");

            ws.Stop();

            StartBtn.Enabled = true;
            StopBtn.Enabled = false;
            StartServerMnu.Enabled = true;
            StopServerMnu.Enabled = false;

            updater_phone_calls.Enabled = false;
            UPCbtn.Enabled = false;

            notifyIcon1.Icon = Properties.Resources.stop_ico;

            DB.disconnect();

            writeLog("Сервер успешно остановлен");
        }

        protected void updatePhoneCalls()
        {
            writeLog("Начинаем обновление звонков");

            Action Action = new Action();
            GetPhoneCallsData[] phoneCalls = Action.getPhoneCalls();

            DB.updatePhoneCalls(phoneCalls);
            writeLog("Звонки обновлены");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread myThread = new Thread(updatePhoneCalls);
            myThread.Start();
        }

        private void updater_phone_calls_Tick(object sender, EventArgs e)
        {
            updatePhoneCalls();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InterfaceBox.Text = config.IniReadValue("Service", "PhoneCallsIface", "localhost:8181");

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
