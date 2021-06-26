using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(FilterType.Equals)]
    internal class EqualsExpressionConverter : ExpressionConverter
    {
        private Expression ConvertNullValue(Expression p, PropertyInfo propInfo)
        {
            if (propInfo.PropertyType.IsNullableType())
            {
                var left = Expression.Property(p, propInfo);
                var right = Expression.Convert(Expression.Constant(null), propInfo.PropertyType);
                return Expression.Equal(left, right);
            }
            else
            {
                if (propInfo.PropertyType.IsValueType)
                {
                    throw new FieldInfo2ExpressionException($"Can not convert null value to '{propInfo.PropertyType.FullName}' type");
                }
                else
                {
                    var left = Expression.Property(p, propInfo);
                    var right = Expression.Constant(null);
                    return Expression.Equal(left, right);
                }
            }
        }
        private Expression ConvertNotNullValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (propInfo.PropertyType.IsNullableType())
            {
                var type = Nullable.GetUnderlyingType(propInfo.PropertyType);
                var left = Expression.Property(p, propInfo);
                var right = Expression.Convert(Expression.Constant(this.ChangeType(value, type)), propInfo.PropertyType);
                return Expression.Equal(left, right);
            }
            else
            {
                var left = Expression.Property(p, propInfo);
                var right = Expression.Constant(this.ChangeType(value, propInfo.PropertyType));
                return Expression.Equal(left, right);
            }
        }
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            if (value == null)
            {
                return ConvertNullValue(p, propInfo);
            }
            else
            {
                return ConvertNotNullValue(p, propInfo, value);
            }
        }
    }
}
