//本类代码收集于网上某篇关于文件操作的文章，部分代码版权归原作者所有。

namespace ExpertLib.IO
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// 文件操作辅助类
    /// </summary>
    /// <remarks>
    /// 本辅助操作类的作用是为了简化一些代码，实际上考虑到程序的性能问题时，可能还是由自已来针对
    /// 特定的File Directory FileInfo DirectoryInfo Path Drive DriveInfo来操作可能更好
    /// </remarks>
    public static class FileIOHelper
    {
        #region FileIsExist
        /// <summary>
        /// 文件是否存在或无权访问
        /// </summary>
        /// <param name="path">相对路径或绝对路径</param>
        /// <returns>如果是目录也返回false</returns>
        public static bool FileIsExist(string path)
        {
            return File.Exists(path);
        }
        #endregion

        #region DirectoryIsExist
        /// <summary>
        /// 目录是否存在或无权访问
        /// </summary>
        /// <param name="Path">相对路径或绝对路径</param>
        /// <returns></returns>
        public static bool DirectoryIsExist(string path)
        {
            return Directory.Exists(path);
        }
        #endregion

        #region FileIsReadOnly
        /// <summary>
        /// 文件是否只读
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static bool FileIsReadOnly(string fullpath)
        {
            FileInfo file = new FileInfo(fullpath);
            if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region SetFileReadonly
        /// <summary>
        /// 设置文件是否只读
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="flag">true表示只读，反之</param>
        public static void SetFileReadonly(string fullpath,bool flag)
        {
            FileInfo file = new FileInfo(fullpath);

            if (flag)
            {
                // 添加只读属性
                file.Attributes |= FileAttributes.ReadOnly;
            }
            else
            {
                // 移除只读属性
                file.Attributes &= ~FileAttributes.ReadOnly;
            }
        }
        #endregion

        #region GetFileSize
        /// <summary>
        /// 取文件长度
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static long GetFileSize(string fullpath)
        {
            FileInfo fi = new FileInfo(fullpath);
            return fi.Length;
        }
        #endregion

        #region GetFileCreateTime
        /// <summary>
        /// 取文件创建时间
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static DateTime GetFileCreateTime(string fullpath)
        {
            FileInfo fi = new FileInfo(fullpath);
            return fi.CreationTime;
        }
        #endregion

        #region GetLastWriteTime
        /// <summary>
        /// 取文件最后存储时间
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static DateTime GetLastWriteTime(string fullpath)
        {
            FileInfo fi = new FileInfo(fullpath);
            return fi.LastWriteTime;
        }
        #endregion

        #region IsPathRooted
        /// <summary>
        /// 指示一个路径是相对路径还是绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }
        #endregion

        #region GetSystemDirectory
        /// <summary>
        /// 取系统目录
        /// </summary>
        /// <returns></returns>
        public static string GetSystemDirectory()
        {
            return System.Environment.SystemDirectory;
        }
        #endregion

        #region GetSpeicalFolder
        /// <summary>
        /// 取系统的特别目录
        /// </summary>
        /// <param name="folderType"></param>
        /// <returns></returns>
        public static string GetSpeicalFolder(Environment.SpecialFolder folderType)
        {
            return System.Environment.GetFolderPath(folderType);
        }
        #endregion

        #region GetTempPath
        /// <summary>
        /// 返回当前系统的临时目录
        /// </summary>
        /// <returns></returns>
        public static string GetTempPath()
        {
            return Path.GetTempPath();
        }
        #endregion

        #region GetInvalidPathChars
        /// <summary>
        /// 取路径中不充许存在的字符
        /// </summary>
        /// <returns></returns>
        public static char[] GetInvalidPathChars()
        {
            return Path.GetInvalidPathChars();
        }
        #endregion

        #region GetCurrentDirectory
        /// <summary>
        /// 取当前目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
        #endregion

        #region SetCurrentDirectory
        /// <summary>
        /// 设当前目录
        /// </summary>
        /// <param name="path"></param>
        public static void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }
        #endregion

        #region CreateTempZeroByteFile
        /// <summary>
        /// 创建一个零字节临时文件
        /// </summary>
        /// <returns></returns>
        public static string CreateTempZeroByteFile()
        {
            return Path.GetTempFileName();
        }
        #endregion

        #region GetRandomFileName
        /// <summary>
        /// 创建一个随机文件名，不创建文件本身
        /// </summary>
        /// <returns></returns>
        public static string GetRandomFileName()
        {
            return Path.GetRandomFileName();
        }
        #endregion

        #region CompareFilesHash
        /// <summary>
        /// 判断两个文件的哈希值是否一致
        /// </summary>
        /// <param name="fileName1"></param>
        /// <param name="fileName2"></param>
        /// <returns></returns>
        public static bool CompareFilesHash(string fileName1, string fileName2)
        {
            using (HashAlgorithm hashAlg = HashAlgorithm.Create())
            {
                using (FileStream fs1 = new FileStream(fileName1, FileMode.Open), fs2 = new FileStream(fileName2, FileMode.Open))
                {
                    byte[] hashBytes1 = hashAlg.ComputeHash(fs1);
                    byte[] hashBytes2 = hashAlg.ComputeHash(fs2);

                    // 比较哈希码
                    return (BitConverter.ToString(hashBytes1) == BitConverter.ToString(hashBytes2));
                }
            }
        }
        #endregion

        #region CalcuDirectorySize
        /// <summary>
        /// 计算一个目录的大小
        /// </summary>
        /// <param name="di">指定目录</param>
        /// <param name="includeSubDir">是否包含子目录</param>
        /// <returns></returns>
        public static long CalcuDirectorySize(DirectoryInfo di, bool includeSubDir)
        {
            long totalSize = 0;
            
            // 检查所有（直接）包含的文件
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }

            // 检查所有子目录，如果includeSubDir参数为true
            if (includeSubDir)
            {
                DirectoryInfo[] dirs = di.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    totalSize += CalcuDirectorySize(dir, includeSubDir);
                }
            }

            return totalSize;
        }
        #endregion

        #region CopyDirectory
        /// <summary>
        /// 复制目录到目标目录
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="destination">目标目录</param>
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            // 如果两个目录相同，则无须复制
            if (destination.FullName.Equals(source.FullName))
            {
                return;
            }

            // 如果目标目录不存在，创建它
            if (!destination.Exists)
            {
                destination.Create();
            }

            // 复制所有文件
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                // 将文件复制到目标目录
                file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
            }

            // 处理子目录
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                string destinationDir = Path.Combine(destination.FullName, dir.Name);

                // 递归处理子目录
                CopyDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }
        #endregion

        #region GetAllDrives
        /// <summary>
        /// 取系统所有的逻辑驱动器
        /// </summary>
        /// <returns></returns>
        public static DriveInfo[] GetAllDrives()
        {
            return DriveInfo.GetDrives();
        }
        #endregion

        #region IsDrivePath
        /// <summary>
        /// 检测一个目录是不是根目录标志
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns></returns>
        public static bool IsDrivePath(string path)
        {
            if (Path.IsPathRooted(path)) //如果为相对路径，肯定不包括根目录
                return false;

            if (Path.GetPathRoot(path) != path)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }
        #endregion
    }
}
