using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
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
