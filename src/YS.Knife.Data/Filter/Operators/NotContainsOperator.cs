using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class NotContainsOperator : ContainsOperator
    {
        public override Operator Operator => Operator.NotContains;
        protected override Expression CompareValue(Expression left, Expression right, Type type)
        {
            return Expression.Not(base.CompareValue(left, right, type));
        }
    }
}
