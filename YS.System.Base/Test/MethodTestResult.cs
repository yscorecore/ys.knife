using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Test
{
    /// <summary>
    /// 表示方法的测试执行结果
    /// </summary>
    public class MethodTestResult : FunctionResult<object>
    {
        public MethodTestResult(object result, TimeSpan ticks)
            : base(result)
        {
            this.Elapsed = ticks;
        }
        public MethodTestResult(Exception error)
            : base(error)
        {

        }
        public MethodTestResult(Exception error, TimeSpan ticks)
            : base(error)
        {
            this.Elapsed = ticks;
        }
        /// <summary>
        /// 获取方法执行的总运行时间
        /// </summary>
        public TimeSpan Elapsed { get; private set; }

        public override string ToString()
        {
            return string.Format("Elapsed:{0}",Elapsed);
        }
    }

}
