using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class StartsWithOperator : StringOperator
    {
        protected override string MethodName => nameof(string.StartsWith);
        public override Operator Operator => Operator.StartsWith;
    }
}
