using System;

namespace YS.Knife.Data
{

    [Serializable]
    public class FieldExpressionException : Exception
    {
        public FieldExpressionException() { }
        public FieldExpressionException(string message) : base(message) { }
        public FieldExpressionException(string message, Exception inner) : base(message, inner) { }
        protected FieldExpressionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
