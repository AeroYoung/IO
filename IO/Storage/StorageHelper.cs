
namespace ExpertLib.IO
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ExpertLib;

    
    internal class StorageHelper
    {
        private static string[] UnallowedChar = new string[] { "!", ":", "/", "\\" };

        #region ValidateStorageName
        /// <summary>
        /// 验证Storage或Stream的命名是否正确
        /// </summary>
        /// <param name="name"></param>
        /// <remarks>
        /// 复合文档的流及存储的命名规则
        ///（1）名称不能超过31字符的长度。
        ///（2）名称中不能包含！、：、/、\、这些字符。
        ///（3）不能使用任何Ord(char)小于32的字符作为首字符
        /// (4) . 和..名称被保留
        /// </remarks>
        public static void ValidateStorageName(string name)
        {
            ArgumentValidation.CheckForEmptyString(name, "name");
            if (name.Length > 31)
                throw new ArgumentException(SR.ExceptionInvalidStorageName, "name");
            if(name[0]<0x32)
                throw new ArgumentException(SR.ExceptionInvalidStorageName, "name");
            if( (name == "." )|| (name =="..")) //不能使用保留名称
                throw new ArgumentException(SR.ExceptionInvalidStorageName, "name");

            foreach (string s in UnallowedChar)
            {
                if(name.Contains(s))
                    throw new ArgumentException(SR.ExceptionInvalidStorageName, "name");
            }
        }
        #endregion

    }
}
