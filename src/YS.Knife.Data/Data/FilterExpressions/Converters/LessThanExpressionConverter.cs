using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(Operator.LessThan)]
    internal class LessThanExpressionConverter : OpExpressionConverter
    {
        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.LessThan;
        protected override Func<Expression, Expression, BinaryExpression> ReverseCompareFunc => Expression.GreaterThan;
    }
}
