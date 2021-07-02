using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(Operator.NotContains)]
    internal class NotContainsExpressionConverter : ContainsExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            return Expression.Not(base.ConvertValue(p, propInfo, value, subFilters));
        }
    }
}
