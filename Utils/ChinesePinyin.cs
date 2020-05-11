using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;

namespace ExpertLib.Utils
{
    public static class ChinesePinyin
    {
        /// <summary> 
    /// 汉字转化为拼音
    /// </summary> 
    /// <param name="str">汉字</param> 
    /// <returns>全拼</returns> 
        public static string GetPinyin(string str)
    {
        string r = string.Empty;
        foreach (char obj in str)
        {
            try
            {
                ChineseChar chineseChar = new ChineseChar(obj);
                string t = chineseChar.Pinyins[0].ToString();
                r += t.Substring(0, t.Length - 1);
            }
            catch
            {
                r += obj.ToString();
            }
        }
        return r;
    }

        /// <summary> 
    /// 汉字转化为拼音首字母
    /// </summary> 
    /// <param name="str">汉字</param> 
    /// <returns>首字母</returns> 
        public static string GetFirstPinyin(this string str)
    {
        string r = string.Empty;
        foreach (char obj in str)
        {
            try
            {
                ChineseChar chineseChar = new ChineseChar(obj);
                string t = chineseChar.Pinyins[0].ToString();
                r += t.Substring(0, 1);
            }
            catch
            {
                r += obj.ToString();
            }
        }
        return r;
    }

        public static bool ContainsChineseWord(this string text)
        {
            char[] c = text.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] >= 0x4e00 && c[i] <= 0x9fbb)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAllChinese(this string text)
        {
            int len = text.Length;
            int chineseLen = 0;
            char[] c = text.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] >= 0x4e00 && c[i] <= 0x9fbb)
                {
                    chineseLen++;
                }
            }

            if (len == chineseLen)
                return true;
            else
                return false;
        }
    }
}