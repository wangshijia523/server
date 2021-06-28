﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="MySqlHelper.cs">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace 服务端
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Windows.Forms;

    using MySql.Data.MySqlClient;

    public class MySqlHelper
    {
        // 从配置文件读取连接字符串
        private static string connstr = string.Empty;
        #region 初始化连接字符串

        /// <summary>
        /// 初始化连接字符串
        /// </summary>
        /// <param name="passWord"></param>
        /// <param name="connStr"></param>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static void InitConnectStr(string host, string userName, string passWord, string connStr)
        {
            connstr = $@"Host={host};UserName={userName};Password={passWord};Database={connStr};Port=3306;CharSet=gbk";
            //connstr = @"Host=192.168.9.179;UserName=htl;Password=123456;Database=" + connStr + @";Port=3306;CharSet=gbk";
        }
        #endregion

        #region 执行查询语句，返回MySqlDataReader
        /// <summary>
        /// 执行查询语句，返回MySqlDataReader
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string sqlString)
        {
            var connection = new MySqlConnection(connstr);
            var cmd = new MySqlCommand(sqlString, connection);
            MySqlDataReader myReader = null;
            try
            {
                connection.Open();
                myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                connection.Close();
                throw new Exception(e.Message);
            }
            finally
            {
                if (myReader == null)
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        #endregion

        #region 执行带参数的查询语句，返回MySqlDataReader
        /// <summary>
        /// 执行带参数的查询语句，返回MySqlDataReader
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string sqlString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(connstr);
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                connection.Close();
                throw new Exception(e.Message);
            }
            finally
            {
                if (myReader == null)
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        #endregion

        #region 执行sql语句,返回执行行数
        /// <summary>
        /// 执行sql语句,返回执行行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        conn.Close();
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
        }
        #endregion

        #region 执行带参数的sql语句，并返回执行行数
        /// <summary>
        /// 执行带参数的sql语句，并返回执行行数
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sqlString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region 执行查询语句，返回DataSet
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sql, conn);
                    // DataAdapter.Fill(ds);
                    dataAdapter.Fill(ds, "Table1");
                }
                catch /*(Exception ex)*/
                {
                    MessageBox.Show(@"连接出错，请检查库名是否正确", @"警告", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    /*throw ex;*/
                }
                finally
                {
                    conn.Close();
                }
                return ds;
            }
        }
        #endregion

        #region 执行带参数的查询语句，返回DataSet
        /// <summary>
        /// 执行带参数的查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sqlString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }
        #endregion

        #region 执行带参数的sql语句，并返回object
        /// <summary>
        /// 执行带参数的sql语句，并返回object
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object GetSingle(string sqlString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 执行存储过程,返回数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedureForDataSet(string storedProcName, IDataParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connstr))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                MySqlDataAdapter sqlDA = new MySqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName,
            IDataParameter[] parameters)
        {
            MySqlCommand command = new MySqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (MySqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        #region 装载MySqlCommand对象
        /// <summary>
        /// 装载MySqlCommand对象
        /// </summary>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText,
            MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text; // cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
        #endregion

        #region 手机魔域刷库专用

        /// <summary>
        /// 手机魔域更新脚本专用
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>成功或者失败</returns>
        public static bool intoSql(string sql)
        {
            var conn = new MySqlConnection(connstr);
            try
            {
                conn.Open();
                var script = new MySqlScript(conn) { Query = sql, Delimiter = "//" };
                script.Execute();
                //script.Delimiter = "\r\n";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
            }

        }

        public static bool IntoSQL(string FailPah, string host, string userName, string passWord, string connStr)
        {
            var strMysqlFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Tools\\mysql.exe";
            var result = ScriptBrush(FailPah, strMysqlFilePath, host, userName, passWord, "3306", connStr);
			
            if (result == string.Empty)
            {
                return true;
            }

            MessageBox.Show(result);
            return false;



        }

        private static string ScriptBrush(string strScirptFilePah, string strMysqlFilePath, string strIp, string strUserName, string strPassWord, string strPort, string strDataBaseName)
        {
            try
            {
                strDataBaseName = "\"" + strDataBaseName + "\"";
                strScirptFilePah = "\"" + strScirptFilePah + "\"";
                strMysqlFilePath = "\"" + strMysqlFilePath + "\"";
                var str = strMysqlFilePath + " -h" + strIp + " -u" + strUserName + " -p" + strPassWord + " -P" + strPort + " " + strDataBaseName + "<" + strScirptFilePah;
                var process = new Process
                                  {
                                      StartInfo =
                                          {
                                              FileName = "cmd.exe",
                                              RedirectStandardInput = true,
                                              RedirectStandardOutput = true,
                                              RedirectStandardError = true,
                                              CreateNoWindow = true,
                                              UseShellExecute = false
                                          }
                                  };
                process.Start();
                process.StandardInput.WriteLine(str);
                process.StandardInput.WriteLine("exit");
                while (!process.HasExited)
                    process.WaitForExit(100);
                var end = process.StandardError.ReadToEnd();
                process.StandardError.Close();
                return end;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region 备份交易服数据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="dataBaseName"></param>
        /// <param name="tableName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string BackupsTrade(string host, string userName, string passWord, string dataBaseName, string tableName, string filePath)
        {
            var str =
                $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Tools\\mysqldump.exe -h{host} -u{userName} -p{passWord} -P3306 -y --no-tablespaces --compact --skip-extended-insert --default-character-set=gb2312 -t {dataBaseName} {tableName}>{filePath}";

            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                process.StandardInput.WriteLine(str);
                process.StandardInput.WriteLine("exit");
                while (!process.HasExited)
                    process.WaitForExit(100);
                var end = process.StandardError.ReadToEnd();
                process.StandardError.Close();
                return end=="" ? "交易服表数据备份成功" : end;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

    }
}



