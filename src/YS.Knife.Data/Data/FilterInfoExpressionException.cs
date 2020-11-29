using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data
{

    [Serializable]
    public class FilterInfoExpressionException : Exception
    {
        public FilterInfoExpressionException() { }
        public FilterInfoExpressionException(string message) : base(message) { }
        public FilterInfoExpressionException(string message, Exception inner) : base(message, inner) { }
        protected FilterInfoExpressionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
