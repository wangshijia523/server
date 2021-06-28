// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunCmd.cs" company="231216@nd">
//   
// </copyright>
// <summary>
//   Defines the RunCmd type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace 服务端
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class RunCmd
    {
        private readonly Process proc = null;

        public RunCmd()
        {
            proc = new Process();
        }

        public string Exe(string cmd)
        {
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            //proc.OutputDataReceived += sortProcess_OutputDataReceived;
            proc.Start();
            var cmdWriter = proc.StandardInput;
            //proc.BeginOutputReadLine();

            if (!string.IsNullOrEmpty(cmd))
            {
                cmdWriter.WriteLine(cmd + "&exit");
            }

            var result = proc.StandardOutput.ReadToEnd();

			cmdWriter.Close();           
            proc.WaitForExit();
            proc.Close();
            return result;

        }

        private void sortProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        }
    }
}
