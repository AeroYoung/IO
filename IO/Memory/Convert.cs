//本文件包括一些常用的，CLR未提供的转换类
//目前版本包括以下转换类
//    DateTimeConvert  :   时间转换类

namespace ExpertLib
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices.ComTypes;

    #region DateTimeConvert
    /// <summary>
    /// 时间转换类
    /// </summary>
    /// <remarks>用于FILETIME与DateTime的转换</remarks>
    public static class DateTimeConvert
    {
        /// <summary>
        /// 将DateTime转换成FILETIME结构
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static System.Runtime.InteropServices.ComTypes.FILETIME ToFILETIME(DateTime dt)
        {
            System.Runtime.InteropServices.ComTypes.FILETIME ft;
            long hFT1 = dt.ToFileTimeUtc();
            ft.dwLowDateTime = (int)(hFT1 & 0xFFFFFFFF);
            ft.dwHighDateTime = (int)(hFT1 >> 32);
            return ft;
        }

        /// <summary>
        /// 将FILETIME转换成DateTime
        /// </summary>
        /// <param name="ft"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME ft)
        {
            long hFT2 = (((long)ft.dwHighDateTime) << 32) + ft.dwLowDateTime;
            DateTime dt = DateTime.FromFileTimeUtc(hFT2);
            return dt;
        }
    }
    #endregion

    #region ValueConvert
    /// <summary>
    /// 常见的数值转换，就是内存表示相同，而可能各表示的值不一样
    /// </summary>
    /// <remarks>
    /// 如int 型与uint型在内存空间里是一样大小，而可能表示的值是不一样的，但是我们可以
    /// 通过以下的函数进行转换，而不需要用到CopyMemory之类的函数
    /// </remarks>
    public static class ValueConvert
    {
        /// <summary>
        /// uint 转成内存表示相同的int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt32(uint value)
        {
            byte[] bytes = System.BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static uint ToUInt32(int value)
        {
            byte[] bytes = System.BitConverter.GetBytes(value);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static short ToInt16(ushort value)
        {
            byte[] bytes = System.BitConverter.GetBytes(value);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort ToUInt16(short value)
        {
            byte[] bytes = System.BitConverter.GetBytes(value);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static long ToInt64(ulong value)
        {
            byte[] bytes = System.BitConverter.GetBytes(value);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static ulong ToUInt64(long value)
        {
            byte[] bytes = System.BitConverter.GetBytes(value);
            return BitConverter.ToUInt64(bytes, 0);
        }

    }
    #endregion
}
