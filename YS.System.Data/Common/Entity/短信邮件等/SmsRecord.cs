using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    /// <summary>
    /// 表示短信记录
    /// </summary>
    public class SmsRecord : System.Data.Entity.LoopRecordBase
    {
        /// <summary>
        /// 短信记录的ID
        /// </summary>
        public Guid SmsRecordId { get; set; }
        /// <summary>
        /// 短信的内容
        /// </summary>
        public string SmsContent { get; set; }
        /// <summary>
        /// 短信处理程序的名称
        /// </summary>
        public string ProviderName { get; set; }
        /// <summary>
        /// 表示发送结果
        /// </summary>
        public string ResultContent { get; set; }
        /// <summary>
        /// 表示接收短信的手机号码
        /// </summary>
        public string Telphone { get; set; }
    }
}
