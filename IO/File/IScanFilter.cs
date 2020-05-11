using System;

namespace ExpertLib.IO
{
    /// <summary>
    /// 扫描过滤条件
    /// </summary>
    public interface IScanFilter
    {
        /// <summary>
        /// 是否符合匹配
        /// </summary>
        /// <param name="name">需要匹配的名字</param>
        /// <returns>如果匹配为true反之为false</returns>
        bool IsMatch(string name);
    }
}
