// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteConnect.cs" company="231216@nd">
//   
// </copyright>
// <summary>
//   The remote connect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace 服务端
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The remote connect.
    /// </summary>
    internal static class RemoteConnect
    {
        #region

        /// <summary>  
        /// 连接远程共享文件夹  
        /// </summary>  
        /// <param name="path">远程共享文件夹的路径</param>  
        /// <param name="userName">用户名</param>  
        /// <param name="passWord">密码</param>  
        /// <returns></returns>  
        public static string ConnectState(string path, string userName, string passWord)
        {
            var proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                var dosLine = "net use " + path + " " + passWord + " /user:" + userName;
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }

                var errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    return path + @"连接成功";
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
        }

        #endregion
    }
}
