using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ExpertLib.IO
{
    #region EventArgs
    #region ScanEventArgs
    /// <summary>
	/// 扫描事件参数.
	/// </summary>
	public class ScanEventArgs : EventArgs
	{
        #region Instance Fields
        string name_;
        bool continueRunning_ = true;
        #endregion

		/// <summary>
		/// 构造一个扫描参数
		/// </summary>
		/// <param name="name"></param>
		public ScanEventArgs(string name)
		{
			this.name_ = name;
            this.continueRunning_ = true;
		}
		
		
		/// <summary>
		/// 正在扫描的文件.
		/// </summary>
		public string FileName
		{
			get { return name_; }
            internal set
            {
                this.name_ = value;
            }
		}
		
		
		/// <summary>
		/// 确认是否继续扫描,默认为true.
		/// </summary>
		public bool ContinueRunning
		{
			get { return continueRunning_; }
			set { continueRunning_ = value; }
		}
    }
    #endregion

    #region ScanFailureEventArgs
    /// <summary>
	/// 扫描失败参数.
	/// </summary>
	public class ScanFailureEventArgs
	{
		/// <summary>
		/// Initialise a new instance of <see cref="ScanFailureEventArgs"></see>
		/// </summary>
		/// <param name="name">The name to apply.</param>
		/// <param name="e">The exception to use.</param>
		public ScanFailureEventArgs(string name, Exception e)
		{
			name_ = name;
			exception_ = e;
		}
		
		
		/// <summary>
		/// 引发错误的文件名.
		/// </summary>
		public string FileName
		{
			get { return name_; }
		}
		
		/// <summary>
		/// 错误的原因.
		/// </summary>
		public Exception Exception
		{
			get { return exception_; }
		}
		
		#region Instance Fields
		string name_;
		Exception exception_;
		#endregion
    }
    #endregion

    #endregion

    #region Delegates
    /// <summary>
	/// Delegate invoked when a directory is processed.
	/// </summary>
	public delegate void ScanDirectoryDelegate(object sender, ScanEventArgs e);
	
	/// <summary>
	/// Delegate invoked when a file is processed.
	/// </summary>
	public delegate void ScanFileDelegate(object sender, ScanEventArgs e);
	
	/// <summary>
	/// Delegate invoked when a directory failure is detected.
	/// </summary>
	public delegate void ScanFailureDelegate(object sender, ScanFailureEventArgs e);
	#endregion

	/// <summary>
	/// 通用文件与目录扫描类
	/// </summary>
    public sealed class FileSystemScanner
    {
        #region Instance Fields
        IScanFilter fileFilter_;
        IScanFilter directoryFilter_;
        long matchFiles;
        long matchDirectorys;
        ScanEventArgs scanArgs;
        bool alive; //是否继续
        #endregion

        #region Constructors
        /// <summary>
        /// 构造一个无任何过滤条件的扫
        /// </summary>
        public FileSystemScanner()
        {
            this.fileFilter_ = null;
            this.directoryFilter_ = null;
            this.matchFiles = 0;
            this.matchDirectorys = 0;
        }

        /// <summary>
        /// 构造扫描器
        /// </summary>
        /// <param name="filter">文件名称过滤器表达式</param>
        public FileSystemScanner(string filter)
        {
            if ((filter == null) || (filter.Trim().Length == 0))
            {
                fileFilter_ = null;
            }
            else
            {
                fileFilter_ = new FileFilter(filter);
            }
            this.directoryFilter_ = null;
            this.matchFiles = 0;
            this.matchDirectorys = 0;
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The <see cref="NameFilter">file filter</see> to apply.</param>
        /// <param name="directoryFilter">The <see cref="NameFilter">directory filter</see> to apply.</param>
        public FileSystemScanner(string fileFilter, string directoryFilter)
        {
            if ((fileFilter == null) || (fileFilter.Trim().Length == 0))
            {
                fileFilter_ = null;
            }
            else
            {
                fileFilter_ = new FileFilter(fileFilter);
            }

            if ((directoryFilter == null) || (directoryFilter.Trim().Length == 0))
            {
                directoryFilter_ = null;
            }
            else
            {
                directoryFilter_ = new FileFilter(directoryFilter);
        
            }
            this.matchFiles = 0;
            this.matchDirectorys = 0;
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The file <see cref="IScanFilter"></see>filter to apply.</param>
        public FileSystemScanner(IScanFilter fileFilter)
        {
            fileFilter_ = fileFilter;
            directoryFilter_ = null;
            this.matchFiles = 0;
            this.matchDirectorys = 0;
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The file <see cref="IScanFilter"></see>filter to apply.</param>
        /// <param name="directoryFilter">The directory <see cref="IScanFilter"></see>filter to apply.</param>
        public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
        {
            fileFilter_ = fileFilter;
            directoryFilter_ = directoryFilter;
            this.matchFiles = 0;
            this.matchDirectorys = 0;
        }
        #endregion

        #region Event
        /// <summary>
        /// 处理目录
        /// </summary>
        public event ScanDirectoryDelegate ProcessDirectoryEvent;

        /// <summary>
        /// 处理文件.
        /// </summary>
        public event ScanFileDelegate ProcessFileEvent;

        /// <summary>
        /// 没有对应目录时发生.
        /// </summary>
        public event ScanFailureDelegate DirectoryFailureEvent;

        #endregion

        #region Scan
        /// <summary>
        /// 搜索一个驱动器
        /// </summary>
        /// <param name="driveinfo"></param>
        /// <remarks>建议一般情况下不要进行递归查询</remarks>
        /// <exception cref="Exception"></exception>
        public void Scan(DriveInfo driveinfo,bool recurse)
        {
            if (!driveinfo.IsReady)
            {
                throw new Exception(SR.ExceptionDriveIsnReady(driveinfo.Name));
            }
            string root = driveinfo.Name;
            this.alive = true;
            scanArgs = new ScanEventArgs(root);
            ScanDir(root, recurse);
        }
        #endregion

        #region Scan
        /// <summary>
        /// 扫描一个目录下的文件和目录.
        /// </summary>
        /// <param name="directory">要扫描的目录</param>
        /// <param name="recurse">是否递归扫描子目录</param>
        /// <remarks>不包括目录本身
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        public void Scan(string directory, bool recurse)
        {
            this.alive = true;
            scanArgs = new ScanEventArgs(directory);
            ScanDir(directory, recurse);
        }
        #endregion
      
        #region MatchFiles
        /// <summary>
        /// 读取匹配的文件数
        /// </summary>
        public long MatchFiles
        {
            get
            {
                return this.matchFiles;
            }
        }
        #endregion

        #region MatchDirectorys
        /// <summary>
        /// 读取匹配的目录数
        /// </summary>
        public long MatchDirectorys
        {
            get
            {
                return this.matchDirectorys;
            }
        }
        #endregion

        #region FileIsMatch
        /// <summary>
        /// 检查文件是否匹配
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool FileIsMatch(string filename)
        {
            if (fileFilter_ == null)
                return true;
            else
            {
                return fileFilter_.IsMatch(filename);
            }
        }
         #endregion

        #region DirectoryIsMatch
        /// <summary>
        /// 检查目录是否匹配
        /// </summary>
        /// <param name="directoryname"></param>
        /// <returns></returns>
        public bool DirectoryIsMatch(string directoryname)
        {
            if (directoryFilter_ == null)
                return true;
            else
                return directoryFilter_.IsMatch(directoryname);
        }
        #endregion

        #region Private function
        
        #region OnScanFailure
        /// <summary>
        /// 产生DirectoryFailure事件.
        /// </summary>
        private void OnScanFailure(ScanFailureEventArgs e)
        {
            this.alive = false;

            if (DirectoryFailureEvent != null)
            {
                DirectoryFailureEvent(this, e);
            }
            else
            {
                  //如果不处理则直接退出,并抛出异常
                throw e.Exception;
            }
        }
        #endregion

        #region OnProcessFile
        /// <summary>
        /// 产生 ProcessFile 事件.
        /// </summary>
        private void OnProcessFile(ScanEventArgs e)
        {
            if (ProcessFileEvent != null)
            {
                ProcessFileEvent(this, e);
                if (e.ContinueRunning == false)
                {
                    this.alive = false;
                }
            }
        }
        #endregion

        #region OnProcessDirectory
        /// <summary>
        /// 产生 ProcessDirectory 事件.
        /// </summary>
        private void OnProcessDirectory(ScanEventArgs e)
        {
            if (ProcessDirectoryEvent != null)
            {
                ProcessDirectoryEvent(this, e);
                if (e.ContinueRunning == false)
                {
                    this.alive = false;
                }
            }
        }
        #endregion

        #region ScanDir
        /// <summary>
        /// 扫描目录核心函数
        /// </summary>
        /// <param name="sourceDirectory">扫描源目录名称</param>
        /// <param name="recurse">是否递归</param>
        void ScanDir(string sourceDirectory, bool recurse)
        {
            if (this.alive == false) return;

            string[] filenames;
            string[] directorynames;
            try
            {
                filenames = System.IO.Directory.GetFiles(sourceDirectory);
                directorynames = System.IO.Directory.GetDirectories(sourceDirectory);
            }
            catch(Exception e)
            {
                ScanFailureEventArgs args = new ScanFailureEventArgs(sourceDirectory, e);
                OnScanFailure(args);
                return;
            }

            //处理目录下的文件
            foreach (string fileName in filenames)
            {
                if (FileIsMatch(fileName))
                {
                    this.matchFiles ++;  //增加文件数
                    scanArgs.FileName = fileName;
                    OnProcessFile(scanArgs);
                    if (this.alive == false) return;
                }
            }
       

            #region 处理子目录
            foreach (string directory in directorynames)
            {
                if (DirectoryIsMatch(directory))
                {
                    this.matchDirectorys++; //增加匹配的目录数
                    this.scanArgs.FileName = directory;
                    OnProcessDirectory(scanArgs);
                    if (this.alive == false) return;

                    if (recurse) //搜索子目录
                    {
                        ScanDir(directory, true);
                    }
                }
            }
            
            #endregion
        }
        #endregion

       
        #endregion
    }
}
