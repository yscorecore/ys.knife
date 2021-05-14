using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(FilterType.GreaterThan)]
    internal class GreaterThanExpressionConverter : OpExpressionConverter
    {
        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.GreaterThan;
        protected override Func<Expression, Expression, BinaryExpression> ReverseCompareFunc => Expression.LessThan;
    }
}
