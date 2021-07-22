using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class GreaterThanOrEqualOperator : ComparableOperator
    {
        public override Operator Operator => Operator.GreaterThanOrEqual;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.GreaterThanOrEqual;
    }
}
