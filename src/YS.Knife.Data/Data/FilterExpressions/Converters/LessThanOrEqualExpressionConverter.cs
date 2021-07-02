using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(Operator.LessThanOrEqual)]
    internal class LessThanOrEqualExpressionConverter : OpExpressionConverter
    {
        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.LessThanOrEqual;
        protected override Func<Expression, Expression, BinaryExpression> ReverseCompareFunc => Expression.GreaterThanOrEqual;
    }
}
