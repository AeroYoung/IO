using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

//private void TestINI_Load(object sender, System.EventArgs e)
//  {
//   //创建一个INIFile对象，参数为文件路径，如果不存在它会自动创建的
//   INIFile inf=new INIFile(@"D:\工作目录\VSPP\VSPPServer\bin\Debug\COMPILED.INI");
//   //显示INI配置的结构
//   foreach(string k in inf.Segments.Keys)
//   {
//    TreeNode o=new TreeNode(k);
//    INISegment s=inf.Segments[k]; //取出当前配置节
//    foreach(string k1 in s.Items.Keys)
//    {
//     TreeNode o1=new TreeNode(k1+" = "+s.Items[k1].Value); //访问配置节中每个配置项
//     o.Nodes.Add(o1);
//    }
//    treeView1.Nodes.Add(o);
//   }
//   //添加一个配置
//   inf.Segments.Add("Test"); //添加Test节
//   inf.Segments["Test"].Items.Add("IP","192.168.0.1"); //添加一个配置项
//   //快速添加一个配置项
//   inf.Segments["测试"].Items["Host"].Value="localhost"; // ：）和上面两行一样的效果
//   //读取也是同样的方便
//   MessageBox.Show(inf.Segments["测试"].Items["Host"].Value);
//   //如果不存在这样的配置项，将返回空字符串并创建这个项
//   MessageBox.Show(inf.Segments["测试"].Items["Server"].Value);
//   //清除一个配置节下面的所有配置项
//   //inf.Segments["Action"].Clear();
//  }

namespace ExpertLib.IO
{
    #region INI文件操作类
    /// <summary>
    /// 配置节
    /// </summary>
    public class INISegment
    {
        private string __Name;
        private INISegments __Owner;
        /// <summary>
        /// 所有配置项集合
        /// </summary>
        public INIItems Items;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="o">Owner</param>
        /// <param name="vName">配置节名称</param>
        public INISegment(INISegments o, string vName)
        {
            __Owner = o;
            __Name = vName;
            Items = new INIItems(this);
            o.Owner.GetSegment(this);
        }
        /// <summary>
        /// 获取配置节的名称
        /// </summary>
        public string Name
        {
            get { return __Name; }
        }
        /// <summary>
        /// 获取Segment的Owner：INISegments集合
        /// </summary>
        public INISegments Owner
        {
            get { return __Owner; }
        }
        /// <summary>
        /// 清除所有设置项
        /// </summary>
        public void Clear()
        {
            __Owner.Owner.WriteSegment(__Name, "\0\0");
        }
    }
    
    /// <summary>
    /// 配置节集合
    /// </summary>
    public class INISegments : DictionaryBase
    {
        private INIFile __Owner;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="o">Owner</param>
        public INISegments(INIFile o)
        {
            __Owner = o;
        }
        /// <summary>
        /// 获取此对象的Owner：INIFile类
        /// </summary>
        public INIFile Owner
        {
            get { return __Owner; }
        }
        /// <summary>
        /// 添加一个已经存在的配置节
        /// </summary>
        /// <param name="o">配置节对象</param>
        public void Add(INISegment o)
        {
            if (!Dictionary.Contains(o.Name))
                Dictionary.Add(o.Name, o);
        }
        /// <summary>
        /// 添加一个可能不存在的配置节（创建一个配置节）
        /// </summary>
        /// <param name="vName">配置节名称</param>
        /// <returns>添加的配置节</returns>
        public INISegment Add(string vName)
        {
            if (Dictionary.Contains(vName))
                return (INISegment)Dictionary[vName];
            INISegment o = new INISegment(this, vName);
            Dictionary.Add(vName, o);
            return o;
        }
        /// <summary>
        /// 获取索引集合
        /// </summary>
        public ICollection Keys
        {
            get { return Dictionary.Keys; }
        }
        /// <summary>
        /// 获取值集合
        /// </summary>
        public ICollection Values
        {
            get { return Dictionary.Values; }
        }
        /// <summary>
        /// 获取配置节
        /// </summary>
        public INISegment this[string vName]
        {
            get
            {
                if (!Dictionary.Contains(vName))
                    return this.Add(vName);
                else
                    return (INISegment)Dictionary[vName];
            }
        }
        /// <summary>
        /// 获取是否包含某配置节
        /// </summary>
        /// <param name="vName">配置节名称</param>
        /// <returns>是否</returns>
        public bool Contains(string vName)
        {
            return Dictionary.Contains(vName);
        }
    }
    
