using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Data.Query.Expressions
{

    [Serializable]
    public class QueryExpressionException : QueryException
    {
        public QueryExpressionException() { }
        public QueryExpressionException(string message) : base(message) { }
        public QueryExpressionException(string message, Exception inner) : base(message, inner) { }
        protected QueryExpressionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
