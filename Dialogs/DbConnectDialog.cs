using System;
using System.ComponentModel;
using System.Windows.Forms;
using ExpertLib.DataBase;
using static ExpertLib.Utils.NativeMethods;

namespace ExpertLib.Dialogs
{
    public partial class DbConnectDialog : Form
    {
        private const int timeout = 4;
        private SqlHelper Sql;

        public DbConnectDialog()
        {
            InitializeComponent();
        }

        private void FormDataBase_Load(object sender, EventArgs e)
        {
            Sql = new SqlHelper();
            txtDb.Text = SqlHelper.DataBase;
            txtServer.Text = SqlHelper.ServerIP;
            txtPwd.Text = SqlHelper.Password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "正在连接服务器....";
            statusStrip1.Visible = true;
            _bar.Value = 0;
            _bar.Maximum = timeout * 2;
            timer1.Enabled = true;
            var args = new[] { txtServer.Text.Trim(), txtPwd.Text.Trim(), txtDb.Text.Trim() };
            _worker.RunWorkerAsync(args);
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var server = (e.Argument as string[])?[0];
            var pwd = (e.Argument as string[])?[1];
            var db = (e.Argument as string[])?[2];

            if (Sql.TestConnection(timeout, out var errorInfo, server, pwd, db))
            {
                MessageBox.Show("连接成功");
                e.Result = "连接成功";
            }
            else
            {
                MessageBox.Show("详细信息请点击日志按钮查看", "连接失败");
                e.Result = "连接失败";
            }
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtServer.Focus();
            button1.Enabled = true;
            button1.Text = "连接";
            statusStrip1.Visible = false;
            timer1.Enabled = false;
            if (!(e.Result is string) || (string) e.Result != "连接成功") 
                return;

            WritePrivateProfileString("a", "server", txtServer.Text, SqlHelper.IniPath);
            WritePrivateProfileString("a", "pwd", txtPwd.Text, SqlHelper.IniPath);
            WritePrivateProfileString("a", "database", txtDb.Text, SqlHelper.IniPath);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _bar.PerformStep();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Log.Show();
        }
    }
}
