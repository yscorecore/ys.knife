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
                    return CompareConstAndConst(left.ConstValue, right.ConstValue);
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
                if (left.ConstValue == null)
                {
                    if (right.PathValueType.IsValueType && !right.PathValueType.IsNullableType())
                    {
                        Type targetType = typeof(Nullable<>).MakeGenericType(right.PathValueType);
                        return CompareValue(
                            Expression.Convert(right.PathValueExpression, targetType), Expression.Constant(null, targetType), targetType);
                    }
                }
                return CompareValue(ConstValueExpression(left.ConstValue, right.PathValueType), right.PathValueExpression, right.PathValueType);
            }
            Expression CompareExpressionAndConst(FilterValueDesc left, FilterValueDesc right)
            {
                if (right.ConstValue == null)
                {
                    if (left.PathValueType.IsValueType && !left.PathValueType.IsNullableType())
                    {
                        Type targetType = typeof(Nullable<>).MakeGenericType(left.PathValueType);
                        return CompareValue(
                            Expression.Convert(left.PathValueExpression, targetType), Expression.Constant(null, targetType), targetType);
                    }
                }
                return CompareValue(left.PathValueExpression, ConstValueExpression(right.ConstValue, left.PathValueType), left.PathValueType);
            }
            Expression CompareExpressionAndExpression(FilterValueDesc left, FilterValueDesc right)
            {
                if (left.PathValueType == right.PathValueType)
                {
                    return CompareValue(left.PathValueExpression, right.PathValueExpression, left.PathValueType);
                }
                else if (Nullable.GetUnderlyingType(left.PathValueType) == right.PathValueType)
                {
                    // left is nullable, right is value 
                    return CompareValue(left.PathValueExpression,
                        Expression.Convert(right.PathValueExpression, left.PathValueType)
                        , left.PathValueType);
                }
                else if (Nullable.GetUnderlyingType(right.PathValueType) == left.PathValueType)
                {
                    // right is nullable, left is value 
                    return CompareValue(Expression.Convert(left.PathValueExpression, right.PathValueType),
                        right.PathValueExpression
                        , right.PathValueType);
                }
                else
                {
                    return CompareValue(left.PathValueExpression,
                         Expression.Convert(right.PathValueExpression, left.PathValueType)
                        , left.PathValueType);
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
