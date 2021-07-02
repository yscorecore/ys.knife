using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace YS.Knife.Data.Filter.Operators
{
    // user can not extend new operator, so make filter operator internal
    internal interface IFilterOperator
    {
        Expression CompareValue(FilterValueDesc left, Operator opType, FilterValueDesc right);

        internal static Expression CreateOperatorExpression(FilterValueDesc left, Operator opType, FilterValueDesc right) {
            if (right.ExpressionValueType != left.ExpressionValueType)
            {
                return Expression.Equal(left.CurrentExpression, Expression.Convert(right.CurrentExpression, left.ExpressionValueType));
            }
            return Expression.Equal(left.CurrentExpression, right.CurrentExpression);
        }
    }
}
