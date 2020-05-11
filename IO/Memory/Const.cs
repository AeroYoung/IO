//本文包括一些系统常量

namespace ExpertLib
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 一些用于HRESULT类型的常用值
    /// </summary>
    public static class HRESULT
    {
        /// <summary>
        /// 表示成功
        /// </summary>
        public const int S_OK = 0;
        /// <summary>
        /// 表示失败
        /// </summary>
        public const int S_FALSE = 1;
    }
}
