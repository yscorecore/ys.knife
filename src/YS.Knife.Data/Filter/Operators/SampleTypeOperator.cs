using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                    return CompareConstAndConst(left.Value, right.Value);
                }
                else
                {
                    return CompareConstAndExpression(left, right);
                }
            }
            else
            {
                if (right.IsConstValue)
                {
                    return CompareExpressionAndConst(left, right);
                }
                else
                {
                    return CompareExpressionAndExpression(left, right);
                }
            }
            Expression CompareConstAndExpression(FilterValueDesc left, FilterValueDesc right)
            {
                if (left.Value == null)
                {
                    if (right.ExpressionValueType.IsValueType && !right.ExpressionValueType.IsNullableType())
                    {
                        Type targetType = typeof(Nullable<>).MakeGenericType(right.ExpressionValueType);
                        return CompareValue(
                            Expression.Convert(right.ValueExpression, targetType), Expression.Constant(null, targetType), targetType);
                    }
                }
                return CompareValue(ConstValueExpression(left.Value, right.ExpressionValueType), right.ValueExpression, right.ExpressionValueType);
            }
            Expression CompareExpressionAndConst(FilterValueDesc left, FilterValueDesc right)
            {
                if (right.Value == null)
                {
                    if (left.ExpressionValueType.IsValueType && !left.ExpressionValueType.IsNullableType())
                    {
                        Type targetType = typeof(Nullable<>).MakeGenericType(left.ExpressionValueType);
                        return CompareValue(
                            Expression.Convert(left.ValueExpression, targetType), Expression.Constant(null, targetType), targetType);
                    }
                }
                return CompareValue(left.ValueExpression, ConstValueExpression(right.Value, left.ExpressionValueType), left.ExpressionValueType);
            }
            Expression CompareExpressionAndExpression(FilterValueDesc left, FilterValueDesc right)
            {
                if (left.ExpressionValueType == right.ExpressionValueType)
                {
                    return CompareValue(left.ValueExpression, right.ValueExpression, left.ExpressionValueType);
                }
                else if (Nullable.GetUnderlyingType(left.ExpressionValueType) == right.ExpressionValueType)
                {
                    // left is nullable, right is value 
                    return CompareValue(left.ValueExpression,
                        Expression.Convert(right.ValueExpression, left.ExpressionValueType)
                        , left.ExpressionValueType);
                }
                else if (Nullable.GetUnderlyingType(right.ExpressionValueType) == left.ExpressionValueType)
                {
                    // right is nullable, left is value 
                    return CompareValue(Expression.Convert(left.ValueExpression, right.ExpressionValueType),
                        right.ValueExpression
                        , right.ExpressionValueType);
                }
                else
                {
                    return CompareValue(left.ValueExpression,
                         Expression.Convert(right.ValueExpression, left.ExpressionValueType)
                        , left.ExpressionValueType);
                }
            }
            Expression CompareConstAndConst(object left, object right)
            {
                if (left is null)
                {
                    if (right is null)
                    {
                        throw ExpressionErrors.CompareBothNullError();
                    }
                    else
                    {
                        var targetType = right.GetType();
                        if (targetType.IsValueType)
                        {
                            targetType = typeof(Nullable<>).MakeGenericType(right.GetType());
                        }
                        return CompareValue(ConstValueExpression(left, targetType), ConstValueExpression(right, targetType), targetType);
                    }
                }
                else
                {
                    var targetType = left.GetType();
                    if (right is null && targetType.IsValueType)
                    {
                        targetType = typeof(Nullable<>).MakeGenericType(left.GetType());
                    }
                    return CompareValue(ConstValueExpression(left, targetType), ConstValueExpression(right, targetType), targetType);
                }
            }

            Expression ConstValueExpression(object value, Type targetType)
            {
                if (targetType.IsNullableType() && value is null)
                {
                    return Expression.Constant(value, targetType);
                }
                return Expression.Constant(ChangeType(value, targetType), targetType);
            }

        }


        protected abstract Expression CompareValue(Expression left, Expression right, Type type);

        private object ChangeType(object value, Type valueType)
        {
            if (value == null)
            {
                return valueType.DefaultValue();
            }
            try
            {
                var originType = Nullable.GetUnderlyingType(valueType) ?? valueType;
                if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(originType))
                {
                    return Convert.ChangeType(value, originType);
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(originType);
                    return converter.ConvertFrom(value);
                }
            }
            catch (Exception ex)
            {
                throw ExpressionErrors.ConvertValueError(value, valueType, ex);
            }
        }
       

    }
}
