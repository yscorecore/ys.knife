using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(FilterType.Exists)]
    internal class ExistsExpressionConverter : OpenExpressionConverter
    {
        private static readonly MethodInfo AnyMethod0 =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(p => p.Name == nameof(Enumerable.Any) && p.GetParameters().Length == 1);
        private static readonly MethodInfo AnyMethod1 =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(p => p.Name == nameof(Enumerable.Any) && p.GetParameters().Length == 2);
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            var trimFilters = subFilters.TrimNotNull().ToList();
            if (trimFilters.Count == 0)
            {
                var (_, propType) = propInfo.PropertyType.GetUnderlyingTypeTypeInfo();
                var subType = propType.GetEnumerableSubType();
                return Expression.Call(AnyMethod0.MakeGenericMethod(subType), Expression.Property(p, propInfo));
            }
            if (trimFilters.Count == 1)
            {
                return CreateAnyMethod1Expression(p, propInfo, trimFilters.First());
            }
            else
            {

                return CreateAnyMethod1Expression(p, propInfo, FilterInfo.CreateAnd(trimFilters.ToArray()));
            }
        }


        private static Expression CreateAnyMethod1Expression(Expression p, PropertyInfo propInfo, FilterInfo filterInfo)
        {
            var (_, propType) = propInfo.PropertyType.GetUnderlyingTypeTypeInfo();
            var subType = propType.GetEnumerableSubType();
            var innerExpression = filterInfo.CreatePredicate(subType);
            var propExpression = Expression.Property(p, propInfo);
            return Expression.Call(AnyMethod1.MakeGenericMethod(subType), propExpression, innerExpression);
        }
    }
}
