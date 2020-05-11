using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ExpertLib.Utils
{    
    public class FtpTaskOld
    {
        public FtpTaskType type;

        public string serverUrl;

        public string localUrl;

        public string Describe {
            get {
                string value = "下载";
                if (type == FtpTaskType.Upload) value = "上传";
                value += "\n 服务器路径:" + serverUrl;
                value += "\n 本地路径:" + localUrl;
                return value;
            }
        }

        public Object tag1;

        public Object tag2;

        public FtpTaskOld(FtpTaskType type, string localUrl, string serverUrl)
        {
            this.type = type;
            this.serverUrl = serverUrl;
            this.localUrl = localUrl;
        }
    }

    public class FtpFileInfoOld
    {
        public FtpFileInfoOld(string root, string url, string value)
        {
            string processstr = value.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            UpDate = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                IsDIR = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
                Name = processstr;

                if (url.Trim().Equals(""))
                {
                    FullName = root + Name;
                    Url = Name;
                }  
                else
                {
                    FullName = root + url + "/" + Name;
                    Url = url + "/" + Name;
                }
                    
            }
            else
            {
                IsDIR = false;

                processstr.Replace("<DIR>", "");
                processstr = processstr.TrimStart();
                double.TryParse(processstr.Substring(0, processstr.IndexOf(" ")),out size);
                Name = processstr.Substring(processstr.IndexOf(" "), processstr.Length- processstr.IndexOf(" ")).TrimStart().TrimEnd();

                if (url.Trim().Equals(""))
                {
                    FullName = root + Name;
                    Url = Name;
                }
                else
                {
                    FullName = root + url + "/" + Name;
                    Url = url + "/" + Name;
                }

                if (Name.LastIndexOf(".") > 0)
                {
                    if (Name.LastIndexOf(".") < Name.Length - 1)
                        Extension = Name.Substring(Name.LastIndexOf(".") + 1, 
                            Name.Length - Name.LastIndexOf(".") - 1);
                    Name = Name.Substring(0, Name.LastIndexOf("."));
                }
            }
            
        }

        public FtpFileInfoOld(List<FtpFileInfoOld> infos)
        {
            Infos = infos;
            IsDIR = true;
        }

        public string Name = "";
        public string Extension = "";

        /// <summary>
        /// 全路径 ，末尾没有'/
        /// </summary>
        public string FullName = "";

        /// <summary>
        /// 全路径不包括IP头，末尾没有'/'
        /// </summary>
        public string Url = "";  // 

        public DateTime UpDate;  
              
        public bool IsDIR = false;

        private double size = 0;

        public double Size { get { return size; } set { size = value;} }        

        public double ReadableSize {
            get {
                var value = size;
                var unit = Utils.HumanReadableFileSize(ref value);
                return value;
            }
        }

        public string Unit {
            get
            {
                var value = size;
                return Utils.HumanReadableFileSize(ref value);
            }
        }

        public List<FtpFileInfoOld> Infos = new List<FtpFileInfoOld>();
    }

    public class FtpCtrlOld
    {
        #region 属性

        private string ftpServerIP;

        public string RootUrl { get { return "ftp://" + ftpServerIP + "/"; } }
        
        private string ftpURI;

        public string FtpURI { get { return ftpURI; } }

        private string ftpUserID;

        private string ftpPassword;

        private bool ifCredential;

        #endregion

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>     
        /// <param name="ifCredential">是否需要密码</param> 
        public FtpCtrlOld(string FtpServerIP, string FtpUserID, string FtpPassword, bool ifCredential)
        {
            ftpServerIP = FtpServerIP;
            
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            ftpURI = "ftp://" + ftpServerIP + "/";
            this.ifCredential = ifCredential;
        }

        #region 检查连接

        public bool CheckFtp()
        {
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create(RootUrl);
                
                if(ifCredential)
                    ftprequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftprequest.Timeout = 3000;

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

        #region 上传下载删除重命名

        public void Upload(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            string uri = ftpURI + fileInf.Name;
            FtpWebRequest reqFTP;

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "Upload Error --> " + ex.Message);
            }
        }
        
        /// <summary>
        /// 上传文件(会覆盖)
        /// </summary>
        /// <param name="serverPath">目标地址，包括文件名，不包括ftp://XX.XX.XX</param>
        /// <param name="localPath"></param>
        /// <param name="updateProgress"></param>
        /// <returns></returns>
        public bool UploadFile(string serverPath, string localPath, Action<double, double> updateProgress = null)
        {
            FtpWebRequest reqFTP;
            Stream stream = null;
            //FtpWebResponse response = null;
            FileStream fs = null;
            try
            {
                FileInfo fileInfo = new FileInfo(localPath);
                if (serverPath.StartsWith("/") && serverPath.Length > 1)
                    serverPath = serverPath.Substring(1, serverPath.Length - 1);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(RootUrl+ serverPath);
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                if (ifCredential)
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;//向服务器发出下载请求命令  
                reqFTP.ContentLength = fileInfo.Length;//为request指定上传文件的大小  
                //response = reqFTP.GetResponse() as FtpWebResponse;
                reqFTP.ContentLength = fileInfo.Length;
                int buffLength = 1024;
                byte[] buff = new byte[buffLength];
                int contentLen;
                fs = fileInfo.OpenRead();
                stream = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                double allbye = fileInfo.Length;
                //更新进度    
                if (updateProgress != null)
                {
                    updateProgress((int)allbye, 0);//更新进度条     
                }
                double startbye = 0;
                int percent = -1;
                while (contentLen != 0)
                {
                    startbye = contentLen + startbye;
                    stream.Write(buff, 0, contentLen);
                    //更新进度                       
                    int newPercent = (int)Math.Floor(startbye / allbye * 100);
                    if (updateProgress != null && newPercent > percent)
                    {
                        percent = newPercent;
                        updateProgress(allbye, startbye);//更新进度条     
                    }
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                stream.Close();
                fs.Close();
                //response.Close();
                return true;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
                throw ex; 
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                //if (response != null)
                //{
                //    response.Close();
                //}
            }
        }
        
        public void Download(string filePath, string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "Download Error --> " + ex.Message);
            }
        }

        public bool DeleteFile(string Url)
        {
            bool success = true;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;

            try
            {
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(RootUrl + Url));
                if (ifCredential)
                    ftpWebRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;

                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

            }
            catch (Exception ex)
            {
                success = false;
                ex.ToString();
            }
            finally
            {
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }

            return success;
        }

        public bool DeleteFolder(string Url)
        {
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(RootUrl + Url));
                if (ifCredential)
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;

                //string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                //long size = response.ContentLength;
                //Stream datastream = response.GetResponseStream();
                //StreamReader sr = new StreamReader(datastream);
                //result = sr.ReadToEnd();
                //sr.Close();
                //datastream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="url">不包括服务器路径但包括文件名 eg: XXX/YY/Z.doc</param>
        /// <param name="newName">包括扩展名</param>
        /// <returns></returns>
        public bool ReName(string url, string newName)
        {
            bool success = true;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;

            try
            {
                
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(RootUrl + url));
                if (ifCredential)
                    ftpWebRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpWebRequest.RenameTo = newName; 

                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                
            }
            catch (Exception ex)
            {
                success = false;
                ex.ToString();
            }
            finally
            {
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }

            return success;
        }

        #endregion

        #region 遍历目录

        /// <summary>
        /// 指定路径下所有文件和文件夹
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<FtpFileInfoOld> GetFileAndFolders(string url)
        {
            List<FtpFileInfoOld> results = new List<FtpFileInfoOld>();
            if (url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);
            try
            {
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(RootUrl + url));
                if (ifCredential)
                    ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string line = reader.ReadLine();
                while (line != null)
                {
                    FtpFileInfoOld file = new FtpFileInfoOld(RootUrl, url, line);

                    if (file.IsDIR)
                    {
                        string subUrl = "";
                        if (url.Trim().Equals(""))
                            subUrl = file.Name;
                        else
                            subUrl = url + "/" + file.Name;
                        file.Infos = GetFileAndFolders(subUrl);
                    }

                    results.Add(file);
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return results;
            }

            return results;
        }

        /// <summary>
        /// 指定路径下所有文件夹
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<FtpFileInfoOld> GetFolders(string url)
        {
            List<FtpFileInfoOld> results = new List<FtpFileInfoOld>();
            if (url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);
            try
            {
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(RootUrl + url));
                if (ifCredential)
                    ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string line = reader.ReadLine();
                while (line != null)
                {
                    FtpFileInfoOld file = new FtpFileInfoOld(RootUrl, url, line);

                    if (file.IsDIR)
                    {
                        string subUrl = "";
                        if (url.Trim().Equals(""))
                            subUrl = file.Name;
                        else
                            subUrl = url + "/" + file.Name;
                        file.Infos = GetFileAndFolders(subUrl);

                        results.Add(file);
                    }

                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "GetFilesDetailList Error --> " + ex.Message);
                return results;
            }

            return results;
        }

        /// <summary>
                /// 获取当前目录下明细(包含文件和文件夹)
                /// </summary>
                /// <returns></returns>
        public string[] GetFilesDetailList()
        {
            string[] downloadFiles;
            try
            {
                StringBuilder result = new StringBuilder();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                if (ifCredential)
                    ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                downloadFiles = null;
                Insert_Standard_ErrorLog.Insert("FtpWeb", "GetFilesDetailList Error --> " + ex.Message);
                return downloadFiles;
            }
        }

        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <returns></returns>
        public string[] GetFileList(string mask)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (mask.Trim() != string.Empty && mask.Trim() != "*.*")
                    {
                        string mask_ = mask.Substring(0, mask.IndexOf("*"));
                        if (line.Substring(0, mask_.Length) == mask_)
                        {
                            result.Append(line);
                            result.Append("\n");
                        }
                    }
                    else
                    {
                        result.Append(line);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                downloadFiles = null;
                if (ex.Message.Trim() != "远程服务器返回错误: (550) 文件不可用(例如，未找到文件，无法访问文件)。")
                {
                    Insert_Standard_ErrorLog.Insert("FtpWeb", "GetFileList Error --> " + ex.Message.ToString());
                }
                return downloadFiles;
            }
        }

        /// <summary>
                /// 获取当前目录下所有的文件夹列表(仅文件夹)
                /// </summary>
                /// <returns></returns>
        public string[] GetDirectoryList()
        {
            string[] drectory = GetFilesDetailList();
            StringBuilder m = new StringBuilder();
            foreach (string str in drectory)
            {
                int dirPos = str.IndexOf("<DIR>");
                if (dirPos > 0)
                {
                    /*判断 Windows 风格*/
                    m.Append(str.Substring(dirPos + 5).Trim());
                    m.Append("\n");
                }
                else if (str.Trim().Substring(0, 1).ToUpper() == "D")
                {
                    /*判断 Unix 风格*/
                    string dir = str.Substring(54).Trim();
                    if (dir != "." && dir != "..")
                    {
                        m.Append(dir);
                        m.Append("\n");
                    }
                }
            }

            char[] n = new char[] { '\n' };
            m.Remove(m.ToString().LastIndexOf("\n"), 1);
            return m.ToString().Split(n);
        }

        #endregion

        #region 是否存在 & 创建目录

        /// <summary>
        /// 路径是否存在，否则创建文件夹
        /// </summary>
        /// <param name="SimpleUrl">简单路径，不包括ftp://XX.XX.XX/ 末尾有没有'/'无所谓</param>
        /// <returns></returns>
        public void DirExistOrCreate(string SimpleUrl)
        {
            SimpleUrl = SimpleUrl.Replace("\\", "/");
            if (SimpleUrl.EndsWith("/"))
                SimpleUrl = SimpleUrl.Substring(0, SimpleUrl.Length - 1);

            string[] values = SimpleUrl.Split(new char[] { '/' });

            string workingDir = "";
            
            for (int i = 0; i < values.Length; i++)
            {
                workingDir = workingDir + values[i] + "/";
                if (!DirExist(workingDir))
                {
                    MakeDir(workingDir);
                }
            }
        }

        public bool DirExist(string SimpleUrl)
        {
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create(RootUrl+ SimpleUrl);
                if (ifCredential)
                    ftprequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;

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

        /// <summary>
                /// 判断当前目录下指定的子目录是否存在
                /// </summary>
                /// <param name="RemoteDirectoryName">指定的目录名</param>
        public bool DirectoryExist(string RemoteDirectoryName)
        {
            string[] dirList = GetDirectoryList();
            foreach (string str in dirList)
            {
                if (str.Trim() == RemoteDirectoryName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
                /// 判断当前目录下指定的文件是否存在
                /// </summary>
                /// <param name="RemoteFileName">远程文件名</param>
        public bool FileExist(string RemoteFileName)
        {
            string[] fileList = GetFileList("*.*");
            foreach (string str in fileList)
            {
                if (str.Trim() == RemoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
                /// 创建文件夹
                /// </summary>
                /// <param name="dirName"></param>
        public void MakeDir(string SimpleUrl)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(RootUrl + SimpleUrl));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        #endregion

        #region 获取信息

        /// <summary>
                /// 获取指定文件大小
                /// </summary>
                /// <param name="filename"></param>
                /// <returns></returns>
        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;

                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "GetFileSize Error --> " + ex.Message);
            }
            return fileSize;
        }

        #endregion
    }

    public class Insert_Standard_ErrorLog
    {
        public static void Insert(string x, string y)
        {

        }
    }
}
