using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class NotEqualsOperator : SampleTypeOperator
    {
        public override Operator Operator => Operator.NotEquals;

        protected override Expression CompareValue(Expression left, Expression right, Type type)
        {
            return Expression.NotEqual(left, right);
        }
    }
}
