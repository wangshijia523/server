// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form1.cs" company="nd@231216">
//   
// </copyright>
// <summary>
//   Defines the Form1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace 服务端
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Windows.Forms;
    using CCWin;

    /// <inheritdoc />
    /// <summary>TODO The form 1.</summary>
    public partial class Form1 : Skin_VS
    {
        public Form1()
        {
            InitializeComponent();

        }

        private readonly ServerSjmyHelper _sjmyHelper = new ServerSjmyHelper();

        private readonly INIFile _iniFile =
            new INIFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"ini\config.ini");

        private Thread SelectThread { get; set; } // 查库线程

        #region 查询线程
        /// <summary>
        /// 查询当前是否有需要执行的任务
        /// </summary>
        private void SelectListen()
        {
            SelectThread = new Thread(DetermineSend); // 这个线程仅仅用来循环查询数据库任务  
            SelectThread.Start(); // 启动这个线程方法             
        }


        #endregion

        #region 查询到符合结果
        /// <summary>
        /// 检测到任务后执行----
        /// </summary>
        private void DetermineSend()
        {
            while (true)
            {
                if (_sjmyHelper.TaskCheck)
                {
                    ShowMsg("查询到任务！！！！");

                    _sjmyHelper.ProjectSite = $@"D:\developer\{_sjmyHelper.Gnpy}";

                    // 复制初始库，也要通过GNPY区分初始库
                    ShowMsg("开始复制初始库");
                    ShowMsg(_sjmyHelper.CopyInitial(_sjmyHelper.Gnpy, _sjmyHelper.AppSrc));

                    // 刷库操作
                    switch (_sjmyHelper.Gnpy)
                    {
                        case "sjmy":

                            if (_sjmyHelper.GetSqlPath() != string.Empty)
                            {
                                if (!_sjmyHelper.intoSQL($@"{_sjmyHelper.GetSqlPath()}"))
                                {
                                    ShowMsg("刷库异常,终止任务。");
                                    Process.Start(
                                        $@"{_sjmyHelper.ProjectSite}lib\99uMessage.exe",
                                        $@"{_sjmyHelper.Uid} 测试完成--推送IP:192.168.64.105，刷库错误，任务已结束");
                                    _sjmyHelper.SetScriptFlog(2);
                                    Restart();
                                    return;
                                }
                            }

                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\monthcardinit.sql");
                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\rechargeweal.sql");
                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\updateSql_{_sjmyHelper.Gnpy}.sql");
                            ShowMsg("已完成刷库操作");
                            break;

                        case "xsjmy":

                            if (_sjmyHelper.GetSqlPath() != string.Empty)
                            {
                                if (!_sjmyHelper.intoSQL($@"{_sjmyHelper.GetSqlPath()}"))
                                {
                                    ShowMsg("刷库异常,终止任务。");
                                    Process.Start(
                                        $@"{_sjmyHelper.ProjectSite}lib\99uMessage.exe",
                                        $@"{_sjmyHelper.Uid} 测试完成--推送IP:192.168.64.105，刷库错误，任务已结束");
                                    _sjmyHelper.SetScriptFlog(2);
                                    Restart();
                                    return;
                                }
                            }
                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\monthcardinit.sql");
                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\rechargeweal.sql");
                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\updateSql_{_sjmyHelper.Gnpy}.sql");
                            ShowMsg("已完成刷库操作");
                            break;

                        case "myht":

                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\userinfo.sql");
                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\mynfkuaice.sql");
                            if (_sjmyHelper.GetSqlPath() != string.Empty)
                            {
                                if (!_sjmyHelper.intoSQL($@"{_sjmyHelper.GetSqlPath()}"))
                                {
                                    ShowMsg("刷库异常,终止任务。");
                                    Process.Start(
                                        $@"{_sjmyHelper.ProjectSite}lib\99uMessage.exe",
                                        $@"{_sjmyHelper.Uid} 测试完成--推送IP:192.168.64.105，刷库错误，任务已结束");
                                    _sjmyHelper.SetScriptFlog(2);
                                    Restart();
                                    return;
                                }
                            }
                            _sjmyHelper.intoSQL($@"{_sjmyHelper.ProjectSite}\main\updateSql_myht.sql");
                            break;
                    }

                    // apk解析                   
                    _sjmyHelper.ApkAnalysis();
                    ShowMsg($"apk解析完成:ApkPackage = {_sjmyHelper.ApkPackage},ApkVersion = {_sjmyHelper.ApkVersion}");

                    // 运行换库脚本
                    ShowMsg("开始执行换库操作");

                    _sjmyHelper.RunCmdPython($@"{_sjmyHelper.ProjectSite}\data\script\servercase\sqlconfig.py", _sjmyHelper.AppSrc);
                    _sjmyHelper.RunCmdPython($@"{_sjmyHelper.ProjectSite}\data\script\servercase\server.py", _sjmyHelper.GetSeversion(), _sjmyHelper.GetNpcVer());
                    ShowMsg("换库操作完成，服务器若开启失败请检查pythonSyslog");

                    // 清除交易服数据
                    if (_iniFile.IniReadValue("server", "TradingDel") == "1" && _sjmyHelper.Gnpy == "sjmy")
                    {
                        _sjmyHelper.DelDeal();
                        ShowMsg("交易服数据清除完成");
                    }
                    else
                    {
                        ShowMsg("本次任务不清除交易服数据");
                    }

                    // 群发送给客户端 
                    ShowMsg("下发分控安装信息");
                    GroupSend($"搭建测试环境*{_sjmyHelper.Gnpy}*{_sjmyHelper.AppSrc}*{_sjmyHelper.ApkVersion}*{_sjmyHelper.Mtid}*{_sjmyHelper.Uninstall}*{_iniFile.IniReadValue("client", "FastLogin")}*{_iniFile.IniReadValue("client", "AllUninstall")}*{_sjmyHelper.ApkPackage}");

                    // 重置 MobileScriptModule  where  flog + mobile + uploadflog 
                    ShowMsg($"已重置数据库共有{_sjmyHelper.ResetOne()}条数据受到影响");

                    // 修改ScriptFlog = 1
                    _sjmyHelper.SetScriptFlog(1);

                    _sjmyHelper.StartTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                    break;
                }
            }
        }
        #endregion

        // 服务端 监听套接字
        private Socket _sokWatch;

        // 服务端 监听线程
        private Thread _thrWatch;

        // 字典集合:保存 通信套接字
        readonly Dictionary<string, Server> _dictConn = new Dictionary<string, Server>();


        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {

                ShowMsg(RemoteConnect.ConnectState(_sjmyHelper.Site, _sjmyHelper.Account, _sjmyHelper.Password));
                ShowMsg(RemoteConnect.ConnectState(@"\\192.168.9.179", "qadwnew", "qadwnew123."));
                Directory.SetCurrentDirectory(@"D:\developer");

                const int port = 6000;
                // 创建监听套接字，使用ip4协议，流式传输，tcp链接
                _sokWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 绑定端口
                // 获取网络节点对象
                var address = IPAddress.Parse(get64segmentIP.LocalIp);
                var endPoint = new IPEndPoint(address, port);

                // 绑定端口（其实内部 就向系统的端口表中注册了一个端口，并指定了当前程序句柄）
                _sokWatch.Bind(endPoint);

                // 设置监听队列,指限制同时处理的连接请求数，即同时处理的客户端连接请求。
                _sokWatch.Listen(10);

                // 开始监听，调用监听线程 执行 监听套接字的监听方法。
                _thrWatch = new Thread(WatchConncting) { IsBackground = true };
                _thrWatch.Start();

                ShowMsg("服务器已启动！");

            }
            catch (SocketException soex)
            {
                ShowMsg("异常:" + soex);
            }
            catch (Exception ex)
            {
                ShowMsg("异常:" + ex);
            }
        }

        #region  打印消息 + ShowMsg(string strmsg)

        /// <summary>
        /// 打印消息 跨线程添加textbox的值
        /// </summary>
        /// <param name="str">内容</param>
        private delegate void Textbdelegate(string str);

        private void ShowMsg(string msg)
        {
            if (log.InvokeRequired)
            {
                Textbdelegate dt = ShowMsg;
                log.Invoke(dt, msg + "\t\n");
            }
            else
            {
                log.AppendText(ServerSjmyHelper.GetTime() + msg + "\t\n");
            }
        }

        private delegate void Listbdelegate(string str);

        private void Listadd(string msg)
        {
            if (skinGroupBox2.InvokeRequired)
            {
                Listbdelegate dt = Listadd;
                listOnline.Invoke(dt, msg);
            }
            else
            {
                listOnline.Items.Add(msg);
            }
        }

        private void Listremove(string msg)
        {
            if (listOnline.InvokeRequired)
            {
                Listbdelegate dt = Listremove;
                listOnline.Invoke(dt, msg);
            }
            else
            {
                listOnline.Items.Remove(msg);
            }
        }
        #endregion

        // bool isWatch = true;
        #region 服务器监听方法 + void WatchConncting()
        void WatchConncting()
        {
            try
            {
                // 循环监听客户端的连接请求。
                while (true)
                {
                    // 开始监听，返回了一个通信套接字
                    var sockMsg = _sokWatch.Accept();

                    var clientipe = (IPEndPoint)sockMsg.RemoteEndPoint;
                    string[] ipList = { "192.168.64.105", "192.168.64.106", "192.168.64.23", "192.168.64.107" };
                    if (!ipList.Contains(clientipe.Address.ToString()))
                    {
                        continue;
                    }

                    // 创建通信管理类
                    var conn = new Server(sockMsg, ShowMsg, RemoveClient, Analysis);

                    // 将当前连接成功的【与客户端通信的套接字】的标识保存起来，并显示到列表中
                    // 将远程客户端的 ip 和 端口 字符串 存入列表
                    Listadd(sockMsg.RemoteEndPoint.ToString());

                    // 将服务器端的通信套接字存入字典集合。
                    _dictConn.Add(sockMsg.RemoteEndPoint.ToString(), conn);
                    ShowMsg(sockMsg.RemoteEndPoint + " 成功连接！\t\n");

                    conn.Send("成功与服务端建立连接！");
                }
            }
            catch (Exception ex)
            {
                ShowMsg("异常" + ex);
            }
        }
        #endregion

        #region 服务端向指定的客户端发送消息
        /// <summary>服务端向指定的客户端发送消息</summary>
        /// <param name="strMsg">The str Msg.</param>
        public void BtnSendMsg(string strMsg)
        {
            var strClient = listOnline.Text;
            if (_dictConn.ContainsKey(strClient))
            {
                ShowMsg("向客户端【" + strClient + "】说:" + strMsg);

                // 通过指定的套接字将字符串发送到指定的客户端
                try
                {
                    _dictConn[strClient].Send(strMsg);
                }
                catch (Exception ex)
                {
                    ShowMsg("异常" + ex.Message);
                }
            }
        }
        #endregion

        #region 遍历dictConn并发送

        private void GroupSend(string strMsg)
        {
            foreach (var kvp in _dictConn)
            {
                kvp.Value.Send(strMsg);
            }
        }


        #endregion

        #region 根据要中断的客户端ipport关闭连接 + void RemoveClient(string clientIpPort)
        /// <summary>
        ///  根据要中断的客户端port关闭连接
        /// </summary>
        /// <param name="clientIpPort"></param>
        private void RemoveClient(string clientIpPort)
        {
            // 移除列表中的项
            Listremove(clientIpPort);

            // 关闭通信管理类
            _dictConn[clientIpPort].Close();

            // 从字典中移除对应的通信管理类的项
            _dictConn.Remove(clientIpPort);
        }
        #endregion

        #region 重置程序状态
        /// <summary>
        /// 重置监听状态
        /// </summary>
        private void Restart()
        {
            _sjmyHelper.subControl = _dictConn.Count;
            SelectListen();
            ConfigInit();

        }


        #endregion

        #region 分析客户端收到的消息

        private void Analysis(string strMsg)
        {
            try
            {

                // 根据ip修改工作设备数量，防止同时接收到消息

                switch (strMsg.Split('：')[1])
                {

                    case "第一轮全部脚本执行完成":

                        _sjmyHelper.subControl = _sjmyHelper.subControl - 1;
                        ShowMsg($"当前工作设备数量{_sjmyHelper.subControl}");

                        if (_sjmyHelper.subControl == 0)
                        {
                            _sjmyHelper.SetScriptFlog(3);


                            if (_iniFile.IniReadValue("server", "Single") == "1")
                            {
                                _sjmyHelper.EndTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                                _sjmyHelper.subControl = _dictConn.Count;

                                GroupSend($"上传任务结果*{_sjmyHelper.GetPath()}*{_sjmyHelper.Gnpy}*{_sjmyHelper.AppSrc}");
                                _sjmyHelper.InsertFailed();
                                _sjmyHelper.InsertMobileUnpass();
                            }

                            else
                            {
                                // 第二轮 重置MobileScriptModule  uploadflog！=成功
                                var countTow = _sjmyHelper.ResetTwo();
                                ShowMsg($"第二轮已重置数据库共有{countTow}条数据受到影响");

                                if (countTow == 0)
                                {
                                    _sjmyHelper.RecordTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                    ShowMsg("第二轮没有需要执行的脚本");
                                    _sjmyHelper.InsertReult();
                                    // 暂时不下载log  ，因为不上传
                                    if (_iniFile.IniReadValue("server", "LoadLog") == "1")
                                    {
                                        _sjmyHelper.RunCmdPython(
                                            $@"{_sjmyHelper.ProjectSite}\data\script\servercase\sqlload.py");
                                    }

                                    Message99U("367085");
                                    Message99U("201371");
                                    if (_sjmyHelper.Uid != "367085" && _sjmyHelper.Uid != "201371")
                                    {
                                        Message99U(_sjmyHelper.Uid);
                                    }

                                    _sjmyHelper.SetScriptFlog(2);
                                    ShowMsg($"任务id：{_sjmyHelper.Mtid}完成测试");
                                    Restart();
                                }

                                else
                                {
                                    // 开始执行第二轮该做的事情
                                    GroupSend(
                                        $"执行第二轮*{_sjmyHelper.Gnpy}*{_sjmyHelper.AppSrc}*{_sjmyHelper.ApkVersion}*{_sjmyHelper.Mtid}*{_sjmyHelper.Uninstall}");

                                    // 重新获取分控在线数量
                                    _sjmyHelper.subControl = _dictConn.Count;
                                }

                            }

                        }

                        break;

                    case "第二轮全部脚本执行完成":

                        _sjmyHelper.subControl = _sjmyHelper.subControl - 1;
                        ShowMsg($"当前工作设备数量{_sjmyHelper.subControl}");
                        if (_sjmyHelper.subControl == 0)
                        {
                            _sjmyHelper.EndTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                            _sjmyHelper.subControl = _dictConn.Count;

                            GroupSend($"上传任务结果*{_sjmyHelper.GetPath()}*{_sjmyHelper.Gnpy}*{_sjmyHelper.AppSrc}");
                            _sjmyHelper.InsertFailed();
                            _sjmyHelper.InsertMobileUnpass();
                        }


                        break;

                    case "任务上传完成":

                        _sjmyHelper.subControl = _sjmyHelper.subControl - 1;
                        ShowMsg($"当前工作设备数量{_sjmyHelper.subControl}");
                        if (_sjmyHelper.subControl == 0)
                        {
                            _sjmyHelper.RecordTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                            _sjmyHelper.InsertReult();

                            // 暂时不下载log  ，因为不上传
                            if (_iniFile.IniReadValue("server", "LoadLog") == "1")
                            {
                                _sjmyHelper.RunCmdPython($@"{_sjmyHelper.ProjectSite}\data\script\servercase\sqlload.py");
                            }

                            Message99U("367085");
                            Message99U("201371");

                            if (_sjmyHelper.Uid != "367085" && _sjmyHelper.Uid != "201371")
                            {
                                Message99U(_sjmyHelper.Uid);
                            }

                            _sjmyHelper.SetScriptFlog(2);

                            ShowMsg($"任务id：{_sjmyHelper.Mtid}完成测试");
                            Restart();
                        }


                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }

        }

        #endregion

        #region 监听button

        /// <summary>
        /// 开始监听按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void monitorButton_Click(object sender, EventArgs e)
        {

            _sjmyHelper.subControl = _dictConn.Count;
            if (_sjmyHelper.subControl == 3)
            {
                SelectListen();
            }
            else
            {
                var result = MessageBox.Show(@"当前设备小于3台，是否继续执行任务", @"提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    SelectListen();
                }
                else
                {
                    ShowMsg("用户取消监听操作！！！\r\n");
                    return;
                }
            }


            ShowMsg("开始监听！！！\r\n");
            monitorButton.Visible = false;

        }

        #endregion

        #region 测试button

        private void button1_Click(object sender, EventArgs e)
        {
            _sjmyHelper.ProjectSite = @"D:\developer\myht";
            _sjmyHelper.RunCmdPython($@"{_sjmyHelper.ProjectSite}\data\script\servercase\sqlconfig.py", "123456");
            MessageBox.Show(@"ok");
        }

        #endregion

        #region appExit
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }


        #endregion

        #region 99u消息发送

        private void Message99U(string uid)
        {
            Process.Start(
                $@"{_sjmyHelper.ProjectSite}\data\lib\99uMessage.exe",
                $@"{uid} 测试完成--推送IP:192.168.64.105，游戏名称:手机魔域，任务编号:{
                        _sjmyHelper.Mtid}，app:{
                        _sjmyHelper.ApkName
                    }，客户端版本号:{_sjmyHelper.ApkVersion}，总脚本数:{_sjmyHelper.ALLNum}，通过:{
                        _sjmyHelper.PassNum()
                    }，不通过:{_sjmyHelper.UnpassNum()}，失败:{_sjmyHelper.FailedNum()}，报告链接:{
                        _sjmyHelper.ReportUrl()
                    }");
        }

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            _sjmyHelper.ProjectSite = @"D:\developer\sjmy";
            _sjmyHelper.RunCmdPython($@"{_sjmyHelper.ProjectSite}\data\script\servercase\sqlconfig.py", "123456");
        }

        private void ConfigInit()
        {
            _iniFile.IniWriteValue("server", "TradingDel", "0");
            _iniFile.IniWriteValue("server", "LoadLog", "0");
            _iniFile.IniWriteValue("server", "Single", "0");
            _iniFile.IniWriteValue("client", "FastLogin", "0");
            _iniFile.IniWriteValue("client", "AllUninstall", "0");

        }

        
    }
}
