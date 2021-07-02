using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(Operator.GreaterThanOrEqual)]
    internal class GreaterThanOrEqualExpressionConverter : OpExpressionConverter
    {
        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.GreaterThanOrEqual;
        protected override Func<Expression, Expression, BinaryExpression> ReverseCompareFunc => Expression.LessThanOrEqual;
    }
}
