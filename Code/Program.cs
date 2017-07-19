using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Xiaolei.TraceLib;

namespace Xiaolei.MockService
{
    static class Program
    {

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, IntPtr wParam, IntPtr lParam);

        public static int MessageCode_Stop = 0x80F0;

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0].ToLower() == "start")
                {
                    bool createdNew;
                    System.Threading.Mutex instance = new System.Threading.Mutex(true, "MockService", out createdNew);
                    if (createdNew)
                    {
                        MockServcieInstance curInstance = new MockServcieInstance();
                        curInstance.ExitFlag = false; 
                        Application.AddMessageFilter(new MsgFilter(curInstance));
                        curInstance.Start();

                        //维持循环，接收推出消息
                        while (!curInstance.ExitFlag)
                        {
                            Application.DoEvents();
                            System.Threading.Thread.Sleep(1000);
                        }
                        instance.ReleaseMutex();
                    }
                    else
                    {
                        TraceHelper.TraceInfo("MockService Started "); 
                    }
                }

                if (args[0].ToLower() == "stop")
                {

                    bool createdNew;
                    System.Threading.Mutex instance = new System.Threading.Mutex(true, "SingletonForm", out createdNew);
                    if (createdNew)
                    {
                        TraceHelper.TraceInfo("MockService not Start ,start it frist");
                        instance.ReleaseMutex();
                    }
                    else
                    {
                        TraceHelper.TraceInfo(" Post Message to Stop MockService");
                        Process[] curProcessArray = Process.GetProcessesByName("SingletonForm");

                        foreach (Process curProcess in curProcessArray)
                        {
                            TraceHelper.TraceInfo(" curProcess Id: " + curProcess.Id);
                            PostThreadMessage(curProcess.Threads[0].Id, (uint)MessageCode_Stop, IntPtr.Zero, IntPtr.Zero);
                        }
                         
                        Application.Exit();
                    }
                }
            }
        }
    }

    /// <summary>监听线程消息
    /// </summary>
    public class MsgFilter : IMessageFilter
    {
        private MockServcieInstance curInstance;

        public MsgFilter(MockServcieInstance I_Instance)
        {
            curInstance = I_Instance;
        }

        public bool PreFilterMessage(ref Message m)
        {
            //如果收到退出消息，退出程序
            if (m.Msg == Program.MessageCode_Stop)
            { 
                curInstance.Stop();
                curInstance.ExitFlag = true;

                return true;
            }
            return false;
        }
    }
     

}
