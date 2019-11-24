using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示执行结果，包括执行过程中所发的异常
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    [Serializable]
    public class FunctionResult<T>
    {
        public FunctionResult()
        {

        }
        public FunctionResult(T result)
        {
            this.Result = result;
        }
        public FunctionResult(Exception error)
        {
            this.Error = error;
        }
        public FunctionResult(Exception error, T result)
        {
            this.Error = error;
            this.Result = result;
        }
        public T Result
        {
            get;
            set;
        }
        /// <summary>
        /// 获取一个值，该值反应了是否执行成功
        /// </summary>
        public bool Succeed
        {
            get
            {
                return this.Error == null;
            }
        }
        /// <summary>
        /// 获取一个值，该值反应了是否有错误发生
        /// </summary>
        public bool HasError
        {
            get
            {
                return this.Error != null;
            }
        }
        public Exception Error { get; set; }
    }
}
