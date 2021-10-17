using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
{
    class NotEndsWithOperator : EndsWithOperator
    {
        public override Operator Operator => Operator.NotEndsWith;
        protected override Expression CompareValue(Expression left, Expression right, Type type)
        {
            return Expression.Not(base.CompareValue(left, right, type));
        }
    }
}
