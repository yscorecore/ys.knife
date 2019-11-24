using System;
using System.Collections.Generic;
using System.Text;

namespace System.Record
{
    /// <summary>
    /// 提供记录的事件参数。
    /// </summary>
    public class RecordEventArgs : EventArgs
    {
        private readonly OperationBase m_Operation;
        /// <summary>
        /// 获取操作对象。
        /// </summary>
        public OperationBase Operation
        {
            get { return m_Operation; }
        }
        /// <summary>
        /// 初始化<see cref="RecordEventArgs"/>的新实例
        /// </summary>
        /// <param name="operation">操作对象</param>
        public RecordEventArgs (OperationBase operation)
        {
            this.m_Operation = operation;
        }
    }
}
