using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(Operator.In)]
    internal class InExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            if (value == null) throw new InvalidOperationException($"{FilterType} 无法处理null值");
            IEnumerable arr = value as IEnumerable;
            if (arr == null) throw new InvalidOperationException($"{FilterType} 值必须为可枚举类型");
            var isNullableType = propInfo.PropertyType.IsNullableType();
            var propType = isNullableType ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            var lst = Activator.CreateInstance(typeof(List<>).MakeGenericType(propInfo.PropertyType)) as IList;
            foreach (var obj in arr)
            {
                if (obj == null)
                {
                    if (isNullableType == false)
                    {
                        throw new InvalidCastException($"无法将null对象转换为{propInfo.PropertyType.FullName}类型");
                    }
                    else
                    {
                        lst.Add(null);
                    }
                }
                else
                {
                    lst.Add(this.ChangeType(obj, propType));
                }
            }
            if (lst.Count == 0) return Expression.Constant(false);
            var methods = lst.GetType().GetMethod("Contains", new Type[] { propInfo.PropertyType });
            return Expression.Call(Expression.Constant(lst), methods, Expression.Property(p, propInfo));
            //return null;
        }
    }
}
