using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ExpertLib.Dialogs
{
    public partial class LogDialog : Form
    {
        public LogDialog()
        {
            InitializeComponent();
            foreach(string log in Log.logs)
            {
                if (log.Length <= 1) continue;
                string type = log.Substring(0, 1);
                string value = log.Substring(1);
                if (type == "i")
                    _box.AppendTextColorful(value, Color.Black);
                else if (type == "w")
                    _box.AppendTextColorful(value, Color.Orange);
                else if(type=="e")
                    _box.AppendTextColorful(value, Color.Red);
                else
                    _box.AppendTextColorful(value, Color.Blue);
            }
            _box.SelectionLength = 0;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog {FileName = "运行日志.rtf"};
            if (dialog.ShowDialog() != DialogResult.OK) return;
            _box.SaveFile(dialog.FileName, RichTextBoxStreamType.RichText);
            MessageBox.Show("文件已成功保存");
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.Clear();
            _box.Clear();
        }
    }


    public static class Log
    {
        public static List<string> logs = new List<string>();

        public static DateTime Begin;

        public static DateTime Last;

        public static string i(string value)
        {
            return WriteLog("i", value);
        }

        public static void w(string value)
        {
            WriteLog("w", value);
        }

        public static void e(string value)
        {
            WriteLog("e", value);
        }

        private static string WriteLog(string type, string value)
        {
            var Now = DateTime.Now;

            var seconds = $"{Math.Ceiling((Now - Begin).TotalMilliseconds),-6}";
            var lastSeconds = $"{Math.Ceiling((Now - Last).TotalMilliseconds),-4}";
            logs.Add(type + Now.ToString("HH:mm:ss") + "[" + seconds + "](" + lastSeconds + ")-> " + value);
            Last = Now;

            return $"{Now:HH:mm:ss} {value}";
        }

        public static void AppendTextColorful(this RichTextBox rtBox, string text, Color color, bool addNewLine = true)
        {
            if (addNewLine)
            {
                text += Environment.NewLine;
            }
            rtBox.SelectionStart = rtBox.TextLength;
            rtBox.SelectionLength = 0;
            rtBox.SelectionColor = color;
            rtBox.AppendText(text);
            rtBox.SelectionColor = rtBox.ForeColor;
        }

        public static void Start()
        {
            Begin = DateTime.Now;
            Last = DateTime.Now;
        }

        public static void Show()
        {
            var dialog = new LogDialog();
            dialog.ShowDialog();
        }

        public static void Clear()
        {
            logs.Clear();
            Start();
        }
    }
}
