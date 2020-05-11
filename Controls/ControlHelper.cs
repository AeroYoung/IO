using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ExpertLib.Controls
{
    /// <summary>
    /// Class ControlHelper.
    /// </summary>
    public static class ControlHelper
    {
        /// <summary>
        /// 设置GDI高质量模式抗锯齿
        /// </summary>
        /// <param name="g">The g.</param>
        public static void SetGDIHigh(this Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
        }

        /// <summary>
        /// 功能描述:检查文本控件输入类型是否有效
        /// </summary>
        /// <param name="strValue">值</param>
        /// <param name="inputType">控制类型</param>
        /// <param name="decMaxValue">最大值</param>
        /// <param name="decMinValue">最小值</param>
        /// <param name="intLength">小数位长度</param>
        /// <param name="strRegexPattern">正则</param>
        /// <returns>返回值</returns>
        public static bool CheckInputType(
            string strValue,
            TextInputType inputType,
            decimal decMaxValue = default(decimal),
            decimal decMinValue = default(decimal),
            int intLength = 2,
            string strRegexPattern = null)
        {
            bool result;
            switch (inputType)
            {
                case TextInputType.NotControl:
                    result = true;
                    return result;
                case TextInputType.UnsignNumber:
                    if (string.IsNullOrEmpty(strValue))
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        if (strValue.IndexOf("-") >= 0)
                        {
                            result = false;
                            return result;
                        }
                    }
                    break;
                case TextInputType.Number:
                    if (string.IsNullOrEmpty(strValue))
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        if (!Regex.IsMatch(strValue, "^-?\\d*(\\.?\\d*)?$"))
                        {
                            result = false;
                            return result;
                        }
                    }
                    break;
                case TextInputType.Integer:
                    if (string.IsNullOrEmpty(strValue))
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        if (!Regex.IsMatch(strValue, "^-?\\d*$"))
                        {
                            result = false;
                            return result;
                        }
                    }
                    break;
                case TextInputType.PositiveInteger:
                    if (string.IsNullOrEmpty(strValue))
                        return true;
                    else
                    {
                        if (!Regex.IsMatch(strValue, "^\\d+$"))
                            return false;
                    }
                    break;
                case TextInputType.Regex:
                    result = (string.IsNullOrEmpty(strRegexPattern) || Regex.IsMatch(strValue, strRegexPattern));
                    return result;
            }
            if (strValue == "-")
            {
                return true;
            }
            decimal d;
            if (!decimal.TryParse(strValue, out d))
            {
                result = false;
            }
            else if (d < decMinValue || d > decMaxValue)
            {
                result = false;
            }
            else
            {
                if (inputType == TextInputType.Number || inputType == TextInputType.UnsignNumber || inputType == TextInputType.PositiveNumber)
                {
                    if (strValue.IndexOf(".") >= 0)
                    {
                        string text = strValue.Substring(strValue.IndexOf("."));
                        if (text.Length > intLength + 1)
                        {
                            result = false;
                            return result;
                        }
                    }
                }
                result = true;
            }
            return result;
        }
    }
}
