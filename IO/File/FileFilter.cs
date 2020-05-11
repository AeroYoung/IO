namespace ExpertLib.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// 文件匹配类，匹配全名
    /// </summary>
    public class FileFilter : IScanFilter
    {
        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="PathFilter"></see>.
        /// </summary>
        /// <param name="filter">The <see cref="NameFilter"></see>filter expression to apply.</param>
        public FileFilter(string filter)
        {
            nameFilter_ = new StringFilter(filter);
        }
        #endregion

        #region IsMatch
        /// <summary>
        /// Test a name to see if it matches the filter.
        /// </summary>
        /// <param name="name">The name to test.</param>
        /// <returns>True if the name matches, false otherwise.</returns>
        public virtual bool IsMatch(string name)
        {
            return nameFilter_.IsMatch(Path.GetFullPath(name));
        }
        #endregion

        #region Instance Fields
        StringFilter nameFilter_;
        #endregion
    }

    /// <summary>
    /// 文件属性匹配(包括名字、大小、创建日期)
    /// </summary>
    /// <remarks>Provides an example of how to customise filtering.</remarks>
    public class ExtendedFileFilter : FileFilter
    {
        #region Constructors
        /// <summary>
        /// Initialise a new instance of ExtendedPathFilter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minSize">The minimum file size to include.</param>
        /// <param name="maxSize">The maximum file size to include.</param>
        public ExtendedFileFilter(string filter,
            long minSize, long maxSize)
            : base(filter)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        /// <summary>
        /// Initialise a new instance of ExtendedPathFilter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minDate">The minimum <see cref="DateTime"/> to include.</param>
        /// <param name="maxDate">The maximum <see cref="DateTime"/> to include.</param>
        public ExtendedFileFilter(string filter,
            DateTime minDate, DateTime maxDate)
            : base(filter)
        {
            MinDate = minDate;
            MaxDate = maxDate;
        }

        /// <summary>
        /// Initialise a new instance of ExtendedPathFilter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minSize">The minimum file size to include.</param>
        /// <param name="maxSize">The maximum file size to include.</param>
        /// <param name="minDate">The minimum <see cref="DateTime"/> to include.</param>
        /// <param name="maxDate">The maximum <see cref="DateTime"/> to include.</param>
        public ExtendedFileFilter(string filter,
            long minSize, long maxSize,
            DateTime minDate, DateTime maxDate)
            : base(filter)
        {
            MinSize = minSize;
            MaxSize = maxSize;
            MinDate = minDate;
            MaxDate = maxDate;
        }
        #endregion

        #region IsMatch
        /// <summary>
        /// 测试文件是否匹配
        /// </summary>
        /// <param name="name">The filename to test.</param>
        /// <returns>True if the filter matches, false otherwise.</returns>
        public override bool IsMatch(string name)
        {
            bool result = base.IsMatch(name);

            if (result)
            {
                FileInfo fileInfo = new FileInfo(name);
                result =
                    (MinSize <= fileInfo.Length) &&
                    (MaxSize >= fileInfo.Length) &&
                    (MinDate <= fileInfo.LastWriteTime) &&
                    (MaxDate >= fileInfo.LastWriteTime)
                    ;
            }
            return result;
        }
        #endregion

        #region MinSize
        /// <summary>
        /// Get/set the minimum size for a file that will match this filter.
        /// </summary>
        public long MinSize
        {
            get { return minSize_; }
            set
            {
                if ((value < 0) || (maxSize_ < value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                minSize_ = value;
            }
        }
        #endregion

        #region MaxSize
        /// <summary>
        /// Get/set the maximum size for a file that will match this filter.
        /// </summary>
        public long MaxSize
        {
            get { return maxSize_; }
            set
            {
                if ((value < 0) || (minSize_ > value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                maxSize_ = value;
            }
        }
        #endregion

        #region MinDate
        /// <summary>
        /// Get/set the minimum <see cref="DateTime"/> value that will match for this filter.
        /// </summary>
        /// <remarks>Files with a LastWrite time less than this value are excluded by the filter.</remarks>
        public DateTime MinDate
        {
            get
            {
                return minDate_;
            }

            set
            {
                if (value > maxDate_)
                {
                    throw new ArgumentException("Exceeds MaxDate", "value");
                }

                minDate_ = value;
            }
        }
        #endregion

        #region MaxDate
        /// <summary>
        /// Get/set the maximum <see cref="DateTime"/> value that will match for this filter.
        /// </summary>
        /// <remarks>Files with a LastWrite time greater than this value are excluded by the filter.</remarks>
        public DateTime MaxDate
        {
            get
            {
                return maxDate_;
            }

            set
            {
                if (minDate_ > value)
                {
                    throw new ArgumentException("Exceeds MinDate", "value");
                }

                maxDate_ = value;
            }
        }
        #endregion

        #region Instance Fields
        long minSize_;
        long maxSize_ = long.MaxValue;
        DateTime minDate_ = DateTime.MinValue;
        DateTime maxDate_ = DateTime.MaxValue;
        #endregion
    }
 
}
