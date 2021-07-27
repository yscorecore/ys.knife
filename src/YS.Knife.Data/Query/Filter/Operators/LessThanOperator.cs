using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Filter.Operators
{
    class LessThanOperator : ComparableOperator
    {
        public override Operator Operator => Operator.LessThan;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.LessThan;
    }
}