    /// <summary>
    /// 配置项
    /// </summary>
    public class INIItem
    {
        private string __Name;
        private string __Value;
        private INIItems __Owner;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="o">Owner</param>
        /// <param name="vName">名称</param>
        /// <param name="vValue">值</param>
        public INIItem(INIItems o, string vName, string vValue)
        {
            __Owner = o;
            __Name = vName;
            __Value = vValue;
            if (!o.Contains(vName))
                o.Owner.Owner.Owner.SetString(o.Owner.Name, vName, vValue);
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name
        {
            get { return __Name; }
        }
        /// <summary>
        /// 获取设置值
        /// </summary>
        public string Value
        {
            get { return __Value; }
            set
            {
                __Value = value;
                __Owner.Owner.Owner.Owner.SetString(__Owner.Owner.Name, __Name, value);
            }
        }
        /// <summary>
        /// 获取Owner
        /// </summary>
        public INIItems Owner
        {
            get { return __Owner; }
        }
    }
    
    /// <summary>
    /// 配置项集合
    /// </summary>
    public class INIItems : DictionaryBase
    {
        private INISegment __Owner;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="o">Owner</param>
        public INIItems(INISegment o)
        {
            __Owner = o;
        }
        /// <summary>
        /// 获取Owner
        /// </summary>
        public INISegment Owner
        {
            get { return __Owner; }
        }
        /// <summary>
        /// 添加一个已经存在的配置项
        /// </summary>
        /// <param name="o">配置项</param>
        public void Add(INIItem o)
        {
            if (!Dictionary.Contains(o.Name))
                Dictionary.Add(o.Name, o);
        }
        /// <summary>
        /// 获取是否包含指定名称的配置项
        /// </summary>
        /// <param name="vName">配置项名称</param>
        /// <returns>是否</returns>
        public bool Contains(string vName)
        {
            return Dictionary.Contains(vName);
        }
        /// <summary>
        /// 获取所有的索引集合
        /// </summary>
        public ICollection Keys
        {
            get { return Dictionary.Keys; }
        }
        /// <summary>
        /// 获取所有的值集合
        /// </summary>
        public ICollection Values
        {
            get { return Dictionary.Values; }
        }
        /// <summary>
        /// 添加一个可能不存在的配置项（创建一个配置项）
        /// </summary>
        /// <param name="vName">配置项名</param>
        /// <param name="vValue">配置项值</param>
        /// <returns>创建的配置项INIItem对象</returns>
        public INIItem Add(string vName, string vValue)
        {
            if (Dictionary.Contains(vName))
                return (INIItem)Dictionary[vName];
            else
            {
                INIItem o = new INIItem(this, vName, vValue);
                this.Add(o);
                return o;
            }
        }
        /// <summary>
        /// 获取指定索引的配置项
        /// </summary>
        public INIItem this[string vName]
        {
            get
            {
                if (Dictionary.Contains(vName))
                    return (INIItem)Dictionary[vName];
                else
                    return this.Add(vName, "");
            }
        }
    }
    
    /// <summary>
    /// INI文件操作类
    /// </summary>
    public class INIFile
    {
        #region 导入DLL函数
        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileIntA(string segName, string keyName, int iDefault, string fileName);
        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileStringA(string segName, string keyName, string sDefault, StringBuilder retValue, int nSize, string fileName);
        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileSectionA(string segName, byte[] sData, int nSize, string fileName);
        [DllImport("kernel32.dll")]
        public extern static int WritePrivateProfileSectionA(string segName, byte[] sData, string fileName);
        [DllImport("kernel32.dll")]
        public extern static int WritePrivateProfileStringA(string segName, string keyName, string sValue, string fileName);
        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileSectionNamesA(byte[] vData, int iLen, string fileName);
        #endregion

