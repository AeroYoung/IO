using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpertLib.Controls
{
    public partial class TitleControl : UserControl
    {
        public TitleControl()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => label1.Text;
            set => label1.Text = value;
        }

        public bool OriginalReadOnly;

        [Description("文本长度"), Category("自定义属性")]
        public int LabelWidth
        {
            get => label1.Width;
            set => label1.Width = value;
        }

        public virtual string Value()
        {
            throw new Exception("TitleControl SetReadOnly");
        }

        public virtual void SetReadOnly(bool only)
        {
            throw new Exception("TitleControl SetReadOnly");
        }

        public virtual void SetValue(string only)
        {
            throw new Exception("TitleControl SetReadOnly");
        }

        public virtual void SetItems(List<string> only)
        {
            ;
        }
    }
}
