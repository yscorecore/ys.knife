using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Aop
{
    [Serializable]
    public class AopException:Exception
    {
        public AopException() { }
        public AopException(string message) : base(message) { }
        public AopException(string message,Exception inner) : base(message,inner) { }
        protected AopException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info,context) { }
    }
}
