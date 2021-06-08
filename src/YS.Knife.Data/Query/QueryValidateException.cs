using System;
using System.Runtime.Serialization;

namespace YS.Knife.Data
{
    [Serializable]
    public class QueryValidateException:ApplicationException
    {
        public QueryValidateException(string message) : base(message) { }
        public QueryValidateException(string message, Exception inner) : base(message, inner) { }

        protected QueryValidateException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

}
