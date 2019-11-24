using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    /// <summary>
    /// 表示操作日志
    /// </summary>
    public class OperationLog : System.Data.Entity.BaseEntity<Guid>
    {

        /// <summary>
        /// 表示日志的概要信息
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 表示日志的详细信息
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// 表示操作者的名称
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 表示操作者的地址信息(IP,MAC等)
        /// </summary>
        public string OperatorAddress { get; set; }
    }
}
