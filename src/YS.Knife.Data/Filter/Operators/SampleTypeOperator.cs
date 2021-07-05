using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    public abstract class SampleTypeOperator : IFilterOperator
    {
        public abstract Operator Operator { get; }

        public virtual Expression CompareValue(FilterValueDesc left, FilterValueDesc right)
        {
            if (left.IsConstValue)
            {
                if (right.IsConstValue)
                {
                    return CompareConstValue(left.Value, right.Value);
                }
                else
                {
                    var leftExpression = BuildExpression(left.Value, right.ExpressionValueType);
                    var rightExpression = right.CurrentExpression;
                    return CompareValue(leftExpression, rightExpression);
                }
            }
            else
            {
                if (right.IsConstValue)
                {
                    var leftExpression = left.CurrentExpression;
                    var rightExpression = BuildExpression(right.Value, left.ExpressionValueType);
                    return CompareValue(leftExpression, rightExpression);
                }
                else
                {
                    var leftExpression = left.CurrentExpression;
                    var rightExpression =
                       ConvertExpressionType(right.CurrentExpression, right.ExpressionValueType, left.ExpressionValueType);
                    return CompareValue(leftExpression, rightExpression);
                }
            }
        }

        private Expression CompareConstValue(object left, object right)
        {
            if (left is null)
            {
                if (right is null)
                {
                    return CompareValue(Expression.Constant(null), Expression.Constant(null));
                }
                else
                {
                    var leftExpression = BuildExpression(left,right.GetType());
                    var rightExpression = Expression.Constant(right);
                    return CompareValue(leftExpression, rightExpression);
                }
            }
            else
            {
                var leftExpression = Expression.Constant(left);
                var rightExpression = BuildExpression(right, left.GetType());
                return CompareValue(leftExpression, rightExpression);
            }
        }
        protected abstract Expression CompareValue(Expression left, Expression right);
        private Expression BuildExpression(object value, Type valueType)
        {
            return null;
        }
        private Expression ConvertExpressionType(Expression expression, Type currentType, Type targetType)
        {
            if (currentType == targetType)
            {
                return expression;
            }
            // TODO ...
            return null;
        }
    }
}
