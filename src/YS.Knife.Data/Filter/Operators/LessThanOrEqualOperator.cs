using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class LessThanOrEqualOperator : ComparableOperator
    {
        public override Operator Operator => Operator.LessThanOrEqual;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.LessThanOrEqual;
    }
}
