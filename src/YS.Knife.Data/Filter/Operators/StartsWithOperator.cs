using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class StartsWithOperator : StringOperator
    {
        public override Operator Operator => Operator.StartsWith;

        protected override Expression CompareValue(Expression left, Expression right)
        {
            throw new NotImplementedException();
        }
    }
}
