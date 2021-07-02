using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(Operator.All)]
    internal class AllExpressionConverter : OpenExpressionConverter
    {
        private static readonly MethodInfo AllMethod1 =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(p =>
                p.Name == nameof(Enumerable.All) && p.GetParameters().Length == 2);

        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            var singleFilterInfo = GetOneFilterInfo(subFilters);
            return CreateAllMethod1Expression(p, propInfo, singleFilterInfo);
        }

        private FilterInfo GetOneFilterInfo(List<FilterInfo> filterInfos)
        {
            var trimItems = filterInfos.TrimNotNull().ToArray();
            return trimItems.Length switch
            {
                0 => null,
                1 => trimItems.First(),
                _ => FilterInfo.CreateAnd(trimItems)
            };
        }

        private static Expression CreateAllMethod1Expression(Expression p, PropertyInfo propInfo, FilterInfo filterInfo)
        {
            var (_, propType) = propInfo.PropertyType.GetUnderlyingTypeTypeInfo();
            var subType = propType.GetEnumerableSubType();
            var innerExpression = filterInfo == null
                ? Expression.Constant(true) as Expression
                : filterInfo.CreatePredicate(subType);
            var propExpression = Expression.Property(p, propInfo);
            return Expression.Call(AllMethod1.MakeGenericMethod(subType), propExpression, innerExpression);
        }
    }
}
