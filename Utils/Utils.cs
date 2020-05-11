using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using ExpertLib.Dialogs;

namespace ExpertLib.Utils
{
    public static class Utils
    {
        /// <summary>  
        /// 转换方法  
        /// </summary>  
        /// <param name="size">字节值</param>  
        /// <returns></returns>  
        public static string HumanReadableFileSize(this double size)
        {
            var unit = HumanReadableFileSize(ref size);
            return Math.Round(size, 2) + unit;
        }

        public static string HumanReadableFileSize(ref double size)
        {
            var units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            size = Math.Round(size, 2);
            return units[i];
        }

        /// <summary>
        /// 合并两个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static T[] MergeArray<T>(this T[] First, T[] Second)
        {
            var result = new T[First.Length + Second.Length];
            First.CopyTo(result, 0);
            Second.CopyTo(result, First.Length);
            return result;
        }

        /// <summary>
        /// 从对象中获得日期
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool ToDate(this object obj, out DateTime date)
        {
            if (obj == null)
            {
                date = DateTime.Now.Date;
                return false;
            }

            var s = obj.ToString();

            if (DateTime.TryParse(s, out date))
                return true;

            if (DateTime.TryParseExact(s, new[] { "dd-M月-yyyy" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date))
                return true;

            if (DateTime.TryParseExact(s, new[] { "yyyy.MM.dd" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date))
                return true;

            if (DateTime.TryParseExact(s, new[] { "yyyy.M.d" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date))
                return true;

            date = DateTime.Now.Date;

            return false;
        }

        /// <summary>
        /// DataRow Copy Value 
        /// </summary>
        /// <param name="sourceRow"></param>
        /// <param name="targetRow"></param>
        /// <param name="fieldName"></param>
        public static void CopyValue(this DataRow sourceRow, DataRow targetRow, string fieldName)
        {
            try
            {
                var source = sourceRow[fieldName];
                var targetCol = targetRow.Table.Columns[fieldName];

                if (targetCol == null)
                    return;

                var targetDataType = targetCol.DataType;

                if (targetDataType == typeof(DateTime))
                {
                    if (source.ToDate(out var date))
                        targetRow[fieldName] = date;
                }
                else if (targetDataType == typeof(float) || targetDataType == typeof(double))
                {
                    double.TryParse(source.ToString(), out var value);
                    targetRow[fieldName] = value;
                }
                else if (targetDataType == typeof(int))
                {
                    int.TryParse(source.ToString(), out var value);
                    targetRow[fieldName] = value;
                }
                else
                {
                    targetRow[fieldName] = source;
                }
            }
            catch (Exception e)
            {
                Log.e($"CopyValue {e.Message}");
            }
            
        }

        public static double CorrectValue(this DataRow dr, string fieldName, double defaultValue)
        {
            if (double.TryParse(dr[fieldName].ToString(), out var value)) 
                return value;

            value = defaultValue;
            dr[fieldName] = defaultValue.ToString();

            return value;
        }

        public static DateTime CorrectValue(this DataRow dr, string fieldName, DateTime defaultValue)
        {
            if (dr[fieldName].ToDate(out var value))
                return value;

            value = defaultValue;
            dr[fieldName] = defaultValue.ToString();

            return value;
        }

        /// <summary>
        /// 当年第几周
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime date)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        /// <summary>  
        /// 得到本周第一天(以星期一为第一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekFirstDayMon(this DateTime datetime)
        {
            //星期一为第一天  
            var weekNow = Convert.ToInt32(datetime.DayOfWeek);
            //因为是以星期一为第一天，所以要判断weekNow等于0时，要向前推6天。  
            weekNow = weekNow == 0 ? 7 - 1 : weekNow - 1;
            //本周第一天  
            return datetime.AddDays((-1) * weekNow).Date;
        }

        /// <summary>  
        /// 得到本周最后一天(以星期天为最后一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekLastDaySun(this DateTime datetime)
        {
            //星期天为最后一天  
            var weekNow = Convert.ToInt32(datetime.DayOfWeek);
            weekNow = (weekNow == 0 ? 7 : weekNow);
            return datetime.AddDays(7 - weekNow).Date;
        }

        /// <summary>
        /// 得到季度第一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetSeasonBegin(this DateTime datetime)
        {
            var d = datetime.AddMonths(0 - ((DateTime.Now.Month - 1) % 3));
            return new DateTime(d.Year, d.Month, 1);
        }

        /// <summary>
        /// 得到季度最后一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetSeasonEnd(this DateTime datetime)
        {
            var ThisMonth = datetime.Month;
            var FirstMonthOfSeason = ThisMonth - (ThisMonth % 3 == 0 ? 3 : (ThisMonth % 3)) + 3;
            datetime = datetime.AddMonths(FirstMonthOfSeason - ThisMonth);

            return datetime.AddMonths(1).AddDays(-1);
        }
    }
}
