using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ComTypes = System.Runtime.InteropServices.ComTypes;
namespace ExpertLib.IO
{
    #region StgElementType
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum StgElementType  :int
    {
        /// <summary>
        /// Indicates that the storage element is a storage object.
        /// </summary>
        Storage=1, 
      
        /// <summary>
        /// Indicates that the storage element is a stream object.
        /// </summary>
        Stream=2,
        
        /// <summary>
        /// Indicates that the storage element is a byte-array object.
        /// </summary>
        LockBytes=3,

        /// <summary>
        /// Indicates that the storage element is a property storage object.
        /// </summary>
        Property=4
    }
    #endregion

    /// <summary>
    /// 本类是用一个类来包装 System.Runtime.InteropServices.ComTypes中的STATSTG结构，便于使用
    /// </summary>
    public class StgElementInfo
    {
        private ComTypes.STATSTG stg_;

        public StgElementInfo(ComTypes.STATSTG stg)
        {
            stg_ = stg;
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public StgElementType StgType
        {
            get
            {
                return (StgElementType)stg_.type;
            }
        }

        /// <summary>
        /// 节点对象名称
        /// </summary>
        public string Name
        {
            get
            {
                return stg_.pwcsName;
            }
        }

        /// <summary>
        /// 返回存储对象的类标识符
        /// </summary>
        public Guid CLSID
        {
            get
            {
                return stg_.clsid;
            }
        }

        /// <summary>
        /// 指定流的大小
        /// </summary>
        public long Size
        {
            get
            {
                return stg_.cbSize;
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return DateTimeConvert.ToDateTime(stg_.ctime);
            }
        }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastAccessTime
        {
            get
            {
                return DateTimeConvert.ToDateTime(stg_.atime);
            }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModifyTime
        {
            get
            {
                return DateTimeConvert.ToDateTime(stg_.mtime);
            }
        }

        //public void Dispose()
        //{
        //    Marshal.ReleaseComObject(this.stg_);
            
        //}
    }
}
