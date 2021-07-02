using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(Operator.Between)]
    internal class BetweenExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", Operator.Between));
            if (!(value is Array)) throw new InvalidOperationException(string.Format("{0} 值必须为数组", Operator.Between));
            Array arr = value as Array;
            if (arr.Rank != 1 && arr.Length != 2) throw new InvalidOperationException(string.Format("{0} 值必须为长度为2的一维数组", Operator.Between));
            var firstvalue = arr.GetValue(arr.GetLowerBound(0));
            var lastvalue = arr.GetValue(arr.GetLowerBound(0) + 1);
            if (firstvalue == null) throw new InvalidOperationException(string.Format("{0}的起始值不能为null", Operator.Between));
            if (lastvalue == null) throw new InvalidOperationException(string.Format("{0}的结束值不能为null", Operator.Between));
            var isnullabletype = propInfo.PropertyType.IsNullableType();
            var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
            {
                throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", Operator.Between, ptype.FullName));
            }
            var propExpression = Expression.Property(p, propInfo);
            if (isnullabletype)
                propExpression = Expression.Property(propExpression, "Value");
            var left = Expression.GreaterThanOrEqual(Expression.Call(
                propExpression,
                ptype.GetMethod("CompareTo", new Type[] { ptype }),
                Expression.Constant(this.ChangeType(firstvalue, ptype))), Expression.Constant(0));
            var right = Expression.LessThanOrEqual(Expression.Call(
                propExpression,
                ptype.GetMethod("CompareTo", new Type[] { ptype }),
                Expression.Constant(this.ChangeType(lastvalue, ptype))), Expression.Constant(0));
            return Expression.AndAlso(left, right);
        }
    }
}
