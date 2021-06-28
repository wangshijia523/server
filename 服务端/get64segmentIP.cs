using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 服务端
{
    using System.Net;
    using System.Net.Sockets;

    class get64segmentIP
    {
        /// <summary>
        /// 获得64段ip（为了跳过虚拟机）
        /// </summary>
        /// <returns></returns>
        public static string LocalIp
        {
            get
            {
                try
                {
                    var hostName = Dns.GetHostName(); // 得到主机名
                    var ipEntry = Dns.GetHostEntry(hostName);
                    foreach (var t in ipEntry.AddressList)
                    {
                        // 从IP地址列表中筛选出IPv4类型的IP地址
                        // AddressFamily.InterNetwork表示此IP为IPv4,
                        // AddressFamily.InterNetworkV6表示此地址为IPv6类型
                        if (t.AddressFamily != AddressFamily.InterNetwork) continue;
                        if (t.ToString().Split('.')[2] == "255"|| t.ToString().Split('.')[2] == "52")
                        {
                            return t.ToString();
                        }
                    }
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
