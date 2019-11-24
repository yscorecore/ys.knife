using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    /// <summary>
    /// 表示策略模式
    /// </summary>
    public class Strategy : BaseEntity
    {
        /// <summary>
        /// 表示策略ID
        /// </summary>
        public Guid StrategyID { get; set; }
        /// <summary>
        /// 表示策略的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表示接口类型（不需要包含程序集信息）
        /// </summary>
        public string InterfaceType { get; set; }
        /// <summary>
        /// 表示上下文的类型（需要包含程序集信息)
        /// </summary>
        public string InstanceType { get; set; }
        /// <summary>
        /// 获取或设置默认值(json文本)
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