        private string __Path;
        /// <summary>
        /// 所有的配置节
        /// </summary>
        public INISegments Segments;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vPath">INI文件路径</param>
        public INIFile(string vPath)
        {
            __Path = vPath;
            Segments = new INISegments(this);
            byte[] bufsegs = new byte[32767];
            int rel = GetPrivateProfileSectionNamesA(bufsegs, 32767, __Path);
            int iCnt, iPos;
            string tmp;
            if (rel > 0)
            {
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)
                {
                    if (bufsegs[iCnt] == 0x00)
                    {
                        tmp = System.Text.ASCIIEncoding.Default.GetString(bufsegs, iPos, iCnt).Trim();
                        iPos = iCnt + 1;
                        if (tmp != "")
                            Segments.Add(tmp);
                    }
                }
            }
        }
        /// <summary>
        /// 获取INI文件路径
        /// </summary>
        public string Path
        {
            get { return __Path; }
        }
        /// <summary>
        /// 读取一个整数型的配置值
        /// </summary>
        /// <param name="segName">配置节名</param>
        /// <param name="keyName">配置项名</param>
        /// <param name="iDefault">默认值</param>
        /// <returns>配置值</returns>
        public int GetInt(string segName, string keyName, int iDefault)
        {
            return GetPrivateProfileIntA(segName, keyName, iDefault, __Path);
        }

        /// <summary>
        /// 读取一个字符串型配置值
        /// </summary>
        /// <param name="segName">配置节名</param>
        /// <param name="keyName">配置项名</param>
        /// <param name="sDefault">默认值</param>
        /// <returns>配置值</returns>
        public string GetString(string segName, string keyName, string sDefault)
        {
            StringBuilder red = new StringBuilder(1024);
            GetPrivateProfileStringA(segName, keyName, "", red, 1024, __Path);
            return red.ToString();
        }

        /// <summary>
        /// 写入配置项
        /// </summary>
        /// <param name="segName">配置节名</param>
        /// <param name="keyName">配置项名</param>
        /// <param name="vValue">配置值</param>
        public void SetString(string segName, string keyName, string vValue)
        {
            WritePrivateProfileStringA(segName, keyName, vValue, __Path);
        }
        /// <summary>
        /// 写入一个配置节
        /// </summary>
        /// <param name="segName">配置节名</param>
        /// <param name="vData">数据</param>
        /// <remarks>
        /// 数据为多个配置项组成的字符串，每个配置项之间用 "\0" 分割
        /// 字符串最后用 "\0\0" 结束
        /// </remarks>
        /// <example>
        /// WriteSegment(segName,"\0\0"); 可以用于清除一个配置节下的所有配置项
        /// </example>
        public void WriteSegment(string segName, string vData)
        {
            WritePrivateProfileSectionA(segName, System.Text.ASCIIEncoding.Default.GetBytes(vData), __Path);
        }
        /// <summary>
        /// 读取一个配置节下面的所有配置项
        /// </summary>
        /// <param name="o">要读取的配置节</param>
        public void GetSegment(INISegment o)
        {
            byte[] vData = new byte[32767];
            int rLen = GetPrivateProfileSectionA(o.Name, vData, 32767, __Path);
            o.Items.Clear();
            if (rLen < 1) return;
            string tmp = "";
            int iPos, iCnt;
            iPos = 0;
            for (iCnt = 0; iCnt < rLen; iCnt++)
            {
                if (vData[iCnt] == 0x00)
                {
                    tmp = System.Text.ASCIIEncoding.Default.GetString(vData, iPos, iCnt - iPos).Trim();
                    if (tmp != "")
                    {
                        string[] t = tmp.Split('=');
                        if (t.Length <= 1)
                            o.Items.Add(t[0], "");
                        else
                            o.Items.Add(t[0], t[1]);
                    }
                    iPos = iCnt + 1;
                }
            }
        }
    }
    #endregion
}
