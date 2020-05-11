using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ExpertLib.Dialogs;
using static ExpertLib.Utils.NativeMethods;

namespace ExpertLib.DataBase
{
    public class SqlHelper
    {    
        public static int ConnectTimeout = 2;//设置连接等待时间
        
        public string ConnectionString; //连接字符串  

        public static string DataBase;
        public static string ServerIP;
        public static string Password;
        public static string IniPath;

        public SqlHelper()
        {
            IniPath = AppDomain.CurrentDomain.BaseDirectory + "constring.ini";
            var server = new StringBuilder(1024);
            var pwd = new StringBuilder(1024);
            var dataBase = new StringBuilder(1024);
            GetPrivateProfileString("a", "server ", "server", server, 1024, IniPath);
            GetPrivateProfileString("a", "pwd ", "password", pwd, 1024, IniPath);
            GetPrivateProfileString("a", "database ", "data_base_name", dataBase, 1024, IniPath);

            ServerIP = server.ToString();
            DataBase = dataBase.ToString();
            Password = pwd.ToString();

            ConnectionString = $"server={ServerIP};database={DataBase};uid=sa;pwd={pwd};" +
                               "Pooling=true; max pool size=32765;min pool size=0;Asynchronous Processing=true;";

            if (!ConnectionString.EndsWith(";"))
                ConnectionString += ";";
            if (!ConnectionString.Contains("Connect Timeout"))
                ConnectionString += "Connect Timeout=" + ConnectTimeout;
        }

        public SqlHelper(string server,string password, string dataBase)
        {
            ServerIP = server;
            DataBase = dataBase;
            Password = password;

            ConnectionString = $"server={ServerIP};database={DataBase};uid=sa;pwd={Password};" +
                               "Pooling=true; max pool size=32765;min pool size=0;Asynchronous Processing=true;";

            if (!ConnectionString.EndsWith(";"))
                ConnectionString += ";";
            if (!ConnectionString.Contains("Connect Timeout"))
                ConnectionString += "Connect Timeout=" + ConnectTimeout;
        }

        public bool TestConnection(int timeout, out string errorInfo,
            string server = null, string pwd = null, string dataBase = null)
        {
            var result = true;
            string temp;
            if (server == null || pwd == null || dataBase == null)
                temp = ConnectionString.Replace("Connect Timeout=" + ConnectTimeout, "Connect Timeout=" + timeout);
            else
                temp = $"server={server};database={dataBase};uid=sa;pwd={pwd};" +
                       "Pooling=true;max pool size=1000;min pool size=0;Asynchronous Processing=true;";
            try
            {
                if (server == null || pwd == null || dataBase == null)
                {
                    using (var SqlConn = new SqlConnection())
                    {
                        SqlConn.ConnectionString = temp;
                        SqlConn.Open();
                        SqlConn.Close();
                        SqlConn.Dispose();
                    }
                }
                else
                {
                    using (var SqlConn = new SqlConnection())
                    {
                        
                        SqlConn.ConnectionString = temp;
                        SqlConn.Open();
                        SqlConn.Close();
                        SqlConn.Dispose();
                    }
                }

                errorInfo = "";
                Log.i("数据库连接成功");
            }
            catch (Exception ex)
            {
                errorInfo = ex.Message;
                Log.e($"连接失败 : {temp} \n {ex.Message}");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// DataTable导入数据库目标表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="targetTableName"></param>
        /// <param name="sourceCols">dt中列名</param>
        /// <param name="targetCols">目标表中的列名</param>
        public void DataTable2SqlServer(DataTable dt, string targetTableName, String[] sourceCols, String[] targetCols)
        {
            if (sourceCols.Length != targetCols.Length) return;
            if (sourceCols.Length == 0) return;

            using (var sqlRevdBulkCopy = new SqlBulkCopy(ConnectionString))
            {
                sqlRevdBulkCopy.DestinationTableName = targetTableName;
                sqlRevdBulkCopy.NotifyAfter = dt.Rows.Count;//有几行数据 

                for (int i = 0; i < sourceCols.Length; i++)
                    sqlRevdBulkCopy.ColumnMappings.Add(sourceCols[i], targetCols[i]);

                sqlRevdBulkCopy.WriteToServer(dt);//数据导入数据库 
                sqlRevdBulkCopy.Close();//关闭连接  
            }
        }

        /// <summary>
        /// DataTable导入数据库目标表，包括DataTable的所有列，除了ID
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="targetTableName">目标表名称</param>
        public void DataTable2SqlServer(DataTable dt, string targetTableName)
        {
            using (var sqlRevdBulkCopy = new SqlBulkCopy(ConnectionString))
            {
                sqlRevdBulkCopy.DestinationTableName = targetTableName;
                sqlRevdBulkCopy.NotifyAfter = dt.Rows.Count;//有几行数据 
                for (int i = 0; i < dt.Columns.Count; i++)//定义列
                {
                    string col_name = dt.Columns[i].ColumnName;
                    if (col_name.Equals("ID")) continue;
                    sqlRevdBulkCopy.ColumnMappings.Add(col_name, col_name);
                }

                sqlRevdBulkCopy.WriteToServer(dt);//数据导入数据库 
                sqlRevdBulkCopy.Close();//关闭连接  
            }
        }

        public DataSet ExecuteQuery(string strSql)
        {
            DataSet newDataSet;
            using (var SqlConn = new SqlConnection())
            {
                SqlConn.ConnectionString = ConnectionString;
                SqlConn.Open();

                var dataAdapter = new SqlDataAdapter(strSql, SqlConn);
                newDataSet = new DataSet();
                dataAdapter.Fill(newDataSet);

                SqlConn.Close();
                SqlConn.Dispose();
            }
            return newDataSet;
        }

        public DataSet ExecuteQuery(string strsql, SqlParameter[] parameterValues)
        {
            DataSet ds;
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                // Invoke RegionUpdate Procedure
                SqlCommand cmd = new SqlCommand(strsql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                foreach (SqlParameter p in parameterValues)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }

                var dataAdapter = new SqlDataAdapter { SelectCommand = cmd };
                ds = new DataSet();
                dataAdapter.Fill(ds);

                conn.Close();
                conn.Dispose();
            }
            return ds;
        }

        public DataSet ExecuteProcedureQuery(string strsql, SqlParameter[] parameterValues)
        {
            SqlDataAdapter dataAdapter;
            DataSet ds;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                // Invoke RegionUpdate Procedure
                SqlCommand cmd = new SqlCommand(strsql, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                foreach (SqlParameter p in parameterValues)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }
                dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = cmd;
                ds = new DataSet();
                dataAdapter.Fill(ds);

                conn.Close();
                conn.Dispose();
            }
            return ds;
        }

