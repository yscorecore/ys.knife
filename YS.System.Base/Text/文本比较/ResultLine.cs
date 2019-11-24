using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Text
{
    /// <summary>
    /// 表示一个比较结果行
    /// </summary>
    public struct ResultLine
    {
        /// <summary>
        /// 对应数据A中的行号，如果无对应行，则为-1
        /// </summary>
        public long LineNumberA{get;set;}
        /// <summary>
        /// 对应数据B中的行号，如果无对应行，则为-1
        /// </summary>
        public long LineNumberB { get; set; }
        /// <summary>
        /// 对应数据A中的内容
        /// </summary>
        public string LineContentA { get; set; }
        /// <summary>
        /// 对应数据B中的内容
        /// </summary>
        public string LineContentB { get; set; }
        /// <summary>
        /// 该行的状态
        /// </summary>
        public CompareState ResultState { get; set; }
    }
}
