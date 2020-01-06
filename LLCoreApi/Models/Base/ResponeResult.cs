using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLCoreApi.Models.Base
{
    public class ResponeResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public string state { get; set; }
    }
}
