using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YS.Knife.Aop;

namespace YS.Knife
{
    [Serializable]
    public class CodeException : ApplicationException
    {
        public int Code { get; set; }
        public CodeException()
        {
        }
        public CodeException(int code)
        {
            this.Code = code;
        }

        public CodeException(int code, string message) : base(message)
        {
            this.Code = code;
        }
        public CodeException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public CodeException(int code, string message, Exception inner) : base(message, inner)
        {
            this.Code = code;
        }
        protected CodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
}
