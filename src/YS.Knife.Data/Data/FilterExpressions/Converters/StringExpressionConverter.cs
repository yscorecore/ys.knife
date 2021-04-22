using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{

    internal abstract class StringExpressionConverter : ExpressionConverter
    {
        protected abstract string MethodName { get; }
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            if (IsNull(value)) throw new InvalidOperationException(string.Format("{0} 无法处理null值", this.FilterType));
            if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", this.FilterType));
            string val = ChangeToString(value);
            return Expression.AndAlso(
                Expression.NotEqual(Expression.Property(p, propInfo), Expression.Constant(null)),
                Expression.Call(
                    Expression.Property(p, propInfo),
                    typeof(string).GetMethod(MethodName, new Type[] { typeof(string) }),
                    Expression.Constant(val)
                ));
        }
    }
}
