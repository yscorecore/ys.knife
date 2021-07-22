using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class LessThanOperator : ComparableOperator
    {
        public override Operator Operator => Operator.LessThan;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.LessThan;
    }
}
