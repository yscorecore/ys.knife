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
        private static MethodInfo AnyMethod0 =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(p => p.Name == "Any" && p.GetParameters().Count() == 1);
        private static MethodInfo AnyMethod1 =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(p => p.Name == "Any" && p.GetParameters().Count() == 2);
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {

            if (subFilters is null || subFilters.Count == 0)
            {
                var (_, propType) = propInfo.PropertyType.GetUnderlyingTypeTypeInfo();
                var subType = propType.GetEnumerableSubType();
                return Expression.Call(AnyMethod0.MakeGenericMethod(subType), Expression.Property(p, propInfo));
            }
            else if (subFilters.Count == 1)
            {
                return CreateAnyMethod1Expression(p, propInfo, subFilters.First());
            }
            else
            {

                return CreateAnyMethod1Expression(p, propInfo, FilterInfo.CreateAnd(subFilters.ToArray()));
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
