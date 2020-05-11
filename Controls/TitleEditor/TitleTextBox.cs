using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExpertLib.Controls
{
    public partial class TitleTextBox : TitleControl
    {
        public TitleTextBox()
        {
            InitializeComponent();
        }

        public void SetTextChange(EventHandler handler)
        {
            textBoxEx1.TextChanged += handler;
        }

        public override string Text => textBoxEx1.Text;

        public override string Value()
        {
            return textBoxEx1.Text.Trim();
        }

        public override void SetReadOnly(bool only)
        {
            textBoxEx1.Enabled = !only;
        }

        public override void SetValue(string only)
        {
            textBoxEx1.Text = only.Trim();
        }

        [Description("最大值"), Category("自定义")]
        public decimal MaxValue
        {
            get => textBoxEx1.MaxValue;
            set => textBoxEx1.MaxValue = value;
        }

        [Description("掩码"), Category("自定义")]
        public char PasswordChar
        {
            get => textBoxEx1.PasswordChar;
            set => textBoxEx1.PasswordChar = value;
        }

        [Description("水印文字"), Category("自定义")]
        public string PromptText
        {
            get => textBoxEx1.PromptText;
            set => textBoxEx1.PromptText = value;
        }

        [Description("获取或设置一个值，该值指示文本框中的文本输入类型。"), Category("自定义")]
        public TextInputType InputType
        {
            get => textBoxEx1.InputType;
            set => textBoxEx1.InputType = value;
        }

        [Description("获取或设置一个值，该值指示当输入类型InputType=Regex时，使用的正则表达式。"), Category("自定义")]
        public string RegexPattern
        {
            get => textBoxEx1.RegexPattern;
            set => textBoxEx1.RegexPattern = value;
        }

        [Description("多行文本"), Category("自定义")]
        public bool Multiline
        {
            get => textBoxEx1.Multiline;
            set => textBoxEx1.Multiline = value;
        }
    }
}
