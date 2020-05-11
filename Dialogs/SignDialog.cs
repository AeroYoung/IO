using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using ExpertLib.DataBase;
using ExpertLib.IO;

namespace ExpertLib.Dialogs
{
    public partial class SignDialog : Form
    {
        #region static 操作文件,获得用户信息

        public static Dictionary<string, User> RememberedUsers = new Dictionary<string, User>();
        public static User CurrentUser = new User();
        private static string BinFile => Path.Combine(Application.UserAppDataPath, "AnalysisStudio.Users.bin");

        #endregion

        private readonly SqlHelper sql = new SqlHelper();

        public SignDialog()
        {
            InitializeComponent();
        }
        
        private void FormSignIn_Load(object sender, EventArgs e)
        {
            RememberedUsers = FileCtr.ReadSerializable<string, User>(BinFile);

            foreach (var user in RememberedUsers.Values)
            {
                if (string.IsNullOrWhiteSpace(user.Id))
                    continue;
                titleCombox1.comboBox1.Items.Add(user.Id);
            }

            if (titleCombox1.comboBox1.Items.Count > 0)
            {
                titleCombox1.comboBox1.SelectedIndex = titleCombox1.comboBox1.Items.Count - 1;           
            }

            titleCombox1.SetTextChange(comboBoxEdit1_TextChanged);
            titleCombox1.comboBox1.SelectedIndexChanged += comboBoxEdit1_SelectedIndexChanged;
            titleCombox1.comboBox1.KeyDown += ComboBox1_KeyDown;
            comboBoxEdit1_SelectedIndexChanged(null, null);

            titleTextBox1.textBoxEx1.KeyDown += TextBoxEx1_KeyDown;
        }

        public User GetUserInfo(string input, string pwd)
        {
            var user = new User();

            var dt = sql.ExecuteQuery("select * from UserTable where LoginId=@id and pwd=@pwd", new[]
            {
                new SqlParameter("id", input), 
                new SqlParameter("pwd", pwd),
            }).Tables[0];


            if (dt.Rows.Count <= 0) return user;
            user = new User(dt.Rows[0]);

            return user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region 结果判定

            CurrentUser = GetUserInfo(titleCombox1.Value(), titleTextBox1.Value());
            var signIn = !string.IsNullOrEmpty(CurrentUser.Id);

            #endregion

            #region 记住密码或者清除密码

            RememberedUsers.Clear();
            
            if(!signIn)//登录失败
            {
                RememberedUsers.Add(CurrentUser.Id, new User());
            }
            else
            {
                if (!checkBox1.Checked)//如果没有单击记住密码的功能，则清除密码
                {
                    RememberedUsers.Add(CurrentUser.Id, new User());
                }
                else
                {
                    RememberedUsers.Remove(CurrentUser.Id);
                    RememberedUsers.Add(CurrentUser.Id, CurrentUser);
                }
            }
            RememberedUsers.Remove("");
            RememberedUsers.WriteSerializable(BinFile);

            #endregion

            #region 结果处理

            if (!signIn)
            {
                MessageBox.Show("账号或密码错误！", "登录失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                titleCombox1.SetValue("");
                titleTextBox1.SetValue("");
                titleCombox1.comboBox1.Focus();
            }
            else
            {
                DialogResult = DialogResult.OK;
            }

            #endregion
        }

        #region 用户输入体验

        private void button1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1_Click(sender, e);
            }
        }

        private void ComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                titleTextBox1.Focus();
            }
        }

        private void TextBoxEx1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        #endregion

        #region 联想输入密码

        /// <summary>
        /// 密码免输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            titleTextBox1.SetValue("");
            if (string.IsNullOrWhiteSpace(titleCombox1.Value())) return;
            var input = titleCombox1.Value();
            foreach (var user in RememberedUsers.Values)
            {
                if (!user.Id.Equals(input)) continue;
                titleTextBox1.SetValue(user.Password);
                checkBox1.Checked = true;
            }
            //textEdit1.Focus();              
        }
        
        private void comboBoxEdit1_TextChanged(object sender, EventArgs e)
        {
            var value = titleCombox1.Value();
            var strsql = "select LoginId from UserTable where LoginId like '%" + value + "%'";

            var dt = sql.ExecuteQuery(strsql).Tables[0];
            var count = dt.Rows.Count;
            if (count > 0)
                titleCombox1.comboBox1.Items.Clear();
            //comboBoxEdit1.ShowPopup();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (!Convert.IsDBNull(dt.Rows[i][0]))
                {
                    titleCombox1.comboBox1.Items.Add(dt.Rows[i][0].ToString());
                    //if (i == 0)
                    //    comboBoxEdit1.Text = dt.Rows[i][0].ToString();
                }
                    
            }

            titleCombox1.comboBox1.DroppedDown = true;
            titleCombox1.comboBox1.SelectionStart = titleCombox1.comboBox1.Text.Length;
        }

        #endregion
    }


    [Serializable]
    public class User
    {
        public string Id;

        public string Password;

        public List<string> Role;

        public User()
        { }

        public User(DataRow dr)
        {
            Id = dr["LoginId"].ToString();
            Password = dr["Pwd"].ToString();

            var role = dr["Role"].ToString().Replace('，', ',');
            Role = new List<string>();
            Role.AddRange(role.Split(','));
        }
    }
}
