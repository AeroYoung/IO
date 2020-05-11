//本代码源于 http://blog.csdn.net/jh_zzz
//并于此基础上进行了大量修改

using ExpertLib.Dialogs;

namespace ExpertLib.IO
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices;
    using ComTypes = System.Runtime.InteropServices.ComTypes;
    using System.Drawing;
    using System.Windows.Forms;
    
    #region StorageShareMode
    /// <summary>
    /// 存储的共享模式
    /// </summary>
    public enum StorageShareMode : uint
    {
        /// <summary>
        /// 共享存取操作
        /// </summary>
        ShareDenyNone = 0x40,
        /// <summary>
        /// 拒绝共享的读操作
        /// </summary>
        ShareDenyRead = 0x30,
        /// <summary>
        /// 拒绝共享的写操作
        /// </summary>
        ShareDenyWrite = 0x20,
        /// <summary>
        /// 独占的存取模式
        /// </summary>
        ShareExclusive = 0x10,
    }
    #endregion

    #region StorageCreateMode
    /// <summary>
    /// 存储创建模式
    /// </summary>
    public enum StorageCreateMode : uint
    {
        /// <summary>
        /// 如果已经存在一个流/存储，它将被删除；如果没有已存在的流/存储，就创建一个新的
        /// </summary>
        Create = 0x1000,
        /// <summary>
        /// 保留原来的数据，并将数据保存在CONTENTS流对象中，且该流位于当前存储对象下
        /// </summary>
        Convert = 0x00020000,
        /// <summary>
        /// 如果已经存在了一个留或存储，调用失败
        /// </summary>
        FailIfThere =0x00000000,
        /// <summary>
        /// 当带有这个标识的复合文档中的流或存储的父存储被释放时，它会被自动释放
        /// </summary>
        DeleteOnRelease =0x04000000
    }
    #endregion

    #region StorageTransactedMode
    /// <summary>
    /// 存储事务模式
    /// </summary>
    public enum StorageTransactedMode :uint
    {
        /// <summary>
        /// 所有对复合文档的修改立即生效
        /// </summary>
        Direct =0x00400000,
        /// <summary>
        /// 直到提交被调用修改才被写入到复合文档，类似于数据库操作中的提交和回滚
        /// </summary>
        Transacted =0x00010000
    }
    #endregion

    #region StorageReadWriteMode
    /// <summary>
    /// 存储读写方式
    /// </summary>
    public enum StorageReadWriteMode :uint
    {
        /// <summary>
        /// 只读模式
        /// </summary>
        Read = 0x0,
        /// <summary>
        /// 只写模式
        /// </summary>
        Write = 0x1,
        /// <summary>
        /// 读写模式
        /// </summary>
        ReadWrite = 0x2
    }
    #endregion

    #region StgcMode
    /// <summary>
    /// 提交方式
    /// </summary>
    public enum StgcMode : uint
    {
        /// <summary>
        /// 默认的
        /// </summary>
        Default = 0,
        /// <summary>
        /// 覆盖
        /// </summary>
        OverWrite = 1,
        /// <summary>
        /// 当前提交
        /// </summary>
        OnlyIfCurrent = 2,
        /// <summary>
        /// 提交到Cache中
        /// </summary>
        DangerouslyCommitmerelyToDiskCache = 4
    }
    #endregion

    #region StorageFile
    /// <summary>
    /// 结构化存储文件类
    /// </summary>
    public static class StorageFile 
    {
      
        #region IsStorageFile
        /// <summary>
        /// 测试一个文件是否结构化文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsStorageFile(string fileName)
        {
            ArgumentValidation.CheckForEmptyString(fileName, "fileName");
            
            //如果文件不存在
            if(!File.Exists(fileName))
            {
                throw new Exception(SR.ExceptionFileNotExist(fileName));
            }

            return StorageNativeMethods.IsStorageFile(fileName);
        }
        #endregion

        #region CreateTempStorageFile
        /// <summary>
        /// 创建一个临时的结构化存储文档
        /// </summary>
        /// <returns></returns>
        public static Storage CreateTempStorageFile()
        {
            uint mode = (uint)StorageCreateMode.Create + (uint)StorageCreateMode.DeleteOnRelease
                + (uint)StorageReadWriteMode.ReadWrite + (uint)StorageShareMode.ShareExclusive;
            try
            {
                IStorage storage = StorageNativeMethods.StgCreateDocfile(null, mode, 0);
                return new Storage(storage);
            }
            catch (COMException)
            {
                return null;
            }
        }
        #endregion

        #region CreateStorageFile
        /// <summary>
        /// 总是创建一个结构化文档不管原来有没有存在,并返回根存储（可读写、不启用事务、拒绝共享写方式）
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns>根存储</returns>
        public static Storage CreateStorageFile(string storageFile)
        {
            
            return CreateStorageFile(storageFile, StorageCreateMode.Create, StorageReadWriteMode.ReadWrite,
                StorageShareMode.ShareExclusive, StorageTransactedMode.Direct);
        }

        public static Storage CreateStorageFile(string storageFile,
                          StorageCreateMode createMode,
                          StorageReadWriteMode readwriteMode,
                          StorageShareMode shareMode,
                          StorageTransactedMode transactedMode)
        {
            ArgumentValidation.CheckForEmptyString(storageFile, "storageFile");

            uint mode = (uint)createMode + (uint)readwriteMode + (uint)shareMode + (uint)transactedMode;
            try
            {
                IStorage storage = StorageNativeMethods.StgCreateDocfile(storageFile, mode, 0);
                return new Storage(storage);
            }
            catch (COMException)
            {
                return null;
            }
        }
        #endregion

        #region OpenStorageFile
        /// <summary>
        /// 打开一个复合文档，并返回根目录的Storage对象
        /// </summary>
        /// <param name="storageFile"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Storage OpenStorageFile(string storageFile)
        {
            return OpenStorageFile(storageFile, StorageReadWriteMode.ReadWrite,
                StorageShareMode.ShareExclusive, StorageTransactedMode.Direct);
        }

        public static Storage ReadStorageFile(string storageFile)
        {
            return OpenStorageFile(storageFile, StorageReadWriteMode.Read,
                StorageShareMode.ShareExclusive, StorageTransactedMode.Direct);
        }

        public static Storage OpenStorageFile(string storageFile,
                          StorageReadWriteMode readwriteMode,
                          StorageShareMode shareMode,
                          StorageTransactedMode transactedMode)
        {
            if(!IsStorageFile(storageFile))
            {
                return null;
            }

            uint mode = (uint)readwriteMode + (uint)shareMode + (uint)transactedMode;
            try
            {
                IStorage storage = StorageNativeMethods.StgOpenStorage(storageFile, IntPtr.Zero, (uint)mode, IntPtr.Zero, 0);
                return new Storage(storage);
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
        #endregion

        #region CleanCopyStorageFile
        /// <summary>
        /// 复制一个存储文件到另一个存储文件，并把无效的空间去除
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="dectFileName"></param>
        public static void CleanCopyStorageFile(string sourceFileName, string dectFileName)
        {
            ArgumentValidation.CheckForEmptyString(sourceFileName, "sourceFileName");
            ArgumentValidation.CheckForEmptyString(dectFileName, "dectFileName");

            Storage source = OpenStorageFile(sourceFileName);
            Storage dect = CreateStorageFile(dectFileName);
            source.CopyTo(dect);
            dect.Commit();
            source.Dispose();
            dect.Dispose();
        }

        public static void CleanCopyStorageFile(this Storage source, string dectFileName)
        {
            ArgumentValidation.CheckForEmptyString(source.Name, "sourceFileName");
            ArgumentValidation.CheckForEmptyString(dectFileName, "dectFileName");

            //Storage source = OpenStorageFile(sourceFileName);
            Storage dect = CreateStorageFile(dectFileName);
            source.CopyTo(dect);
            dect.Commit();
            source.Dispose();
            dect.Dispose();
        }
        #endregion
    }
    #endregion

    #region Storage
    /// <summary>
    /// 存储类
    /// </summary>
    public class Storage : System.IDisposable
    {
        #region Instance Field
        private bool disposed;
        private IStorage storage;
        #endregion

        #region Constructor
        internal Storage(IStorage storage)
        {
            this.storage = storage;
        }

        ~Storage()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                if (!this.disposed)
                {
                    Marshal.ReleaseComObject(this.storage);
                    //this.storage.Release();
                    this.storage = null;                   
                    this.disposed = true;
                }

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region CreateStorage
        /// <summary>
        /// 创建一个存储 用直接模式打开
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Storage CreateStorage(string name)
        {
            return CreateStorage(name, StorageCreateMode.Create, StorageReadWriteMode.ReadWrite, StorageTransactedMode.Direct);
        }

        public Storage CreateStorage(string name, StorageCreateMode createMode,
                 StorageReadWriteMode readwriteMode,
                 StorageTransactedMode transactedMode)
        {
            StorageHelper.ValidateStorageName(name);

            IStorage subStorage = null;

            try
            {
                //子点节，总是建议使用独占模式
                uint mode = (uint)readwriteMode + (uint)StorageShareMode.ShareExclusive
                         + (uint)transactedMode + (uint)createMode;
                this.storage.CreateStorage(name,mode,0, 0, out subStorage);
                this.storage.Commit(0);

                return new Storage(subStorage);
            }
            catch (COMException ex)
            {
                if (subStorage != null)
                    Marshal.ReleaseComObject(subStorage);
                Log.e($"CreateStorage {ex.Message}");
            }

            return null;
        }
        #endregion

        #region OpenStorage
        /// <summary>
        /// 打开一个子存储
        /// </summary>
        /// <param name="name">存储名称</param>
        /// <returns></returns>
        public Storage OpenStorage(string name)
        {
            return OpenStorage(name, StorageReadWriteMode.ReadWrite, StorageTransactedMode.Direct);
        }

        public Storage ReadStorage(string name)
        {
            return OpenStorage(name, StorageReadWriteMode.Read, StorageTransactedMode.Direct);
        }

        public Storage OpenStorage(string name,
                 StorageReadWriteMode readwriteMode,
                 StorageTransactedMode transactedMode)
        {
            StorageHelper.ValidateStorageName(name);

            IStorage subStorage = null;

            try
            {
                //子点节，总是建议使用独占模式
                uint mode = (uint)readwriteMode + (uint)StorageShareMode.ShareExclusive
                         + (uint)transactedMode;
                this.storage.OpenStorage(name, null, mode, IntPtr.Zero, 0, out subStorage);
                this.storage.Commit(0);

                return new Storage(subStorage);
            }
            catch (COMException)
            {
                if (subStorage != null)
                    Marshal.ReleaseComObject(subStorage);
            }

            return null;
        }
        #endregion

        #region CopyTo
        /// <summary>
        /// 将这个存储复制到另一个存储，可以实现空间清理
        /// </summary>
        /// <param name="destinationStorage"></param>
        public void CopyTo(Storage destinationStorage)
        {
            this.storage.CopyTo(0, IntPtr.Zero, IntPtr.Zero, destinationStorage.storage);
        }
        #endregion

        #region RecurOpenStorage
        /// <summary>
        /// 用路径的方式定位一个存储
        /// </summary>
        /// <param name="name">类于于路径的字符串如ABC\DEF\JJJ</param>
        /// <returns></returns>
        public Storage RecurOpenStorage(string name)
        {
            string pwcsName;

            int pos = name.IndexOf('\\');
            if (pos > 0)
            {
                pwcsName = name.Substring(0, pos);
                name = name.Substring(pos + 1);
            }
            else
            {
                pwcsName = name;
                name = "";
            }

            Storage subStorage = OpenStorage(pwcsName);
            if (subStorage != null && name.Length > 0)
            {
                return subStorage.RecurOpenStorage(name);
            }

            return subStorage;
        }
        #endregion

        #region CreateStream
        /// <summary>
        /// 创建一个流
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageStream CreateStream(string name)
        {
            return CreateStream(name, StorageCreateMode.Create, StorageReadWriteMode.ReadWrite, StorageTransactedMode.Direct);
        }

        public StorageStream CreateStream(string name, StorageCreateMode createMode,
                 StorageReadWriteMode readwriteMode,
                 StorageTransactedMode transactedMode)
        {
            StorageHelper.ValidateStorageName(name);

            IStream subStream = null;

            try
            {
                //总是建议使用独占模式
                uint mode = (uint)readwriteMode + (uint)StorageShareMode.ShareExclusive
                         + (uint)transactedMode + (uint)createMode;
                this.storage.CreateStream(name,mode,0, 0, out subStream);
                this.storage.Commit(0);

                return new StorageStream(subStream);
            }
            catch (COMException)
            {
                if (subStream != null)
                    Marshal.ReleaseComObject(subStream);

                return null;
            }
        }
        #endregion

        #region OpenStream
        /// <summary>
        /// 打开一个流
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageStream OpenStream(string name)
        {
            return OpenStream(name, StorageReadWriteMode.ReadWrite, StorageTransactedMode.Direct);
        }

        public StorageStream ReadStream(string name)
        {
            return OpenStream(name, StorageReadWriteMode.Read, StorageTransactedMode.Direct);
        }

        public StorageStream OpenStream(string name, 
                 StorageReadWriteMode readwriteMode,
                 StorageTransactedMode transactedMode)
        {
            StorageHelper.ValidateStorageName(name);

            IStream subStream = null;

            try
            {
                //总是建议使用独占模式
                uint mode = (uint)readwriteMode + (uint)StorageShareMode.ShareExclusive
                         + (uint)transactedMode;
                this.storage.OpenStream(name, IntPtr.Zero,mode,0, out subStream);
                return new StorageStream(subStream);
            }
            catch (COMException)
            {
                if (subStream != null)
                    Marshal.ReleaseComObject(subStream);

                return null;
            }
        }
        #endregion

        #region Commit
        /// <summary>
        /// 提交,包括子对象
        /// </summary>
        public void Commit()
        {
           this.storage.Commit((uint)StgcMode.Default);
        }
        #endregion

        #region Revert
        /// <summary>
        /// 取消自打开以来或上次提交以来的所有修改
        /// </summary>
        /// <remarks>
        /// 仅在事务模式下有效，注意此函数调用后，所有的子存储和流对象不再有效。
        /// </remarks>
        public void Revert()
        {
            this.storage.Revert();
        }
        #endregion

        #region 取子元素信息
        
        public List<StgElementInfo> GetChildElementsInfo()
        {
            IEnumSTATSTG statstg;
            ComTypes.STATSTG stat;
            uint k;
            List<StgElementInfo> list = new List<StgElementInfo>();
            storage.EnumElements(0, IntPtr.Zero, 0,out statstg);//此处没有释放
            statstg.Reset();
            while (statstg.Next(1, out stat, out k) == HRESULT.S_OK)
            {
                list.Add(new StgElementInfo(stat));
            }
            Marshal.ReleaseComObject(statstg);//释放
            statstg = null;
            return list;
        }
        #endregion

        #region MoveElement
        /// <summary>
        /// 将本存储对象下指定的存储或流对象移动到指定的流或存储里
        /// </summary>
        /// <param name="elementName">存储或流名</param>
        /// <param name="destStorage">目标存储</param>
        public void MoveElement(string elementName, Storage destStorage)
        {
            ArgumentValidation.CheckForEmptyString(elementName,"elementName");
            if (IsElementExist(elementName))
            {
                try
                {
                    this.storage.MoveElementTo(elementName, destStorage.storage, elementName, StorageConst.STGMOVE_MOVE);
                }
                catch (COMException ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception(SR.ExceptionElementNotExist);
            }

        }
        #endregion

        #region CopyElement
        /// <summary>
        /// 将本存储对象下指定的存储或流对象复制到指定的流或存储里
        /// </summary>
        /// <param name="elementName">存储或流名</param>
        /// <param name="destStorage">目标存储</param>
        public void CopyElement(string elementName, Storage destStorage)
        {
            ArgumentValidation.CheckForEmptyString(elementName, "elementName");
            if (IsElementExist(elementName))
            {
                try
                {
                    this.storage.MoveElementTo(elementName, destStorage.storage, elementName, StorageConst.STGMOVE_COPY);
                }
                catch (COMException ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception(SR.ExceptionElementNotExist);
            }

        }
        #endregion

        #region DeleteElement
        /// <summary>
        /// 删除指定的子元素
        /// </summary>
        /// <param name="elementName"></param>
        public void DeleteElement(string elementName)
        {
            if (!IsElementExist(elementName))
                throw new Exception(SR.ExceptionElementNotExist);

            this.storage.DestroyElement(elementName);
        }
        #endregion

        #region RenameElement
        /// <summary>
        /// 重命名子元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="newName"></param>
        public void RenameElement(string elementName, string newName)
        {
            StorageHelper.ValidateStorageName(newName);

            if (!IsElementExist(elementName))
                throw new Exception(SR.ExceptionElementNotExist);

            if (IsElementExist(newName))
                throw new Exception(SR.ExceptionElementNameExist);
            

            this.storage.RenameElement(elementName, newName);
        }
        #endregion

        #region IsElementExist
        /// <summary>
        /// 检查是否存有指定名称的子对象
        /// </summary>
        /// <param name="elementName">对象名称</param>
        /// <returns></returns>
        /// <remarks>忽略大小写比较名称</remarks>
        public bool IsElementExist(string elementName)
        {
            ArgumentValidation.CheckForEmptyString(elementName, "elementName");

            IEnumSTATSTG statstg;
            ComTypes.STATSTG stat;
            uint k;
            this.storage.EnumElements(0, IntPtr.Zero, 0, out statstg);
            statstg.Reset();
            while (statstg.Next(1, out stat, out k) == HRESULT.S_OK)
            {
                //忽略大小写比较
                if (string.Compare(stat.pwcsName, elementName, true) == 0) return true;
            }
            return false;
        }
        #endregion

        #region CLSID
        /// <summary>
        /// 读取或设置存储对应的CLSID值
        /// </summary>
        public Guid CLSID
        {
            get
            {
                ComTypes.STATSTG statstg;
                this.storage.Stat(out statstg, StorageConst.STATFLAG_NONAME);
                return statstg.clsid;
            }
            set
            {
                this.storage.SetClass(value);
            }
        }
        #endregion

        #region Name
        /// <summary>
        /// 读取存储的名字
        /// </summary>
        public string Name
        {
            get
            {
                ComTypes.STATSTG statstg;
                this.storage.Stat(out statstg, StorageConst.STATFLAG_DEFAULT);
                return statstg.pwcsName;

            }
        }
        #endregion

        #region StateBits
        /// <summary>
        /// 读写状态码
        /// </summary>
        public uint StateBits
        {
            get
            {
                ComTypes.STATSTG statstg;
                this.storage.Stat(out statstg, StorageConst.STATFLAG_NONAME);
                
                return ValueConvert.ToUInt32(statstg.grfStateBits);
            }
            set
            {
                this.storage.SetStateBits(value, 0x0);
            }
        }
        #endregion

        #region StorageInfo
        /// <summary>
        /// 读取存储属性
        /// </summary>
        public StgElementInfo StorageInfo
        {
            get
            {
                ComTypes.STATSTG statstg;
                this.storage.Stat(out statstg, StorageConst.STATFLAG_DEFAULT);
                return new StgElementInfo(statstg);
            }
        }
        #endregion
    }
    #endregion
}
