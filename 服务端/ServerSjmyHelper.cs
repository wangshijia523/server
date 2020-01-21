// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerSjmyHelper.cs" company="nd@231216">
//   
// </copyright>
// <summary>
//   涉及第二轮要用到函数使用static方法，如果重新new数据将丢失
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace 服务端
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    using ApkReader;

    using CCWin.SkinClass;

    public class ServerSjmyHelper
    {

        /// <summary>
        /// 分控在线数量
        /// </summary>
        public int subControl { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public int Mtid { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Gnpy { get; set; }

        /// <summary>
        /// 服务器对应id
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// 任务使用库名(环境所在文件夹名)
        /// </summary>
        public string AppSrc { get; set; }

        /// <summary>
        /// 流程类型
        /// </summary>
        public string ScriptType { get; set; }

        /// <summary>
        /// 发起人工号
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 卸载标志
        /// </summary>
        public string Uninstall { get; set; }

        /// <summary>
        /// APK包名
        /// </summary>
        public string ApkPackage { get; set; }

        /// <summary>
        /// APK文件名称
        /// </summary>
        public string ApkName { get; set; }

        /// <summary>
        /// 运行脚本总数
        /// </summary>
        public int ALLNum { get; set; }


        /// <summary>
        /// 获取当前时间，格式"yyyy-MM-dd HH:mm:ss    "
        /// </summary>
        /// <returns></returns>
        public static string GetTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    ");
        }


        /// <summary>通过脚本总数</summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int PassNum()
        {

            return (int)SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select count(*) from MobileScriptModule where PacketChannel = '{Gnpy}' and uploadflog = 1",
                new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0];
        }

        /// <summary>不通过脚本总数</summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int UnpassNum()
        {

            return (int)SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select count(*) from MobileScriptModule where PacketChannel = '{Gnpy}' and uploadflog = 0",
                new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0];
        }

        /// <summary>失败脚本总数</summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int FailedNum()
        {
            return (int)SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select count(*) from MobileScriptModule where PacketChannel = '{Gnpy}' and uploadflog = 2",
                new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0];
        }

        /// <summary>
        /// 获取游服版本
        /// </summary>
        /// <returns></returns>
        public string GetSeversion()
        {
            var number = 0;

            switch (Gnpy)
            {
                case "sjmy":
                    number = 44;
                    break;
                case "xsjmy":
                    number = 45;
                    break;
                case "myht":
                    number = 58;
                    break;
            }

            return SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select ServerSion from GameServer where SID = {number}",
                new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0].ToString();
        }

        /// <summary>
        /// 获取npc版本号
        /// </summary>
        /// <returns></returns>
        public string GetNpcVer()
        {
            var tempSid = 0;
            switch (Gnpy)
            {
                case "sjmy":
                    tempSid = 17;
                    break;
                case "xsjmy":
                    tempSid = 29;
                    break;
                case "myht":
                    tempSid = 59;
                    break;
            }

            return SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select ServerSion from GameServer where SID = {tempSid}",
                new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0].ToString();
        }


        public string ReportUrl()
        {
            return SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select ReportUrl from MobileTaskTB  where MTID = {Mtid} ",
                new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0].ToString();
        }

        /// <summary>
        /// APK版本号
        /// </summary>
        public string ApkVersion { get; set; }

        /// <summary>结果上传后记录TIME</summary>
        public string RecordTime { private get; set; }

        /// <summary>服务端操作完成后记录TIME</summary>
        public string StartTime { private get; set; }

        /// <summary>结果上传前记录TIME </summary>
        public string EndTime { private get; set; }

        /// <summary>
        /// 模拟器登录账号，格式（xxxxxx，xxx）
        /// </summary>
        private string channleResult { get; set; }

        public string Database = "QAGameAuto";

        public string Site = @"\\192.168.45.25";

        public string Account = "administrator";

        public string Password = "www.99.com.";

        public string ApkSite = @"\\192.168.45.25\upload\";

        public string ProjectSite;

        #region 解析apk
        /// <summary>
        /// 解析Apk
        /// </summary>
        public void ApkAnalysis()
        {
            try
            {
                var reader = new ApkReader();
                var ApkPath = $@"{ApkSite}{Gnpy}\{AppSrc}";
                var Apk = string.Empty;
                var root = new DirectoryInfo(ApkPath);
                foreach (var t in root.GetFiles())
                {
                    if (Path.GetExtension(t.Name) == ".apk")
                    {
                        Apk = t.FullName;
                        ApkName = t.Name;
                    }
                }

                var info = reader.Read(Apk);
                ApkPackage = info.PackageName;
                ApkVersion = info.VersionName;

                DirFile.Copy($@"{ApkSite}{Gnpy}\type1.png", $@"{ApkPath}\{Gnpy}.png");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region 返回sql脚本路径
        /// <summary>
        /// 返回45.25上sql脚本路径
        /// </summary>
        /// <returns></returns>
        public string GetSqlPath()
        {
            var Path = $@"{ApkSite}{Gnpy}\{AppSrc}";
            var root = new DirectoryInfo(Path);
            foreach (var t in root.GetFiles())
            {
                if (System.IO.Path.GetExtension(t.Name) == ".sql")
                {
                    return t.FullName;
                }
            }

            return string.Empty;
        }


        #endregion

        #region 任务检查
        /// <summary>
        /// 检查当前是否有任务
        /// </summary>
        public bool TaskCheck
        {
            get
            {

                try
                {
                    // 查询是否有正在执行的任务（ScriptFlog = 1 or 3）
                    var being = SqlHelper.ExecuteDataSet(
                        CommandType.Text,
                        "select s1.MTID,s1.GNPY,s1.SID,s1.AppSrc,s1.ScriptType,s1.UID from MobileTaskTB s1,MobileTaskTempTB s2 where (s1.ScriptFlog=1 or s1.ScriptFlog=3)  and s1.MTID=s2.MTID",
                        new List<SqlParameter>().ToArray()).Tables[0];
                    if (being.Rows.Count == 0)
                    {
                        var result = SqlHelper.ExecuteDataSet(
                            CommandType.Text,
                            "select MTID,GNPY,SID,AppSrc,ScriptType,UID,Uninstall from MobileTaskTB  where ScriptFlog=0 order by MTID",
                            new List<SqlParameter>().ToArray()).Tables[0];
                        if (result.Rows.Count != 0)
                        {

                            Mtid = result.Rows[0][0].ToInt32();
                            Gnpy = result.Rows[0][1].ToString();
                            Sid = result.Rows[0][2].ToString();
                            AppSrc = result.Rows[0][3].ToString();
                            ScriptType = result.Rows[0][4].ToString();
                            Uid = result.Rows[0][5].ToString();
                            Uninstall = result.Rows[0][6].ToString();
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("当前无法连接数据库，请检查数据库后点击确认", "警告");
                }

                return false;
            }
        }
        #endregion

        #region 复制初始库
        /// <summary>179复制需要运行的初始库</summary>
        /// <param name="gnpy">项目名称</param>
        /// <param name="AppSrc">时间戳(文件夹名称)</param>
        /// <returns></returns>
        public string CopyInitial(string gnpy, string AppSrc)
        {
            const string path179 = @"\\192.168.9.179\data\";
            try
            {
                switch (gnpy)
                {
                    case "sjmy":
                        if (DirFile.IsExistDirectory(path179 + "sjmy_autotest_init_ndsdk"))
                        {
                            DirFile.CopyFolder(path179 + "sjmy_autotest_init_ndsdk", path179 + "sjmy_again" + AppSrc);
                            DirFile.CopyFolder(path179 + "sjmy_autotest_init_ndsdk", path179 + "sjmy_autotest" + AppSrc);
                        }
                        else
                        {
                            return "sjmy_autotest_init_ndsdk不存在";
                        }

                        break;

                    case "xsjmy":
                        if (DirFile.IsExistDirectory(path179 + "sjmy_autotest_init_xsj"))
                        {
                            DirFile.CopyFolder(path179 + "sjmy_autotest_init_xsj", path179 + "sjmy_again" + AppSrc);
                            DirFile.CopyFolder(path179 + "sjmy_autotest_init_xsj", path179 + "sjmy_autotest" + AppSrc);
                        }
                        else
                        {
                            return "sjmy_autotest_init_xsj不存在";
                        }

                        break;

                    case "myht":

                        if (DirFile.IsExistDirectory(path179 + "my_bak"))
                        {
                            DirFile.CopyFolder(path179 + "my_bak", path179 + "myht_again" + AppSrc);
                            DirFile.CopyFolder(path179 + "my_bak", path179 + "myht_autotest" + AppSrc);
                        }
                        else
                        {
                            return "my_bak不存在";
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return "初始库复制完成";
        }

        #endregion

        #region 根据项目复制换库脚本文件夹

        public string CopyServercase(string gnpy)
        {
            const string path105 = @"D:\developer\qatest\data\script\";
            try
            {
                switch (gnpy)
                {
                    case "sjmy":
                        DirFile.CopyFolder(path105 + "servercase_ndsdk", path105 + "servercase");
                        break;
                    case "xsjmy":
                        DirFile.CopyFolder(path105 + "servercase_xsj", path105 + "servercase");
                        break;
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "换库脚本文件夹复制完成";
        }

        #endregion

        #region cmd运行python脚本
        /// <summary>
        /// 运行python脚本
        /// </summary>
        /// <param name="path">脚本绝对路径</param>
        /// <param name="parameter1">脚本启动参数1</param>
        /// <param name="parameter2">脚本启动参数2</param>
        public void RunCmdPython(string path, string parameter1 = "", string parameter2 = "")
        {
            var runcmd = new RunCmd();
            runcmd.Exe($@"python {path} {parameter1} {parameter2}");
        }

        #endregion

        #region 重置数据库第一轮运行脚本
        /// <summary>
        /// 重置数据库第一轮运行脚本
        /// </summary>
        /// <returns>返回受到影响到行数</returns>
        public int ResetOne()
        {
            SqlHelper.ExecteNonQuery(
                CommandType.Text,
                $"update MobileScriptModule set flog=null,mobile=null,uploadflog=null,ip=null",
                new List<SqlParameter>().ToArray());

            // simple=1表示简单流程要跑

            switch (ScriptType)
            {
                case "999":
                    ALLNum = SqlHelper.ExecteNonQuery(
                        CommandType.Text,
                        $"update MobileScriptModule set flog={Mtid},mobile=null,uploadflog=null,ip=null where PacketChannel= '{Gnpy}'",
                        new List<SqlParameter>().ToArray());
                    break;
                case "998":
                    ALLNum = SqlHelper.ExecteNonQuery(
                        CommandType.Text,
                        $"update MobileScriptModule set flog={Mtid},mobile=null,uploadflog=null,ip=null where simple=1 and PacketChannel= '{Gnpy}'",
                        new List<SqlParameter>().ToArray());
                    break;
                case "997":
                    // 需要读取txt
                    var FilePath = $@"{ApkSite}{Gnpy}\{AppSrc}\script.txt";
                    ALLNum = SqlHelper.ExecteNonQuery(
                        CommandType.Text,
                        $"update MobileScriptModule set flog={Mtid},mobile=null,uploadflog=null,ip=null where SID in ({File.ReadAllText(FilePath)}) and PacketChannel= '{Gnpy}'",
                        new List<SqlParameter>().ToArray());
                    break;
            }

            return ALLNum;
        }

        #endregion

        #region 重置数据库第二轮运行脚本
        /// <summary>
        /// 重置数据库第二轮运行脚本
        /// </summary>
        /// <returns>返回受到影响到行数</returns>
        public int ResetTwo()
        {
            var being = SqlHelper.ExecteNonQuery(
                CommandType.Text,
                $"update MobileScriptModule set flog={Mtid},mobile=null,uploadflog=null,ip=null where uploadflog !=1 and PacketChannel= '{Gnpy}'",
                new List<SqlParameter>().ToArray());

            return being;
        }

        #endregion

        #region 修改账号快速登录服务器
        /// <summary>使用MySqlData批量更新数据</summary>
        /// <param name="severID">The sever ID.</param>
        /// <returns></returns>
        public int sqlUpdate(string severID)
        {
            MySqlHelper.InitConnectStr("sjmy_config_nd");
            return MySqlHelper.ExecuteSql(
                $"update cq_login_record set server_id ='{severID}' where username in {channleResult}");

        }
        #endregion

        #region 获取模拟器账号
        /// <summary>
        /// 通过xml获取的accountid 获得模拟器登录账号
        /// </summary>
        public void getChannel()
        {
            var account = "(";
            var xd = new XmlDocument();
            //var sArray = ApkVersion.Split('.');
            //xd.Load($@"{ProjectSite}script\ndsdk\{sArray[0]}.{sArray[1]}.X\environinit\accountid.xml");
            xd.Load($@"Y:\sjmy\data\script\AllVersion\6.7.X\environinit\accountid.xml");
            var root = xd.DocumentElement;
            if (root != null)
                foreach (XmlNode xmlNode in root.ChildNodes)
                {
                    foreach (XmlNode xmlElement in xmlNode.ChildNodes)
                    {
                        account = account + xmlElement.InnerText + ",";
                    }
                }
            account = account.Substring(0, account.Length - 1) + ")";

            MySqlHelper.InitConnectStr("sjmy_autotest_init_ndsdk");
            var channle = MySqlHelper.GetDataSet(
                $"select channel_account from cq_user where account_id in {account}");
            channleResult = "(";

            for (var i = 0; i < channle.Tables["Table1"].Rows.Count; i++)
            {
                channleResult = channleResult + channle.Tables["Table1"].Rows[i].ItemArray[0].ToString().Remove(0, 5) + ",";
            }

            channleResult = channleResult + "883181957,938704954,934224165,934246512)";

            Console.WriteLine(channleResult);
        }

        #endregion

        #region 刷入初始库

        public bool intoSQL(string filePath)
        {
            if (!DirFile.IsExistFile(filePath))
            {
                return true;
            }

            if (Gnpy == "myht")
            {
                return MySqlHelper.IntoSQL(filePath, "myht_again" + AppSrc) && MySqlHelper.IntoSQL(filePath, "myht_autotest" + AppSrc);
            }

            return MySqlHelper.IntoSQL(filePath, "sjmy_again" + AppSrc) && MySqlHelper.IntoSQL(filePath, "sjmy_autotest" + AppSrc);
        }

        #endregion

        #region 获取未成功的脚本

        private DataTable GetDataSet()
        {
            return SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select ScriptName, mobile, ip from MobileScriptModule where uploadflog != 1 and  PacketChannel = '{Gnpy}'",
                new List<SqlParameter>().ToArray()).Tables[0];
        }

        #endregion

        #region 解析未成功脚本路径

        /// <summary>
        /// 解析未成功脚本路径
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            var dataTable = GetDataSet();
            var path = string.Empty;
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                path =
                    $"{dataTable.Rows[i][0]}:{dataTable.Rows[i][2].ToString()}@{dataTable.Rows[i][1].ToString()}{dataTable.Rows[i][0].ToString().Split('-')[0]}测试总报告,{path}";
            }
            if (path == string.Empty)
            {
                path = "无未通过报告";
            }

            return path;

        }


        #endregion

        #region 插入任务结果

        /// <summary>
        /// 插入任务结果
        /// </summary>
        public void InsertReult()
        {
            SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"insert into MobileTestResult(AllNum,PassNum,UnpassNum,FailedNum,TestRsult,ApkFileName,GNPY,MTID,StartTime,EndTime,ClientVersion,ResourcesVersion,SpeGameSever,RecordTime) Values({ALLNum},{PassNum()},{UnpassNum()},{FailedNum()},0,'{ApkName}','{Gnpy}','{Mtid}','{StartTime}','{EndTime}','{ApkVersion}','full','{GetSeversion()}','{RecordTime}')",
                new List<SqlParameter>().ToArray());
        }

        #endregion

        #region 修改ScriptFlog

        /// <summary>
        /// 设置当前任务状态
        /// </summary>
        /// <param name="number"></param>
        public void SetScriptFlog(int number)
        {
            SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"update MobileTaskTB set ScriptFlog={number} where MTID={Mtid}",
                new List<SqlParameter>().ToArray());
        }


        #endregion

        #region 插入失败结果表
        /// <summary>插入失败脚本语句</summary>
        /// <param name="ChName"></param>
        /// <param name="EnName"></param>
        /// <param name="MobileNo">The Mobile No.</param>
        /// <param name="Ip">The Ip.</param>
        private void InsertFailedSql(string ChName, string EnName, string MobileNo, string Ip)
        {
            var test =
                $"insert into MobileTestFailed(ChinedeName,EngLishName,MTID,MID,Ip) Values('{ChName}','{EnName}',{Mtid},'{MobileNo}','{Ip}')";
            SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"insert into MobileTestFailed(ChinedeName,EngLishName,MTID,MID,Ip) Values('{ChName}','{EnName}',{Mtid},'{MobileNo}','{Ip}')",
                new List<SqlParameter>().ToArray());
        }

        /// <summary>插入失败脚本</summary>
        public void InsertFailed()
        {

            var dateTable = SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select ScriptName, mobile, ip from MobileScriptModule where uploadflog =2 and PacketChannel ='{Gnpy}'",
                new List<SqlParameter>().ToArray()).Tables[0];

            for (var i = 0; i < dateTable.Rows.Count; i++)2
            {
                var MID = SqlHelper.ExecuteDataSet(
                    CommandType.Text,
                    $"select MID from MobileDevices where MobileNo = '{dateTable.Rows[i][1]}'",
                    new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0].ToString();

                InsertFailedSql(dateTable.Rows[i][0].ToString().Split('-')[0], dateTable.Rows[i][0].ToString().Split('-')[1], MID, dateTable.Rows[i][2].ToString());
            }
        }



        #endregion

        #region 插入不通过结果表

        /// <summary>插入不通过脚本语句</summary>
        /// <param name="MID">The MID.</param>
        /// <param name="ChName"></param>
        /// <param name="EnName"></param>
        /// <param name="NDaccount"></param>
        /// <param name="PCaccount"></param>
        /// <param name="Ip">The Ip.</param>
        private void InsertMobileUnpassSql(string MID, string ChName, string EnName, string NDaccount, string PCaccount, string Ip)
        {
            SqlHelper.ExecuteDataSet(
                   CommandType.Text,
                   $"insert into MobileUnpass(MID,MTID,ChineseName,EnglishName,NDsdkAccount,PcsimulatorAccount,Ip) Values('{MID}',{Mtid},'{ChName}','{EnName}','{NDaccount}','{PCaccount}','{Ip}')",
                   new List<SqlParameter>().ToArray());
        }

        /// <summary>
        /// 插入不通过脚本
        /// </summary>
        public void InsertMobileUnpass()
        {

            var dateTable = SqlHelper.ExecuteDataSet(
                CommandType.Text,
                $"select ScriptName,mobile,ip from MobileScriptModule where uploadflog =0 and PacketChannel ='{Gnpy}'",
                new List<SqlParameter>().ToArray()).Tables[0];

            var xd = new XmlDocument();
            xd.Load($@"D:\developer\{Gnpy}\xmlconfig\UserInfo.xml");

            var xml = new XmlDocument();
            xml.Load($@"D:\developer\{Gnpy}\xmlconfig\accountid.xml");

            for (var i = 0; i < dateTable.Rows.Count; i++)
            {
                var MID = SqlHelper.ExecuteDataSet(
                    CommandType.Text,
                    $"select MID from MobileDevices where MobileNo = '{dateTable.Rows[i][1]}'",
                    new List<SqlParameter>().ToArray()).Tables[0].Rows[0][0].ToString();

                var node = xd.SelectSingleNode($"/root/{dateTable.Rows[i][0].ToString().Split('-')[1]}");
                if (node != null)
                {
                    //if (Gnpy == "sjmy")
                    //{
                    //    var nodeAcc = xml.SelectSingleNode($"/script/{node.InnerText}");
                    //    if (nodeAcc != null)
                    //    {
                    //        var pcAccount = string.Empty;
                    //        MySqlHelper.InitConnectStr(@"sjmy_autotest_init_ndsdk");
                    //        var channle = MySqlHelper.GetDataSet(
                    //            $"select channel_account from cq_user where account_id = {nodeAcc.InnerText}");

                    //        if (channle.Tables["Table1"].Rows.Count != 0)
                    //        {
                    //            pcAccount = channle.Tables["Table1"].Rows[0].ItemArray[0].ToString().Remove(0, 5);
                    //        }

                    //        InsertMobileUnpassSql(
                    //            MID,
                    //            dateTable.Rows[i][0].ToString().Split('-')[0],
                    //            dateTable.Rows[i][0].ToString().Split('-')[1],
                    //            node.InnerText,
                    //            pcAccount,
                    //            dateTable.Rows[i][2].ToString());
                    //    }
                    //}
                    InsertMobileUnpassSql(
                            MID,
                            dateTable.Rows[i][0].ToString().Split('-')[0],
                            dateTable.Rows[i][0].ToString().Split('-')[1],
                            node.InnerText,
                            node.InnerText,
                            dateTable.Rows[i][2].ToString());
                }
            }
        }


        #endregion

        #region 清除交易服数据
        public void DelDeal()
        {
            MySqlHelper.InitConnectStr("sjmy_TradeServer");
            MySqlHelper.GetDataSet("delete from cq_far_trade");
        }

        #endregion
    }
}





