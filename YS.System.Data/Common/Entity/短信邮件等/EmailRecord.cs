using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    public class EmailRecord : System.Data.Entity.LoopRecordBase
    {

        public Guid EmailRecordId { get; set; }

        public string BodyContent { get; set; }

        /// <summary>
        /// 表示邮件的主题
        /// </summary>
        public string Subject { get; set; }
        public string IsBodyHtml { get; set; }

        /// <summary>
        /// 表示处理程序的名称
        /// </summary>
        public string ProviderName { get; set; }
    }
}
