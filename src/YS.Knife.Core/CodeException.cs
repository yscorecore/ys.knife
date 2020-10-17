using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{


    [Serializable]
    public sealed class CodeException : ApplicationException
    {
        public string Code { get; set; }
        public CodeException(string code)
        {
            this.Code = code;
        }

        public CodeException(string code, string message) : base(message)
        {
            this.Code = code;
        }
        public CodeException(string code, string message, Exception inner) : base(message, inner)
        {
            this.Code = code;
        }
        protected CodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
