using System;

namespace YS.Knife
{
    [Serializable]
    public class KnifeException : Exception
    {
        public KnifeException() { }
        public KnifeException(string message) : base(message) { }
        public KnifeException(string message, Exception inner) : base(message, inner) { }
        protected KnifeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
