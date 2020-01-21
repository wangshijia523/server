// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" company="nd@231216">
//   
// </copyright>
// <summary>
//   Defines the Server type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace 服务端
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using System.Windows.Forms;

    public class Server
    {
        //ServerSjmyHelper sjmyHelper = new ServerSjmyHelper();

        // 与某个客户端通信套接字
        Socket sokMsg;

        // 通信线程
        Thread thrMsg;

        // 创建一个委托对象， 在窗体显示消息的方法

        DGShowMsg dgShow;

        // 创建一个关闭连接的方法
        DGCloseConn dgCloseConn;

        // 创建一个分析消息的方法

        DGAnalysis dgAnalysis;

        #region 构造函数
        public Server(Socket sokMsg, DGShowMsg dgShow, DGCloseConn dgCloseConn, DGAnalysis dgAnalysis)
        {
            this.sokMsg = sokMsg;
            this.dgShow = dgShow;
            this.dgCloseConn = dgCloseConn;
            this.dgAnalysis = dgAnalysis;

            // 创建通信线程，负责调用通信套接字，来接收客户端消息。
            thrMsg = new Thread(ReceiveMsg) { IsBackground = true };
            thrMsg.Start(this.sokMsg);
        }
        #endregion

        bool isReceive = true;
        #region 接收客户端发送的消息

        public void ReceiveMsg(object obj)
        {
            var sockMsg = obj as Socket;

            // 通信套接字 监听客户端的消息,传输的是byte格式。
            // 开辟了一个 1M 的空间，创建的消息缓存区，接收客户端的消息。
            byte[] arrMsg = new byte[1024 * 1024 * 1];
            try
            {
                while (isReceive)
                {
                    // 注意：Receive也会阻断当前的线程。
                    // 接收客户端的消息,并存入消息缓存区。
                    // 并 返回 真实接收到的客户端数据的字节长度。
                    var realLength = sockMsg.Receive(arrMsg);
                    // 将接收的消息转成字符串
                    var strMsg = System.Text.Encoding.UTF8.GetString(arrMsg, 0, realLength);
                    dgShow(strMsg);

                    dgAnalysis(strMsg);
                    // 将消息显示到文本框 用于接收客户端反馈             
                }
            }
            catch (Exception)
            {
                // 显示消息
                dgShow(sokMsg.RemoteEndPoint + "断开连接！\t\n");

                // 调用窗体类的关闭移除方法
                if (sokMsg != null)
                {
                    dgCloseConn(sokMsg.RemoteEndPoint.ToString());
                }
               

            }
        }
        #endregion

        #region 向客户端发送文本消息 + void Send(string msg)
        /// <summary>
        /// 向客户端发送文本消息
        /// </summary>
        /// <param name="msg"></param>
        public void Send(string msg)
        {
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(msg);

            //通过指定的套接字将字符串发送到指定的客户端
            try
            {
                sokMsg.Send(arrMsg);
            }
            catch (Exception ex)
            {
                dgShow("异常" + ex.Message);
            }
        }
        #endregion



        #region 关闭通信
        /// <summary>
        /// 关闭通信
        /// </summary>
        public void Close()
        {
            isReceive = false;
            sokMsg.Close();
            sokMsg = null;
        }
        #endregion
    }
}
