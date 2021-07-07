using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter
{
    public class FilterValueDesc
    {
        public object ConstValue { get; set; }

        public Type PathValueType { get; set; }

        public Expression PathValueExpression { get; set; }

        public bool IsConstValue { get => PathValueExpression == null; }

        public Expression GetExpression(Type targetType)
        {
            if (IsConstValue)
            {
                return GetConstValueExpression(targetType);
            }
            else
            {
                return GetExpressionValueExpression(targetType);
            }
        }
        private Expression GetConstValueExpression(Type targetType)
        {
            if (targetType.IsNullableType() && ConstValue is null)
            {
                return Expression.Constant(ConstValue, targetType);
            }
            return Expression.Constant(ChangeType(ConstValue, targetType), targetType);
        }
        private Expression GetExpressionValueExpression(Type targetType)
        {
            if (PathValueType == targetType)
            {
                return PathValueExpression;
            }
            else
            {
                return Expression.Convert(PathValueExpression, targetType);
            }
        }

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
