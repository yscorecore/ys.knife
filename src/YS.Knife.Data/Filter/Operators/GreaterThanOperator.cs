using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class GreaterThanOperator : ComparableOperator
    {
        public override Operator Operator => Operator.GreaterThan;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.GreaterThan;
    }
}
