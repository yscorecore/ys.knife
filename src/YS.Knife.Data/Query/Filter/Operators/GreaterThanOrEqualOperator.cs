using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
{
    class GreaterThanOrEqualOperator : ComparableOperator
    {
        public static GreaterThanOrEqualOperator Default = new GreaterThanOrEqualOperator();
        public override Operator Operator => Operator.GreaterThanOrEqual;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.GreaterThanOrEqual;
    }
}
