using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ExpertLib
{
    /// <summary>
    /// 参数验证类
    /// </summary>
    public static class ArgumentValidation
    {
        #region CheckForEmptyString
        /// <summary>
        /// 检查是否空字符
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="variableName"></param>
        public static void CheckForEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);
            CheckForNullReference(variableName, "variableName");
            if (variable.Length == 0)
            {
                throw new ArgumentException(SR.ExceptionEmptyString(variableName));
				
            }
        }
        #endregion

        #region CheckForNullReference
        /// <summary>
        /// 检查参数是否NULL引用
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="variableName"></param>
        public static void CheckForNullReference(object variable, string variableName)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }

            if (null == variable)
            {
                throw new ArgumentNullException(variableName);
            }
        }
        #endregion

        #region CheckForZeroBytes
        /// <summary>
        /// 验证所传的字节数组是否为空或零长
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="variableName"></param>
        public static void CheckForZeroBytes(byte[] bytes, string variableName)
        {
            CheckForNullReference(bytes, "bytes");
            CheckForNullReference(variableName, "variableName");
            if (bytes.Length == 0)
            {
                throw new ArgumentException(SR.ExceptionByteArrayValueMustBeGreaterThanZeroBytes, variableName);
            }
        }
        #endregion

        #region CheckForZeroArray
        /// <summary>
        /// 检查参数是否零长的数组或空值，如是则抛出一个异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="paraName"></param>
        public static void CheckForZeroArray<T> (T[] value,string paraName)
        {
            CheckForNullReference(value, "paraName");
            CheckForNullReference(paraName, "paraName");
            if (value.Length == 0)
            {
                throw new ArgumentException(SR.ExceptionNullOrZeroArray(paraName));
            }
        }
        #endregion

        #region CheckExpectedType
        /// <summary>
        /// 验证一个值是否期待的类型
        /// </summary>
        /// <param name="variable">对象值</param>
        /// <param name="type">期待的类型</param>
        public static void CheckExpectedType(object variable, Type type)
        {
            CheckForNullReference(variable, "variable");
            CheckForNullReference(type, "type");
            if (!type.IsAssignableFrom(variable.GetType()))   //主要是继承类也可以
            {
                throw new ArgumentException(SR.ExceptionExpectedType(type.FullName));
            }
        }
        #endregion

        #region CheckEnumeration
        /// <summary>
        /// 验证一个值是不是枚举类型的可选值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="variable">验证的变量</param>
        /// <param name="variableName">变量的名称</param>
        public static void CheckEnumeration(Type enumType, object variable, string variableName)
        {
            CheckForNullReference(variable, "variable");
            CheckForNullReference(enumType, "enumType");
            CheckForNullReference(variableName, "variableName");

            if (!Enum.IsDefined(enumType, variable))
            {
                throw new ArgumentException(SR.ExceptionEnumerationNotDefined(variable.ToString(), enumType.FullName), variableName);
            }
        }
        #endregion

        #region CheckValueLimit
        /// <summary>
        /// 检测一个值是否在合理的数据范围内
        /// </summary>
        /// <typeparam name="T">只支持具有IComparable接口的对象</typeparam>
        /// <param name="value">要检可的值</param>
        /// <param name="paraName">参数名称</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public static void CheckValueLimit<T> (T value, string paraName, T min,T max)
            where T : System.IComparable
        {
            CheckForNullReference(paraName, "paraName");
            if (value.CompareTo(min) < 0)
            {
                throw new ArgumentOutOfRangeException(paraName);
            }
            else if (value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(paraName);
            }
            else
            {
            }
        }
        #endregion 

        #region CheckValueMinLimit
        /// <summary>
        /// 检测一个值是否小于最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="paraName"></param>
        /// <param name="min">最小充许值</param>
        public static void CheckValueMinLimit<T>(T value, string paraName, T min)
            where T : System.IComparable
        {
            CheckForNullReference(paraName, "paraName");
            if (value.CompareTo(min) < 0)
            {
                throw new ArgumentOutOfRangeException(paraName);
            }
        }
        #endregion

        #region CheckValueMaxLimit
        /// <summary>
        /// 检测一个值是否大于最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="paraName"></param>
        /// <param name="max">最大值</param>
        public static void CheckValueMaxLimit<T>(T value, string paraName, T max)
            where T : System.IComparable
        {
            CheckForNullReference(paraName, "paraName");
            if (value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(paraName);
            }
        }

        #endregion

        #region CheckRegExpression
        /// <summary>
        /// 测试一个字符串是否合法的正达表达式.
        /// </summary>
        /// <param name="expression">需要测试的表达式</param>
        /// <param name="paramName">参数名称</param>
        public static void CheckRegExpression(string expression,string paramName)
        {
            CheckForEmptyString(expression, "expresion");
            CheckForEmptyString(expression, "paramName");
            try
            {
                Regex exp = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
            catch
            {
                throw new ArgumentException(SR.ExceptionNotValidRegexExpress(expression),paramName);
            }
        }    
        #endregion

        #region CheckForValidatePathName
        /// <summary>
        /// 检查路径是否有效字符串
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="variableName"></param>
        //public static void CheckForValidatePathName(string variable, string variableName)
        //{
        //    CheckForNullReference(variable, variableName);
        //    CheckForNullReference(variableName, "variableName");
        //    if (Path.)
        //    {
        //        throw new ArgumentException(SR.ExceptionInvalidatePathString(variableName));

        //    }
        //}
        #endregion
    }
}