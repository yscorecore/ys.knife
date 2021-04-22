using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(FilterType.All)]
    internal class AllExpressionConverter : OpenExpressionConverter
    {
        private static readonly MethodInfo AllMethod1 =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(p => p.Name == "All" && p.GetParameters().Count() == 2);

        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {

            if (subFilters is null || subFilters.Count == 0)
            {
                return CreateAllMethod1Expression(p, propInfo, null);
            }

            if (subFilters.Count == 1)
            {
                return CreateAllMethod1Expression(p, propInfo, subFilters.First());
            }

            return CreateAllMethod1Expression(p, propInfo, FilterInfo.CreateAnd(subFilters.ToArray()));
        }
        private static Expression CreateAllMethod1Expression(Expression p, PropertyInfo propInfo, FilterInfo filterInfo)
        {
            var (_, propType) = propInfo.PropertyType.GetUnderlyingTypeTypeInfo();
            var subType = propType.GetEnumerableSubType();
            var innerExpression = filterInfo == null ? Expression.Constant(true) as Expression : filterInfo.CreatePredicate(subType);
            var propExpression = Expression.Property(p, propInfo);
            return Expression.Call(AllMethod1.MakeGenericMethod(subType), propExpression, innerExpression);
        }
    }
}
