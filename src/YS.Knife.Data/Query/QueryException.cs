using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Data.Query
{

    [Serializable]
    public class QueryException : ApplicationException
    {
        public QueryException() { }
        public QueryException(string message) : base(message) { }
        public QueryException(string message, Exception inner) : base(message, inner) { }
        protected QueryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
