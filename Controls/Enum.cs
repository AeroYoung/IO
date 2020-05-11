using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertLib.Controls
{
    public enum Func
    {
        [Description("计数")]
        Count = 1,

        [Description("求和")]
        Sum = 1,
    }

    public enum TimeUnit
    {
        NoTimeUnit = 0,

        Day = 1,

        Week = 2,

        Month = 3,

        Season = 4,

        Year = 5
    }

    public enum TextInputType
    {
        [Description("不控制输入")]
        NotControl = 1,
        
        [Description("任意数字")]
        Number = 2,
        
        [Description("非负数")]
        UnsignNumber = 4,
        
        [Description("正数")]
        PositiveNumber = 8,
        
        [Description("整数")]
        Integer = 16,
        
        [Description("非负整数")]
        PositiveInteger = 32,
        
        [Description("正则验证")]
        Regex = 64
    }
}
