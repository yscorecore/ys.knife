using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class NotStartsWithOperator : StartsWithOperator
    {
        public override Operator Operator => Operator.NotStartsWith;
        protected override Expression CompareValue(Expression left, Expression right, Type type)
        {
            return Expression.Not(base.CompareValue(left, right, type));
        }
    }
}
