using System;
using System.Collections.Generic;

namespace ExpertLib.Controls
{
    public partial class TitleDate : TitleControl
    {
        public TitleDate()
        {
            InitializeComponent();
        }

        public DateTime Date => dateTimePicker1.Value.Date;

        public void SetValueChange(EventHandler handler)
        {
            dateTimePicker1.ValueChanged += handler;
            //dateEdit1.EditValueChanged += handler;
        }

        public override string Value()
        {
            return dateTimePicker1.Value.Date.ToString("yyyy/MM/dd");
            //return dateEdit1.EditValue.ToString();
        }

        

        public override void SetReadOnly(bool only)
        {
            dateTimePicker1.Enabled = !only;
            //dateEdit1.ReadOnly = only;
        }

        public override void SetValue(string only)
        {
            if (DateTime.TryParse(only, out var date))
                dateTimePicker1.Value = date.Date;
        }

        public void SetValue(DateTime only)
        {
            dateTimePicker1.Value = only.Date;
        }

        public override void SetItems(List<string> only)
        {
            ;
        }
    }
}
