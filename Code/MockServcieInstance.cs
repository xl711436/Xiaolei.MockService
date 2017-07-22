using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xiaolei.TraceLib;

namespace Xiaolei.MockService
{
    /// <summary> 实现类似windows 服务的编程模型
    /// </summary>
    public class MockServcieInstance
    {
        /// <summary>服务是否结束的标记
        /// </summary>
        public bool ExitFlag { get; set; }

        public MockServcieInstance()
        {
         TraceHelper.TraceInfo("MockServcie Instance");
        }

        public void Start()
        {
            TraceHelper.TraceInfo("MockServcie Start");
        }

        public void Stop()
        {
            TraceHelper.TraceInfo("MockServcie Stop");
        } 
    }

}
