namespace ExpertLib.Utils
{
    /// <summary>
    /// EnumRegex
    /// 枚举状态的说明。
    /// 
    /// 修改纪录
    /// 
    ///		2017.05.03 版本：1.0 AeroYoung 创建。
    ///		
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>AeroYoung</name>
    ///		<date>2017.05.03</date>
    /// </author> 
    /// </summary>    
    public static class RegexCollection 
    {
        public static string PositiveInteger = @"^[0-9]*[1-9][0-9]*$";// 正整数正则表达式
        public static string Integer = @"^-?\d+$";// 整数正则表达式
        public static string Float = @"^[+|-]?\d*\.?\d*$";// 浮点数正则表达式
    }
}