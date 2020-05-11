namespace ExpertLib.IO
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices;
    using ComTypes = System.Runtime.InteropServices.ComTypes;
    using ExpertLib;

    #region Const
    /// <summary>
    /// 一些库内部使用的常量
    /// </summary>
    /// <remarks>源定义于WTypes.h</remarks>
    internal static class StorageConst
    {
        //STGMOVE
        public const uint STGMOVE_MOVE	= 0;
        public const uint STGMOVE_COPY	= 1;
        public const uint STGMOVE_SHALLOWCOPY	= 2;

        //STATFLAG
        public const uint STATFLAG_DEFAULT	= 0;
	    public const uint STATFLAG_NONAME	= 1;
	    public const uint STATFLAG_NOOPEN	= 2;
    }
    #endregion
    
    #region StorageNativeMethods
    /// <summary>
    /// 结构化存储相关函数包装类
    /// </summary>
    internal sealed class StorageNativeMethods
    {
        [DllImport("ole32.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern IStorage StgCreateDocfile([MarshalAs(UnmanagedType.LPWStr)] string pwcsName, uint grfMode, uint reserved);

        [DllImport("ole32.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern IStorage StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string pwcsName, IntPtr pstgPriority, uint grfMode, IntPtr snbExclude, uint reserved);

        // HRESULT StgIsStorageFile(const WCHAR * pwcsName ); 原型函数
        // 因为无输出参数，因此PreserveSig设为true
        [DllImport("ole32.dll", PreserveSig = true)]
        private static extern int StgIsStorageFile([MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

        #region IsStorageFile
        /// <summary>
        /// 测试一个文件是否结构化文档
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static bool IsStorageFile(string fileName)
        {
            ArgumentValidation.CheckForEmptyString(fileName,"fileName");
            if (!File.Exists(fileName))
            {
                throw new ArgumentException(SR.ExceptionFileNotExist(fileName),"fileName");
            }

            int ret = StgIsStorageFile(fileName);
            if (ret == HRESULT.S_OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
    #endregion

    #region IEnumSTATSTG
    /// <summary>
    /// IEnumSTATSTG 接口
    /// </summary>
    [ComImport, Guid("0000000d-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumSTATSTG
    {
        //[PreserveSig]
        //int Next(uint celt, [MarshalAs(UnmanagedType.LPArray), Out] ComTypes.STATSTG[] rgelt, out uint pceltFetched);

        [PreserveSig]
        int Next(uint celt, out ComTypes.STATSTG rgelt, out uint pceltFetched);
        
        void Skip(uint celt);

        void Reset();

        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumSTATSTG Clone();
    }
    #endregion

    #region IStorage
    /// <summary>
    /// IStorage 接口
    /// </summary>
    [ComImport, Guid("0000000b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IStorage
    {
        //void Release();

        void CreateStream(string pwcsName, uint grfMode, uint reserved1, uint reserved2, out IStream ppstm);

        void OpenStream(string pwcsName, IntPtr reserved1, uint grfMode, uint reserved2, out IStream ppstm);

        void CreateStorage(string pwcsName, uint grfMode, uint reserved1, uint reserved2, out IStorage ppstg);

        void OpenStorage(string pwcsName, IStorage pstgPriority, uint grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstg);

        void CopyTo(uint ciidExclude, IntPtr rgiidExclude, IntPtr snbExclude, IStorage pstgDest);

        void MoveElementTo(string pwcsName, IStorage pstgDest, string pwcsNewName, uint grfFlags);

        void Commit(uint grfCommitFlags);

        void Revert();

        void EnumElements(uint reserved1, IntPtr reserved2, uint reserved3, out IEnumSTATSTG ppenum);

        void DestroyElement(string pwcsName);

        void RenameElement(string pwcsOldName, string pwcsNewName);

        void SetElementTimes(string pwcsName, ComTypes.FILETIME pctime, ComTypes.FILETIME patime, ComTypes.FILETIME pmtime);

        void SetClass(Guid clsid);

        void SetStateBits(uint grfStateBits, uint grfMask);

        void Stat(out ComTypes.STATSTG pstatstg, uint grfStatFlag);
    }
    #endregion

    #region IStream
    /// <summary>
    /// IStream 接口 
    /// </summary>
    /// <remarks>
    /// 实际上Microsoft.CLRAdmin空间里定义了同样的接口
    /// </remarks>
    [ComImport, Guid("0000000c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IStream
    {
        void Read([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbRead);

        void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbWritten);

        void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);

        void SetSize(long libNewSize);

        void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);

        void Commit(int grfCommitFlags);

        void Revert();

        void LockRegion(long libOffset, long cb, int dwLockType);

        void UnlockRegion(long libOffset, long cb, int dwLockType);

        void Stat(out ComTypes.STATSTG pstatstg, int grfStatFlag);

        void Clone(out IStream ppstm);
    }
    #endregion


}
