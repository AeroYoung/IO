using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ExpertLib.Utils
{
    #region 任务类

    public enum FtpTaskType { Upload = 1, Download = 2 }

    public class FtpTask
    {
        public FtpTaskType type;

        public string serverName;

        public string localUrl;

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool Overwrite;

        public string Describe
        {
            get
            {
                string value = "下载";
                if (type == FtpTaskType.Upload) value = "上传";
                value += "\n 文件名称(服务器):" + serverName;
                value += "\n 文件路径(本机):" + localUrl;
                return value;
            }
        }
        
        public FtpTask(FtpTaskType type, string localUrl, string serverName,bool Overwrite)
        {
            this.type = type;
            this.serverName = serverName;
            this.localUrl = localUrl;
            this.Overwrite = Overwrite;
        }
    }

    #endregion

    #region 文件信息类

    public class FileStruct
    {
        public string Flags;
        public string Owner;
        public string Group;
        public bool IsDirectory;
        public DateTime CreateTime;
        public string Name;
        public double Size;
        public string Extension;
        public string FullPath;
    }

    public enum FileListStyle
    {
        UnixStyle,
        WindowsStyle,
        Unknown
    }

    #endregion

    public class FtpCtrl
    {
        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region 属性信息
        /// <summary>
                /// FTP请求对象
                /// </summary>
        FtpWebRequest Request = null;
        /// <summary>
        /// FTP响应对象
        /// </summary>
        FtpWebResponse Response = null;
        FtpWebResponse Response2 = null;

        /// <summary>
        /// FTP服务器地址
        /// </summary>
        private Uri _RootUri;

        public string RootUri
        {
            get
            {
                string strUri = _RootUri.ToString();
                if (strUri.EndsWith("/"))
                {
                    strUri = strUri.Substring(0, strUri.Length - 1);
                }
                return strUri;
            }
        }

        /// <summary>
        /// FTP服务器地址+当前工作目录
        /// </summary>
        public Uri WorkUri
        {
            get
            {
                if (_DirectoryPath == "/")
                {
                    return _RootUri;
                }
                else
                {
                    
                    return new Uri(RootUri + this.DirectoryPath);
                }
            }
            set
            {
                if (value.Scheme != Uri.UriSchemeFtp)
                {
                    throw new Exception("Ftp 地址格式错误!");
                }
                _RootUri = new Uri(value.GetLeftPart(UriPartial.Authority));
                _DirectoryPath = value.AbsolutePath;
                if (!_DirectoryPath.EndsWith("/"))
                {
                    _DirectoryPath += "/";
                }
            }
        }
        
        /// <summary>
        /// 当前工作目录
        /// </summary>
        private string _DirectoryPath;

        /// <summary>
        /// 当前工作目录
        /// </summary>
        public string DirectoryPath
        {
            get { return _DirectoryPath; }
            set { _DirectoryPath = value; }
        }

        /// <summary>
        /// FTP登录用户
        /// </summary>
        private string _UserName;
        /// <summary>
        /// FTP登录用户
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        private string _ErrorMsg;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get { return _ErrorMsg; }
            set { _ErrorMsg = value; }
        }

        /// <summary>
        /// FTP登录密码
        /// </summary>
        private string _Password;
        /// <summary>
        /// FTP登录密码
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        /// <summary>
        /// 连接FTP服务器的代理服务
        /// </summary>
        private WebProxy _Proxy = null;
        /// <summary>
        /// 连接FTP服务器的代理服务
        /// </summary>
        public WebProxy Proxy
        {
            get
            {
                return _Proxy;
            }
            set
            {
                _Proxy = value;
            }
        }

        /// <summary>
        /// 是否需要删除临时文件
        /// </summary>
#pragma warning disable 414
        private bool _isDeleteTempFile = false;
#pragma warning restore 414

        /// <summary>
        /// 异步上传所临时生成的文件
        /// </summary>
#pragma warning disable 414
        private string _UploadTempFile = "";
#pragma warning restore 414

        private bool _UsePassive = false;//默认是 true也就是被动模式，主动模式false

        #endregion

        #region 公共参数

        public int PreViewFileSize = 20 * 1024 * 1024;

        public int SmallFileSize = 100 * 1024 * 1024;

        public int LargeFileSize = 2047 * 1024 * 1024;
        
        #endregion

        #region 构造和析构函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="FtpUri">FTP地址</param>
        /// <param name="strUserName">登录用户名</param>
        /// <param name="strPassword">登录密码</param>
        public FtpCtrl(Uri FtpUri, string strUserName, string strPassword)
        {
            this._RootUri = new Uri(FtpUri.GetLeftPart(UriPartial.Authority));
            _DirectoryPath = FtpUri.AbsolutePath;
            if (!_DirectoryPath.EndsWith("/"))
            {
                _DirectoryPath += "/";
            }
            this._UserName = strUserName;
            this._Password = strPassword;
            this._Proxy = null;
        }

        public FtpCtrl()
        {
            string strpath = AppDomain.CurrentDomain.BaseDirectory + "constring.ini";
            string hostName = Dns.GetHostName();

            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString("a", "ftpIP ", "127.0.0.1", temp, 1024, strpath);

            Uri FtpUri = new Uri("ftp://" + temp.ToString().Trim());

            this._RootUri = new Uri(FtpUri.GetLeftPart(UriPartial.Authority));
            _DirectoryPath = FtpUri.AbsolutePath;
            if (!_DirectoryPath.EndsWith("/"))
            {
                _DirectoryPath += "/";
            }
            this._UserName = "";
            this._Password = "";
            this._Proxy = null;
        }
        
        /// <summary>
        /// 析构函数
        /// </summary>
        ~FtpCtrl()
        {
            if (Response != null)
            {
                Response.Close();
                Response = null;
            }
            if (Response2 != null)
            {
                Response2.Close();
                Response2 = null;
            }
            if (Request != null)
            {
                Request.Abort();
                Request = null;
            }
        }

        #endregion

        #region 建立连接与检查

        /// <summary>
        /// 建立FTP链接,返回响应对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="FtpMathod">操作命令</param>
        private FtpWebResponse Open(Uri uri, string FtpMathod)
        {
            try
            {
                Request = (FtpWebRequest)WebRequest.Create(uri);
                Request.Method = FtpMathod;
                Request.UseBinary = true;
                Request.UsePassive = _UsePassive;
                Request.Credentials = new NetworkCredential(this.UserName, this.Password);
                if (Proxy != null)
                {
                    Request.Proxy = this.Proxy;
                }
                FtpWebResponse result = (FtpWebResponse)Request.GetResponse();
                return result;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.ToString();
                throw ex;
            }
            finally
            {
                //Request.Abort();
                //Request = null;
            }
        }
        
        /// <summary>
        /// 建立FTP链接,返回请求对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="FtpMathod">操作命令</param>
        private FtpWebRequest OpenRequest(Uri uri, string FtpMathod)
        {
            try
            {
                Request = (FtpWebRequest)WebRequest.Create(uri);
                Request.Method = FtpMathod;
                Request.UseBinary = true;
                Request.UsePassive = _UsePassive;
                Request.Credentials = new NetworkCredential(this.UserName, this.Password);
                if (this.Proxy != null)
                {
                    Request.Proxy = this.Proxy;
                }
                return Request;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw ep;
            }
            finally
            {
                //Request.Abort();
                //Request = null;
            }
        }
        
        public bool CheckFtp()
        {
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create(_RootUri);

                ftprequest.Credentials = new NetworkCredential(UserName, Password);
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftprequest.Timeout = 1000;
                ftprequest.UsePassive = _UsePassive;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftprequest.GetResponse();

                ftpResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        #endregion

        #region 目录切换操作

        public bool GotoDirByFullPath(string FullPath)
        {
            FullPath = FullPath.Replace("\\", "/");
            if (FullPath.StartsWith("./") || FullPath.StartsWith("."))
            {
                return GotoDirectory(FullPath);
            }
            else if (FullPath.StartsWith("/"))
            {
                FullPath = "." + FullPath;
            }

            return GotoDirectory(FullPath);
        }

        /// <summary>
        /// 进入一个目录
        /// </summary>
        /// <param name="DirectoryName">
        /// 新目录的名字 
        /// 一、若新目录是当前目录的子目录，则直接指定子目录。如: DirectoryName='SubDirectory1'; 
        /// 二、若新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
         /// </param>
        public bool GotoDirectory(string DirectoryName)
        {
            string CurrentWorkPath = this.DirectoryPath;
            try
            {
                DirectoryName = DirectoryName.Replace("\\", "/");
                string[] DirectoryNames = DirectoryName.Split(new char[] { '/' });
                if (DirectoryNames[0] == ".")
                {
                    DirectoryPath = "/";
                    if (DirectoryNames.Length == 1)
                    {
                        return true;
                    }
                    Array.Clear(DirectoryNames, 0, 1);
                }
                bool Success = false;
                foreach (string dir in DirectoryNames)
                {
                    if (dir != null && dir.Trim()!="")
                    {
                        Success = EnterOneSubDirectory(dir);
                        if (!Success)
                        {
                            this.DirectoryPath = CurrentWorkPath;
                            return false;
                        }
                    }
                }
                return Success;

            }
            catch (Exception ep)
            {
                this.DirectoryPath = CurrentWorkPath;
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }

        /// <summary>
        /// 进入一个目录(若不存在则创建)
        /// </summary>
        /// <param name="DirectoryName">
        /// 新目录的名字 
        /// 一、若新目录是当前目录的子目录，则直接指定子目录。如: DirectoryName='SubDirectory1'; 
        /// 二、若新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
         /// </param>
        public bool GotoOrCreateDir(string DirectoryName)
        {
            string CurrentWorkPath = this.DirectoryPath;
            try
            {
                DirectoryName = DirectoryName.Replace("\\", "/");
                string[] DirectoryNames = DirectoryName.Split(new char[] { '/' });
                if (DirectoryNames[0] == ".")
                {
                    this.DirectoryPath = "/";
                    if (DirectoryNames.Length == 1)
                    {
                        return true;
                    }
                    Array.Clear(DirectoryNames, 0, 1);
                }
                bool Success = false;
                foreach (string dir in DirectoryNames)
                {
                    if (dir != null && dir.Trim() != "")
                    {
                        if (!DirectoryExist(dir))
                            MakeDirectory(dir);
                        Success = EnterOneSubDirectory(dir);
                        if (!Success)
                        {
                            this.DirectoryPath = CurrentWorkPath;
                            return false;
                        }
                    }
                }
                return Success;

            }
            catch (Exception ep)
            {
                this.DirectoryPath = CurrentWorkPath;
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }

        /// <summary>
                /// 从当前工作目录进入一个子目录
                /// </summary>
                /// <param name="DirectoryName">子目录名称</param>
        private bool EnterOneSubDirectory(string DirectoryName)
        {
            try
            {
                if (DirectoryName.IndexOf("/") >= 0 || !IsValidPathChars(DirectoryName))
                {
                    throw new Exception("目录名非法!");
                }
                if (DirectoryName.Length > 0 && DirectoryExist(DirectoryName))
                {
                    if (!DirectoryName.EndsWith("/"))
                    {
                        DirectoryName += "/";
                    }
                    _DirectoryPath += DirectoryName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }

        public string UpperDir(string dir)
        {
            string result = dir;

            result = result.Replace("//", "/");

            if (result == "/")
            {
                return dir;
            }

            if (result.EndsWith("/")) result = result.Substring(0,
                result.Length - 1);//删除末尾的斜杠
            int index = result.LastIndexOf("/");
            if (index >= result.Length - 1 || index < 0)
            {
                return dir;
            }

            result = result.Substring(0, index);
            if (!result.EndsWith("/")) result += "/";
            
            return result;
        }

        /// <summary>
        /// 从当前工作目录往上一级目录
        /// </summary>
        public bool ComeOutDirectory()
        {
            string dir = UpperDir(_DirectoryPath);
            _DirectoryPath = dir;
            
            return true;
            //string temp = _DirectoryPath;
            //_DirectoryPath = _DirectoryPath.Replace("//", "/");

            //if (_DirectoryPath == "/")
            //{
            //    return false;
            //}

            //if (_DirectoryPath.EndsWith("/")) _DirectoryPath = _DirectoryPath.Substring(0,
            //    _DirectoryPath.Length - 1);//删除末尾的斜杠
            //int index = _DirectoryPath.LastIndexOf("/");
            //if (index >= _DirectoryPath.Length - 1)
            //{
            //    _DirectoryPath = temp;
            //    return false;
            //}

            //_DirectoryPath = _DirectoryPath.Substring(0, index);
            //if (!_DirectoryPath.EndsWith("/")) _DirectoryPath += "/";
            //return true;
        }

        #endregion

        #region 文件、目录名称有效性判断

        /// <summary>
        /// 判断目录名中字符是否合法
        /// </summary>
        /// <param name="DirectoryName">目录名称</param>
        public bool IsValidPathChars(string DirectoryName)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            char[] DirChar = DirectoryName.ToCharArray();
            foreach (char C in DirChar)
            {
                if (Array.BinarySearch(invalidPathChars, C) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// 判断文件名中字符是否合法
        /// </summary>
        /// <param name="FileName">文件名称</param>
        public bool IsValidFileChars(string FileName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            char[] NameChar = FileName.ToCharArray();
            foreach (char C in NameChar)
            {
                if (Array.BinarySearch(invalidFileChars, C) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 目录或文件存在的判断

        /// <summary>
        /// 判断当前目录下指定的子目录是否存在
        /// </summary>
        /// <param name="RemoteDirectoryName">指定的目录名</param>
        public bool DirectoryExist(string RemoteDirectoryName)
        {
            try
            {
                if (!IsValidPathChars(RemoteDirectoryName))
                {
                    return false;
                    //throw new Exception("目录名非法！");
                }
                FileStruct[] listDir = ListDirectories();
                foreach (FileStruct dir in listDir)
                {
                    if (dir.Name == RemoteDirectoryName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ep)
            {
                //MessageBox.Show(ep.ToString());
                ep.ToString();
                return false;
            }
        }

        public bool DirExistByFullPath(string path)
        {
            try
            {
                if (!path.EndsWith("/")) path += "/";
                Response = Open(new Uri(_RootUri + path), WebRequestMethods.Ftp.ListDirectoryDetails);
                return true;
            }
            catch (Exception ep)
            {
                ep.ToString();
                return false;
            }
        }

        /// <summary>
        /// 判断一个远程文件是否存在服务器当前目录下面
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        public bool FileExist(string RemoteFileName)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("文件名非法！");
                }
                FileStruct[] listFile = ListFiles();
                foreach (FileStruct file in listFile)
                {
                    if (file.Name == RemoteFileName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }

        /// <summary>
        /// 判断一个远程文件是否存在服务器当前目录下面,在则把名称加上序号
        /// </summary>
        /// <param name="RemoteFileName"></param>
        /// <returns></returns>
        public string FileUniqueName(string RemoteFileName)
        {
            string result = RemoteFileName;
            int i = 1;
            while (FileExist(result))
            {
                result = "(" + (i++).ToString() + ")" + RemoteFileName;
            }
            return result;
        }

        #endregion

        #region 列出目录文件信息
        
        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件和目录
        /// </summary>
        public FileStruct[] ListFilesAndDirectories()
        {
            FtpWebRequest Request = null;
            FtpWebResponse Response = null;
            StreamReader stream = null;
            try
            {
                Request = (FtpWebRequest)WebRequest.Create(this.WorkUri);
                Request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                Request.UseBinary = true;
                Request.UsePassive = _UsePassive;
                Request.Credentials = new NetworkCredential(this.UserName, this.Password);
                if (Proxy != null)
                {
                    Request.Proxy = this.Proxy;
                }
                Response = (FtpWebResponse)Request.GetResponse();
                stream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
                string Datastring = stream.ReadToEnd();
                FileStruct[] list = GetList(Datastring, _DirectoryPath);
                return list;
                
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
            finally
            {
                if(Request!=null)
                {
                    Request.Abort();
                    Request = null;
                }
                if(Response!=null)
                {
                    Response.Close();
                    Response = null;
                }
                if(stream!=null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                
            }

            //Response = Open(this.WorkUri, WebRequestMethods.Ftp.ListDirectoryDetails);
            //StreamReader stream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
            //string Datastring = stream.ReadToEnd();
            //FileStruct[] list = GetList(Datastring,_DirectoryPath);
            //return list; 
        }

        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件
        /// </summary>
        public FileStruct[] ListFiles()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listFile = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (!file.IsDirectory)
                {
                    listFile.Add(file);
                }
            }
            return listFile.ToArray();
        }

        /// <summary>
        /// 列出FTP服务器上面当前目录的所有的目录
        /// </summary>
        public FileStruct[] ListDirectories()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listDirectory = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (file.IsDirectory)
                {
                    listDirectory.Add(file);
                }
            }
            return listDirectory.ToArray();
        }

        public FileStruct[] ListFilesAndDirsByFullPath(string path)
        {
            if (!path.EndsWith("/")) path += "/";

            Response = Open(new Uri(_RootUri + path), WebRequestMethods.Ftp.ListDirectoryDetails);
            StreamReader stream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
            string Datastring = stream.ReadToEnd();
            FileStruct[] list = GetList(Datastring,path);
            return list;
        }
        
        public FileStruct[] ListFilesByFullPath(string path)
        {
            if (!path.EndsWith("/")) path += "/";

            FileStruct[] listAll = ListFilesAndDirsByFullPath(path);
            List<FileStruct> listFile = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (!file.IsDirectory)
                {
                    listFile.Add(file);
                }
            }
            return listFile.ToArray();
        }
        
        public FileStruct[] ListDirsByFullPath(string path)
        {
            if (!path.EndsWith("/")) path += "/";

            FileStruct[] listAll = ListFilesAndDirsByFullPath(path);
            List<FileStruct> listDirectory = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (file.IsDirectory)
                {
                    listDirectory.Add(file);
                }
            }
            return listDirectory.ToArray();
        }

        /// <summary>
        /// 获得文件和目录列表
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>
        private FileStruct[] GetList(string datastring,string parentPath)
        {
            List<FileStruct> myListArray = new List<FileStruct>();
            string[] dataRecords = datastring.Split('\n');
            FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
            foreach (string s in dataRecords)
            {
                if (_directoryListStyle != FileListStyle.Unknown && s != "")
                {
                    FileStruct f = new FileStruct();
                    f.Name = "..";
                    switch (_directoryListStyle)
                    {
                        case FileListStyle.UnixStyle:
                            f = ParseFileStructFromUnixStyleRecord(s);
                            break;
                        case FileListStyle.WindowsStyle:
                            f = ParseFileStructFromWindowsStyleRecord(s,parentPath);
                            break;
                    }
                    if (!(f.Name == "." || f.Name == ".."))
                    {
                        myListArray.Add(f);
                    }
                }
            }
            return myListArray.ToArray();
        }

        /// <summary>
        /// 从Windows格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        private FileStruct ParseFileStructFromWindowsStyleRecord(string Record,string parentPath)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
                f.Name = processstr;
                f.Size = 0;
                f.Extension = "";
            }
            else
            {
                f.IsDirectory = false;
                
                processstr.Replace("<DIR>", "");
                processstr = processstr.TrimStart();
                double.TryParse(processstr.Substring(0, processstr.IndexOf(" ")), out f.Size);
                f.Name = processstr.Substring(processstr.IndexOf(" "), processstr.Length - processstr.IndexOf(" ")).TrimStart().TrimEnd();

                if (f.Name.LastIndexOf(".") > 0)
                {
                    if (f.Name.LastIndexOf(".") < f.Name.Length - 1)
                        f.Extension = f.Name.Substring(f.Name.LastIndexOf(".") + 1,
                            f.Name.Length - f.Name.LastIndexOf(".") - 1);
                }
                else
                {
                    f.Extension = "";
                }

            }
            f.FullPath = parentPath + f.Name;
            return f;
        }
        
        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        /// <param name="recordList">文件信息列表</param>
        private FileListStyle GuessFileListStyle(string[] recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10
                && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return FileListStyle.UnixStyle;
                }
                else if (s.Length > 8
                 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return FileListStyle.WindowsStyle;
                }
            }
            return FileListStyle.Unknown;
        }

        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)  //time
            {
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;   //最后就是名称
            f.Size = 0;
            f.Extension = "";
            return f;
        }

        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }
        #endregion

        #region 建立、删除子目录

        /// <summary>
        /// 在FTP服务器上当前工作目录建立一个子目录
        /// </summary>
         /// <param name="DirectoryName">子目录名称</param>
        public bool MakeDirectory(string DirectoryName)
        {
            try
            {
                if (!IsValidPathChars(DirectoryName))
                {
                    return false;
                    //throw new Exception("目录名非法！");
                }
                if (DirectoryExist(DirectoryName))
                {
                    return false;
                    //throw new Exception("服务器上面已经存在同名的文件名或目录名！");
                }
                Response = Open(new Uri(this.WorkUri.ToString() + DirectoryName), 
                    WebRequestMethods.Ftp.MakeDirectory);
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                ex.ToString();
                return false;
            }
        }
        
        /// <summary>
        /// 从当前工作目录中删除一个空子目录
        /// </summary>
        /// <param name="DirectoryName">子目录名称</param>
        public bool RemoveDirectory(string DirectoryName)
        {
            try
            {
                if (!IsValidPathChars(DirectoryName))
                {
                    throw new Exception("目录名非法！");
                }
                if (!DirectoryExist(DirectoryName))
                {
                    throw new Exception("服务器上面不存在指定的文件名或目录名！");
                }
                Response = Open(new Uri(this.WorkUri.ToString() + DirectoryName), 
                    WebRequestMethods.Ftp.RemoveDirectory);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        public bool RemoveDirectoryByFullPath(string path)
        {
            try
            {
                if (!DirectoryExist(path))
                {
                    return false;
                }
                Response = Open(new Uri(_RootUri + path),
                    WebRequestMethods.Ftp.RemoveDirectory);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        public bool TruncateDirectory(string DirectoryName)
        {
            string CurrentWorkPath = this.DirectoryPath;
            if (CurrentWorkPath.StartsWith("/")) CurrentWorkPath = "." + CurrentWorkPath;

            try
            {
                GotoDirectory(DirectoryName);
                FileStruct[] list = ListFilesAndDirectories();
                foreach (FileStruct file in list)
                {
                    if (!file.IsDirectory)
                    {
                        DeleteFile(file.Name);
                    }
                    else
                    {
                        TruncateDirectory(file.Name);
                    }
                }
                //最后删除此文件夹
                GotoDirectory(CurrentWorkPath);
                bool result = RemoveDirectory(DirectoryName);
                return result;
            }
            catch (Exception ex)
            {
                ex.ToString();
                GotoDirectory(CurrentWorkPath);
                return false;
            }
        }

        #endregion

        #region 上传文件
        
        /// <summary>
        /// 上传小于100M的文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件名</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        public bool UploadSmallFile(string LocalFullPath, string RemoteFileName, bool OverWriteRemoteFile)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(Path.GetFileName(LocalFullPath)) || !IsValidPathChars(Path.GetDirectoryName(LocalFullPath)))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                if (File.Exists(LocalFullPath))
                {
                    FileStream stream = new FileStream(LocalFullPath, FileMode.Open, FileAccess.Read);
                    if (stream.Length > SmallFileSize)
                        throw new Exception("文件过大上传失败");
                    //byte[] bt = new byte[Stream.Length];//单位是B
                    //Stream.Read(bt, 0, (Int32)Stream.Length);//注意，因为Int32的最大限制，最大上传文件只能是大约2G多一点
                    bool bl = UploadFile(stream, RemoteFileName, OverWriteRemoteFile,null);
                    stream.Close();
                    stream.Dispose();
                    return bl;
                }
                else
                {
                    throw new Exception("本地文件不存在!");
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }

        /// <summary>
        /// 上传小于2G的文件到服务器
        /// 有错误会throw
        /// </summary>
        /// <param name="LocalFullPath"></param>
        /// <param name="RemoteFileName"></param>
        /// <param name="OverWriteRemoteFile"></param>
        /// <param name="UpdateProgress">委托事件</param>
        /// <returns></returns>
        public bool UploadMediumFile(string LocalFullPath, string RemoteFileName, bool OverWriteRemoteFile, 
            Action<double, double> UpdateProgress)
        {
            FileStream stream = null;
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(Path.GetFileName(LocalFullPath)) || !IsValidPathChars(Path.GetDirectoryName(LocalFullPath)))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                if (!File.Exists(LocalFullPath))
                {
                    throw new Exception("本地文件不存在");
                }
                stream = new FileStream(LocalFullPath, FileMode.Open, FileAccess.Read);
                if (stream.Length > LargeFileSize)
                    throw new Exception("文件过大上传失败");

                bool bl = UploadFile(stream, RemoteFileName, OverWriteRemoteFile, UpdateProgress);

                return bl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(stream!=null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        /// <summary>
        /// 上传文件到FTP服务器 文件大小小于LargeFileSize = 2G
        /// </summary>
        /// <param name="FileBytes">文件二进制内容</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        /// <param name="UpdateProgress">委托事件</param>
        public bool UploadFile(FileStream stream, string RemoteFileName, bool OverWriteRemoteFile,
            Action<double, double> UpdateProgress = null)
        {
            Stream requestStream = null;
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("非法文件名！");
                }
                if (!OverWriteRemoteFile && FileExist(RemoteFileName))
                {
                    throw new Exception("FTP服务上面已经存在同名文件！");
                }
                
                ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(WorkUri.ToString() + RemoteFileName));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = _UsePassive;               
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                
                requestStream = ftpRequest.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                int TotalRead = 0;
                int percent = -1;
                while (true)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    TotalRead += bytesRead;
                    requestStream.Write(buffer, 0, bytesRead);
                    //更新进度                       
                    int newPercent = (int)Math.Floor((double)TotalRead / stream.Length * 100);
                    if (newPercent > percent)
                    {
                        percent = newPercent;
                        UpdateProgress?.Invoke(TotalRead, stream.Length);//更新进度条     
                    }
                }

                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
            finally
            {
                if(requestStream!=null)
                {
                    requestStream.Close();
                    requestStream.Dispose();
                    requestStream = null;
                }
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                if (ftpResponse != null)
                {
                    ftpResponse.Close();
                    ftpResponse = null;
                }
                if (ftpRequest != null)
                {
                    ftpRequest.Abort();
                    ftpRequest = null;
                }
            }
        }

        /// <summary>
        /// 上传流到服务器，最大2G
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="RemoteFileName"></param>
        /// <param name="OverWriteRemoteFile"></param>
        /// <returns></returns>
        public bool UploadFile(Stream stream, string RemoteFileName, bool OverWriteRemoteFile)
        {
            Stream requestStream = null;
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("非法文件名！");
                }
                if (!OverWriteRemoteFile && FileExist(RemoteFileName))
                {
                    throw new Exception("FTP服务上面已经存在同名文件！");
                }

                ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(WorkUri.ToString() + RemoteFileName));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = _UsePassive;               
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                requestStream = ftpRequest.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                int TotalRead = 0;
                while (true)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    TotalRead += bytesRead;
                    requestStream.Write(buffer, 0, bytesRead);
                    
                }

                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                    requestStream.Dispose();
                    requestStream = null;
                }
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                if (ftpResponse != null)
                {
                    ftpResponse.Close();
                    ftpResponse = null;
                }
                if (ftpRequest != null)
                {
                    ftpRequest.Abort();
                    ftpRequest = null;
                }
            }

            //try
            //{
            //    if (!IsValidFileChars(RemoteFileName))
            //    {
            //        throw new Exception("非法文件名！");
            //    }
            //    if (!OverWriteRemoteFile && FileExist(RemoteFileName))
            //    {
            //        throw new Exception("FTP服务上面已经存在同名文件！");
            //    }

            //    Response = Open(new Uri(this.WorkUri.ToString() + RemoteFileName), WebRequestMethods.Ftp.UploadFile);
            //    Stream requestStream = Request.GetRequestStream();

            //    byte[] buffer = new byte[1024];
            //    int bytesRead = 0;
            //    int TotalRead = 0;
            //    //int percent = -1;
            //    while (true)
            //    {
            //        bytesRead = stream.Read(buffer, 0, buffer.Length);
            //        if (bytesRead == 0)
            //            break;
            //        TotalRead += bytesRead;
            //        requestStream.Write(buffer, 0, bytesRead);
                    
            //    }
            //    requestStream.Close();
            //    Response = (FtpWebResponse)Request.GetResponse();

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    ex.ToString();
            //    return false;
            //}
        }

        /// <summary>
        /// 服务器内复制文件
        /// </summary>
        /// <param name="SourceFullPath">源文件全路径</param>
        /// <param name="TargetName">目标名称</param>
        /// <param name="UpdateProgress"></param>
        /// <returns></returns>
        public bool CopyFile(string SourceFullPath, string TargetName, Action<double, double> UpdateProgress = null)
        {
            try
            {
                string s = SourceFullPath.Replace(".", "").Replace("/", "");
                string t = (_DirectoryPath+TargetName).Replace(".", "").Replace("/", "");
                if (s.Equals(t) || t.StartsWith(s)) return false;
                                
                Response2 = Open(new Uri(RootUri + SourceFullPath), WebRequestMethods.Ftp.DownloadFile);
                Stream SourceStream = Response2.GetResponseStream();

                bool bl = UploadFile(SourceStream, TargetName, true);

                SourceStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }
        
        #endregion

        #region 下载文件、文件夹

        public long GetFileSize(string RemoteFileName)
        {
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(WorkUri.ToString() + RemoteFileName));
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = _UsePassive;               
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                long size = ftpResponse.ContentLength;
                return size;
            }
            catch (Exception ex)
            {
                return -1;
                throw ex;
            }
            finally
            {
                if (ftpResponse != null)
                {
                    ftpResponse.Close();
                    ftpResponse = null;
                }
                if (ftpRequest != null)
                {
                    ftpRequest.Abort();
                    ftpRequest = null;
                }
            }
        }

        public long GetFileSizeByFullPath(string path)
        {
            try
            {
                Response = Open(new Uri(RootUri + path), WebRequestMethods.Ftp.GetFileSize);
                long size = Response.ContentLength;
                return size;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return -1;
            }
        }

        /// <summary>
        /// 下载并覆盖文件
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="RemoteFileName"></param>
        public void DownloadFile(string fileFullPath, string RemoteFileName,
            Action<double, double> UpdateProgress)
        {
            FileStream fileStream = null;
            Stream ftpStream = null;
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;
            try
            {
                long TotalSize = GetFileSize(RemoteFileName);

                //获得源stream
                ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(WorkUri.ToString() + RemoteFileName));
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = _UsePassive;               
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();                
                ftpStream = ftpResponse.GetResponseStream();
                
                int bytesRead;
                int TotalRead = 0;
                int percent = -1;
                byte[] buffer = new byte[2048];

                //创建目标filestream
                fileStream = new FileStream(fileFullPath, FileMode.Create);

                while (true)
                {
                    bytesRead = ftpStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    TotalRead += bytesRead;
                    fileStream.Write(buffer, 0, bytesRead);
                    //更新进度                       
                    int newPercent = (int)Math.Floor((double)TotalRead / TotalSize * 100);
                    if (newPercent > percent)
                    {
                        percent = newPercent;
                        UpdateProgress?.Invoke(TotalRead, TotalSize);//更新进度条     
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                    ftpStream.Dispose();
                    ftpStream = null;
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    fileStream = null;
                }
                if (ftpResponse != null)
                {
                    ftpResponse.Close();
                    ftpResponse = null;
                }
                if (ftpRequest != null)
                {
                    ftpRequest.Abort();
                    ftpRequest = null;
                }
            }
        }

        /// <summary>
        /// 下载并覆盖合并文件夹
        /// </summary>
        /// <param name="TargetFullPath">目标地址=父目录/RemoteFolder</param>
        /// <param name="RemoteFolder">服务器待下载目录</param>
        /// <param name="UpdateProgress"></param>
        public void DownloadFolder(string TargetFullPath, string RemoteFolder,
            Action<double, double> UpdateProgress)
        {
            string CurrentWorkPath = this.DirectoryPath;
            if (CurrentWorkPath.StartsWith("/")) CurrentWorkPath = "." + CurrentWorkPath;

            DirectoryInfo TargetFolder = new DirectoryInfo(TargetFullPath);
            if (!TargetFolder.Exists)
                TargetFolder.Create();

            GotoDirectory(RemoteFolder);
            FileStruct[] files = ListFilesAndDirectories();
            foreach (FileStruct file in files)
            {
                try
                {
                    if (file.IsDirectory)
                    {
                        DownloadFolder(TargetFolder + "\\" + file.Name, file.Name, UpdateProgress);
                    }
                    else
                    {
                        DownloadFile(TargetFolder + "\\" + file.Name, file.Name, UpdateProgress);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            GotoDirectory(CurrentWorkPath);
        }

        public void DownloadLittleFile(string fileFullPath,string RemoteFileName)
        {
            FileStream fileStream = null;
            Stream ftpStream = null;
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;
            try
            {
                fileStream = new FileStream(fileFullPath, FileMode.Create);

                ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(WorkUri.ToString() + RemoteFileName));
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = _UsePassive;               
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                
                int bytesRead;
                int TotalRead = 0;
                byte[] buffer = new byte[2048];

                while (true)
                {
                    bytesRead = ftpStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    TotalRead += bytesRead;
                    fileStream.Write(buffer, 0, bytesRead);
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                    ftpStream.Dispose();
                    ftpStream = null;
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    fileStream = null;
                }
                if (ftpResponse != null)
                {
                    ftpResponse.Close();
                    ftpResponse = null;
                }
                if (ftpRequest != null)
                {
                    ftpRequest.Abort();
                    ftpRequest = null;
                }
            }
        }

        #endregion

        #region 删除文件
        /// <summary>
        /// 从FTP服务器上面删除一个文件
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        public void DeleteFile(string RemoteFileName)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("文件名非法！");
                }
                Uri uri = new Uri(WorkUri.ToString() + RemoteFileName);
                Response = Open(uri, WebRequestMethods.Ftp.DeleteFile);
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }

        public void DeleteFileByFullPath(string path)
        {
            try
            {
                Uri uri = new Uri(RootUri + path);
                Response = Open(uri, WebRequestMethods.Ftp.DeleteFile);
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }


        #endregion

        #region 重命名文件

        /// <summary>
        /// 更改一个文件的名称或一个目录的名称 TODO-重名会不会覆盖？
        /// </summary>
        /// <param name="RemoteFileName">原始文件或目录名称</param>
        /// <param name="NewFileName">新的文件或目录的名称</param>
        public bool ReName(string RemoteFileName, string NewFileName)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(NewFileName))
                {
                    throw new Exception("文件名非法！");
                }
                if (RemoteFileName == NewFileName)
                {
                    return true;
                }
                if (FileExist(RemoteFileName) || DirectoryExist(RemoteFileName))
                {
                    Request = OpenRequest(new Uri(this.WorkUri.ToString() + RemoteFileName), WebRequestMethods.Ftp.Rename);
                    Request.RenameTo = NewFileName;
                    
                    Response = (FtpWebResponse)Request.GetResponse();
                }
                else
                {
                    throw new Exception("文件在服务器上不存在！");
                }
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                return false;
            }
        }

        #endregion

        #region 拷贝、移动文件
        
        /// <summary>
        /// 把当前目录下面的一个文件移动到服务器上面另外的目录中
        /// 注意：移动文件之后当前工作目录还是文件原来所在的目录
        /// </summary>
        /// <param name="RemoteFile">当前目录下的文件名</param>
        /// <param name="DirectoryName">新目录名称，TODO 不存在则创建
        /// 说明：如果新目录是当前目录的子目录，则直接指定子目录。如: SubDirectory1/SubDirectory2 ；
        /// 如果新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
        /// </param>
        /// <returns></returns>
        public bool MoveFileToAnotherDirectory(string RemoteFile, string DirectoryName)
        {
            string CurrentWorkDir = this.DirectoryPath;
            try
            {
                if (DirectoryName == "")
                    return false;
                if (!DirectoryName.StartsWith("/"))
                    DirectoryName = "/" + DirectoryName;
                if (!DirectoryName.EndsWith("/"))
                    DirectoryName += "/";
                bool Success = ReName(RemoteFile, DirectoryName + RemoteFile);
                this.DirectoryPath = CurrentWorkDir;
                return Success;
            }
            catch (Exception ep)
            {
                this.DirectoryPath = CurrentWorkDir;
                ErrorMsg = ep.ToString();
                throw ep;
            }
        }
        #endregion
    }
}
