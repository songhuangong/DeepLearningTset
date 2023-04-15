using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ToolsLibrary
{
    public class CmdWindowCtrl
    {
        public Process? myPro { get; set; } = null;

        //执行命令的返回值！
        public string StrReturn { get; set; } = "";



        //Cmd的工作目录
        private string workingDirectory="";
        public string WorkingDirectory
        {
            get { return workingDirectory; }
        }



        private int? id = null;

        public int? Id
        {
            get { return id; }
        }


        public void SetWorkingDirectory(string path)
        {
            workingDirectory = path;    
        }


        /// <summary>
        /// 运行cmd命令
        /// 会显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        public async Task<bool> RunCmd(string cmdExe, string cmdStr, bool createNoWindow = true)
        {
            bool result = false;
            try
            {
                if (myPro == null)
                {
                    id = null;
                    myPro = new Process();
                }

                if (id != null)
                {
                    var process = Process.GetProcessById((int)id);
                    if (process != null)
                    {
                        MessageBox.Show($"服务已启动！Id: {id}");
                        return false;
                    }
                }
                
                using (myPro)
                {
                    
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);

                    //不显示程序窗口
                    psi.CreateNoWindow = createNoWindow;

                    //Vista or higher check
                    if (System.Environment.OSVersion.Version.Major >= 6)
                    {
                        psi.Verb = "runas";
                    }
                    psi.UseShellExecute = false;

                    if (workingDirectory != "")
                    {
                        psi.WorkingDirectory = workingDirectory;
                    }
                    
                    myPro.StartInfo = psi;
                    myPro.Start();
                    this.id = myPro.Id;
                    await myPro.WaitForExitAsync();
                    result = true;
                }
                myPro = null;
                id = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return result;
        }



        /// <summary>
        /// 运行cmd.exe命令(输入输出，重定向了)
        /// </summary>
        /// <param name="cmdExe"></param>
        /// <param name="cmdStr"></param>
        /// <param name="createNoWindow"></param>
        /// <returns></returns>
        public async Task<bool> RunCmdExe(string cmdExe, string cmdStr, bool createNoWindow = true)
        {
            bool result = false;
            try
            {
                if (myPro == null)
                {
                    id = null;
                    myPro = new Process();
                }

                if (id != null)
                {
                    var process = Process.GetProcessById((int)id);
                    if (process != null)
                    {
                        MessageBox.Show("服务已启动！");
                        return false;
                    }
                }

                using (myPro)
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    //是否使用操作系统shell启动
                    myPro.StartInfo.UseShellExecute = false;
                    //接受来自调用程序的输入信息
                    myPro.StartInfo.RedirectStandardInput = true;
                    //由调用程序获取输出信息
                    myPro.StartInfo.RedirectStandardOutput = true;
                    //重定向标准错误输出
                    myPro.StartInfo.RedirectStandardError = true;
                    //不显示程序窗口
                    myPro.StartInfo.CreateNoWindow = createNoWindow;

                    if (workingDirectory != "")
                    {
                        myPro.StartInfo.WorkingDirectory = workingDirectory;
                    }


                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");
                    //string str = string.Format(@"""{0}"" {1} ", cmdExe, cmdStr);

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;

                    this.id = myPro.Id;
                    await myPro.WaitForExitAsync();
                    //读取返回值
                    StrReturn =  await myPro.StandardOutput.ReadToEndAsync();

                    result = true;
                }
                myPro = null;
                id = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }



        public void Kill()
        {
            if (myPro != null)
            {
                myPro.Kill(true);
            }
        }


    }
}
