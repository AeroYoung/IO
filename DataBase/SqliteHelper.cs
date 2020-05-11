using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using ExpertLib.Dialogs;

namespace ExpertLib.DataBase
{
    public class SqliteHepler
    {
        public string ConnectionString; //连接字符串  

        public string DbPath;

        public string Password;

        public SqliteHepler(string db, string pwd = null)
        {
            DbPath = db;
            Password = pwd;
            ConnectionString = string.IsNullOrWhiteSpace(pwd)
                ? $"DataSource={DbPath}"
                : $"DataSource={DbPath};Pwd={Password}";
        }

        public bool TestConnection(out string errorInfo)
        {
            var result = true;
            errorInfo = "";

            if (!File.Exists(DbPath))
            {
                errorInfo = $"数据库文件不存在: {DbPath}";
                Log.e("数据库文件不存在: {DbPath}");
                return false;
            }

            try
            {
                using (var SqlConn = new SQLiteConnection())
                {
                    SqlConn.ConnectionString = ConnectionString;
                    SqlConn.Open();
                    SqlConn.Close();
                    SqlConn.Dispose();
                }
            }
            catch (Exception ex)
            {
                errorInfo = ex.ToString();
                Log.e($"连接失败:{DbPath} {ex.Message}");
                result = false;
            }

            return result;
        }

        public DataSet ExecuteQuery(string strSql)
        {
            DataSet newDataSet;
            using (var SqlConn = new SQLiteConnection())
            {
                SqlConn.ConnectionString = ConnectionString;
                SqlConn.Open();

                var dataAdapter = new SQLiteDataAdapter(strSql, SqlConn);
                newDataSet = new DataSet();
                dataAdapter.Fill(newDataSet);

                SqlConn.Close();
                SqlConn.Dispose();
            }

            return newDataSet;
        }

        public DataSet ExecuteQuery(string strsql, SQLiteParameter[] parameterValues)
        {
            DataSet ds;
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                // Invoke RegionUpdate Procedure
                var cmd = new SQLiteCommand(strsql, conn) { CommandType = CommandType.Text, CommandTimeout = 0 };
                foreach (var p in parameterValues)
                {
                    if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }

                var dataAdapter = new SQLiteDataAdapter { SelectCommand = cmd };
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
        /// <param name="strSql">语句</param>
        public void ExecuteNonQuery(string strSql)
        {
            using (var SqlConn = new SQLiteConnection())
            {
                SqlConn.ConnectionString = ConnectionString;
                SqlConn.Open();
                var Comm = new SQLiteCommand(strSql, SqlConn);


                if (SqlConn.State == 0)
                    SqlConn.Open();
                Comm.ExecuteNonQuery();
                Comm.Dispose();

                SqlConn.Close();
                SqlConn.Dispose();
            }
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
