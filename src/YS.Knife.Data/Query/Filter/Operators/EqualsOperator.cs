using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Filter.Operators
{
    class EqualsOperator : SampleTypeOperator
    {
        public override Operator Operator => Operator.Equals;

        protected override Expression CompareValue(Expression left, Expression right, Type type)
        {
            return Expression.Equal(left, right);
        }
    }
}