        #region 执行没有返回的数据库操作

        /// <summary>
        /// 执行没有返回的数据库操作
        /// </summary>
        /// <param name="strsql"></param>
        /// <param name="parameterValues"></param>
        public void ExecuteNonQuery(string strsql, SqlParameter[] parameterValues)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                // Invoke RegionUpdate Procedure
                var cmd = new SqlCommand(strsql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 0
                };
                foreach (var p in parameterValues)
                {
                    if (p.Direction == ParameterDirection.InputOutput && p.Value == null)
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 执行没有返回的数据库操作
        /// </summary>
        /// <param name="strSql">语句</param>
        public void ExecuteNonQuery(string strSql)
        {
            using (var SqlConn = new SqlConnection())
            {
                SqlConn.ConnectionString = ConnectionString;
                SqlConn.Open();
                var Comm = new SqlCommand(strSql, SqlConn);


                if (SqlConn.State == 0)
                    SqlConn.Open();
                Comm.ExecuteNonQuery();
                Comm.Dispose();

                SqlConn.Close();
                SqlConn.Dispose();
            }
        }

        #endregion

        #region 修改表的结构

        public void AddColumn(String table, String column, String type)
        {
            String strsql = "if (select name from sys.syscolumns where name='" + column + "' and id=OBJECT_ID('" + table + "')) is null begin";
            strsql += " \n alter table " + table + " ADD  " + column + " " + type;
            strsql += " \n end";
            ExecuteNonQuery(strsql);
        }

        #endregion

        #region 获得数据

        public List<string> GetStringList(string strsql)
        {
            var dt = ExecuteQuery(strsql).Tables[0];

            if (dt.Rows.Count == 0)
                return new List<string>();

            var list = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row[0].ToString());
            }

            return list;

        }

        public int GetExpr1(string strsql, int defaultValue)
        {
            DataTable dt = ExecuteQuery(strsql).Tables[0];

            if (dt.Rows.Count == 0) return defaultValue;

            return int.TryParse(dt.Rows[0][0].ToString(), out var result) ? result : defaultValue;
        }

        public double GetExpr1(string strsql, double defaultValue)
        {
            var dt = ExecuteQuery(strsql).Tables[0];

            if (dt.Rows.Count == 0) return defaultValue;

            return double.TryParse(dt.Rows[0][0].ToString(), out var result) ? result : defaultValue;
        }

        public string GetExpr1(string strsql, string defaultValue)
        {
            DataTable dt = ExecuteQuery(strsql).Tables[0];

            if (dt.Rows.Count == 0) return defaultValue;
            else return dt.Rows[0][0].ToString();

        }

        public byte[] GetExpr1(string strsql)
        {
            DataTable dt = ExecuteQuery(strsql).Tables[0];

            if (dt.Rows.Count == 0) return null;
            if (dt.Columns.Contains("Expre1"))
                return (byte[])dt.Rows[0]["Expr1"];
            else
                return (byte[])dt.Rows[0][0];
        }

        #endregion

        
    }

}
