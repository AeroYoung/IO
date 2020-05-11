// 根据SharpZipLib的代码进行修改
// 主要改进 
//    1> 修改内部表达式结构为List<Regex> ，从而获得更好的性能
//    2> 增加部分函数的参数验证
//    3> 修改函数说明为中文

namespace ExpertLib.IO
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ExpertLib;

    /// <summary>
    /// StringFilter是一个充许正匹配和负匹配的字符串过滤器.
    /// A filter is a sequence of independant <see cref="Regex">regular expressions</see> separated by semi-colons ';'
    /// Each expression can be prefixed by a plus '+' sign or a minus '-' sign to denote the expression
    /// is intended to include or exclude names.  If neither a plus or minus sign is found include is the default
    /// A given name is tested for inclusion before checking exclusions.  Only names matching an include spec 
    /// and not matching an exclude spec are deemed to match the filter.
    /// An empty filter matches any name.
    /// </summary>
    /// <example>The following expression includes all name ending in '.dat' with the exception of 'dummy.dat'
    /// "+\.dat$;-^dummy\.dat$"
    /// </example>
    public class StringFilter : IScanFilter
    {
        #region 构造器
        /// <summary>
        /// 创建一个字符串过滤器
        /// </summary>
        /// <param name="filter">过滤器表达式</param>
        /// <remarks>如果是空字符串，或非正确的过滤器表达式都会产生一个参数异常
        /// </remarks>
        public StringFilter(string filter)
        {
            ArgumentValidation.CheckForEmptyString(filter,"filter");
            
            if (!IsValidFilterExpression(filter))
            {
                throw new ArgumentException(SR.ExceptionInvalidFilterExpress(filter));
            }

            filter_ = filter;
            inclusions_ = new List<Regex>();
            exclusions_ = new List<Regex>();
            Compile();
        }
        #endregion

        #region IsValidExpression
        /// <summary>
        /// 测试一个字符串是否合法的正达表达式.
        /// </summary>
        /// <param name="expression">需要测试的表达式</param>
        /// <returns>True if expression is a valid <see cref="System.Text.RegularExpressions.Regex"/> false otherwise.</returns>
        public static bool IsValidExpression(string expression)
        {
            bool result = true;
            try
            {
                Regex exp = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
            catch
            {
                result = false;
            }
            return result;
        }
        #endregion

        #region IsValidFilterExpression
        /// <summary>
        /// 测试一个表达式是否符合Filter
        /// </summary>
        /// <param name="toTest">The filter expression to test.</param>
        /// <returns>True if the expression is valid, false otherwise.</returns>
        public static bool IsValidFilterExpression(string toTest)
        {
            ArgumentValidation.CheckForEmptyString(toTest, "toTest");

            bool result = true;

            try
            {
                string[] items = toTest.Split(';');
                for (int i = 0; i < items.Length; ++i)
                {
                    if (items[i] != null && items[i].Length > 0)
                    {
                        string toCompile;

                        if (items[i][0] == '+')
                        {
                            toCompile = items[i].Substring(1, items[i].Length - 1);
                        }
                        else if (items[i][0] == '-')
                        {
                            toCompile = items[i].Substring(1, items[i].Length - 1);
                        }
                        else
                        {
                            toCompile = items[i];
                        }

                        Regex testRegex = new Regex(toCompile, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region ToString
        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <returns>The string equivalent for this filter.</returns>
        public override string ToString()
        {
            return filter_;
        }
        #endregion

        #region IsIncluded
        /// <summary>
        /// 测试字符串是否在包含匹配中
        /// </summary>
        /// <param name="name">The value to test.</param>
        /// <returns>True if the value is included, false otherwise.</returns>
        public bool IsIncluded(string name)
        {
            ArgumentValidation.CheckForEmptyString(name, "name");

            bool result = false;
            if (inclusions_.Count == 0)
            {
                result = true;
            }
            else
            {
                foreach (Regex r in inclusions_)
                {
                    if (r.IsMatch(name))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion

        #region IsExcluded
        /// <summary>
        /// 测试字符串是否排除匹配中
        /// </summary>
        /// <param name="name">The value to test.</param>
        /// <returns>True if the value is excluded, false otherwise.</returns>
        public bool IsExcluded(string name)
        {
            bool result = false;
            foreach (Regex r in exclusions_)
            {
                if (r.IsMatch(name))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        #endregion

        #region IsMatch
        /// <summary>
        /// 测试是否匹配.
        /// </summary>
        /// <param name="name">The value to test.</param>
        /// <returns>True if the value matches, false otherwise.</returns>
        public bool IsMatch(string name)
        {
            return (IsIncluded(name) == true) && (IsExcluded(name) == false);
        }
        #endregion

        #region private 
        /// <summary>
        /// Compile this filter.
        /// </summary>
        void Compile()
        {
            string[] items = filter_.Split(';');
            for (int i = 0; i < items.Length; ++i)
            {
                if ((items[i] != null) && (items[i].Length > 0))
                {
                    bool include = (items[i][0] != '-');
                    string toCompile;

                    if (items[i][0] == '+')
                    {
                        toCompile = items[i].Substring(1, items[i].Length - 1);
                    }
                    else if (items[i][0] == '-')
                    {
                        toCompile = items[i].Substring(1, items[i].Length - 1);
                    }
                    else
                    {
                        toCompile = items[i];
                    }

                    if (include)
                    {
                        inclusions_.Add(new Regex(toCompile, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
                    }
                    else
                    {
                        exclusions_.Add(new Regex(toCompile, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
                    }
                }
            }
        }
        #endregion

        #region Instance Fields
        string filter_;
        List<Regex> inclusions_;
        List<Regex> exclusions_;
        #endregion
    }
}
