using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Data.FilterExpressions;
using YS.Knife.Data.FilterExpressions.Converters;

namespace YS.Knife.Data
{
    public static class TypeExtensions
    {
        public static Type GetEnumerableSubType(this Type enumableType)
        {
            var subType = enumableType.GetInterfaces()
                .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(p => p.GetGenericArguments().First()).FirstOrDefault();
            if (subType == null)
            {
                throw new InvalidOperationException($"Can not get subtype from type '{enumableType.FullName}'.");
            }
            return subType;
        }

        public static bool IsNullableType(this Type type)
        {
            return type != null && Nullable.GetUnderlyingType(type) != null;
        }

        public static (bool IsNullbale, Type UnderlyingType) GetUnderlyingTypeTypeInfo(this Type type)
        {
            return type.IsNullableType() ? (true, Nullable.GetUnderlyingType(type)) : (false, type);
        }



    }

    public static class FilterInfoExtensions
    {

        internal static Expression VisitExpression(this PropertyInfo propertyInfo, Expression p)
        {
            var left = Expression.Property(p, propertyInfo);
            if (propertyInfo.PropertyType.IsNullableType())
                left = Expression.Property(left, nameof(Nullable<int>.Value));
            return left;
        }

        public static IQueryable<T> WhereCondition<T>(this IQueryable<T> source, FilterInfo filterInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            if (filterInfo == null) return source;
            return source.Where(filterInfo.CreatePredicate<T>());
        }

        public static IQueryable<T> WhereCondition<T>(this IQueryable<T> source, params FilterInfo[] filterInfos)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            foreach (var item in filterInfos ?? Enumerable.Empty<FilterInfo>())
            {
                source = WhereCondition(source, item);
            }

            return source;
        }

        public static Expression<Func<T, bool>> CreatePredicate<T>(this FilterInfo filterInfo)
        {
            _ = filterInfo ?? throw new ArgumentNullException(nameof(filterInfo));
            var p = Expression.Parameter(typeof(T), "p");
            var res = FromContidtionInternal(typeof(T), filterInfo, p);
            return Expression.Lambda<Func<T, bool>>(res, p);
        }

        public static LambdaExpression CreatePredicate(this FilterInfo filterInfo, Type type)
        {
            _ = filterInfo ?? throw new ArgumentNullException(nameof(filterInfo));
            var p = Expression.Parameter(type, "p");
            var res = FromContidtionInternal(type, filterInfo, p);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(type, typeof(bool)), res, p);
        }

        private static Expression FromOrConditionInternal(Type entityType, FilterInfo orCondition,
            ParameterExpression p)
        {
            if (orCondition == null) throw new ArgumentNullException(nameof(orCondition));
            Expression current = Expression.Constant(false);
            foreach (FilterInfo item in orCondition.Items)
            {
                var next = FromContidtionInternal(entityType, item, p);
                current = Expression.OrElse(current, next);
            }

            return current;
        }

        private static Expression FromAndConditionInternal(Type entityType, FilterInfo andCondition,
            ParameterExpression p)
        {
            if (andCondition == null) throw new ArgumentNullException(nameof(andCondition));
            Expression current = Expression.Constant(true);
            foreach (FilterInfo item in andCondition.Items)
            {
                var next = FromContidtionInternal(entityType, item, p);
                current = Expression.AndAlso(current, next);
            }

            return current;
        }

        private static Expression FromItemConditionItemInternal(Type entityType, FilterInfo singleItem,
            ParameterExpression p)
        {
            if (singleItem == null) throw new ArgumentNullException(nameof(singleItem));
            if (string.IsNullOrEmpty(singleItem.FieldName)) throw new ArgumentException("FieldName必须填充");
            var paths = singleItem.FieldName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            PropertyInfo pinfo;
            Type ty = entityType;
            Expression exp = p;
            for (int i = 0; i < paths.Length - 1; i++)
            {
                pinfo = ty.GetProperty(paths[i]);
                if (pinfo == null)
                    throw new ArgumentException($"在类型{ty.FullName}中无法找到指定的属性{paths[i]}");
                ty = pinfo.PropertyType;
                exp = Expression.Property(exp, pinfo);
            }

            pinfo = ty.GetProperty(paths.Last());
            if (pinfo == null)
                throw new ArgumentException($"在类型{ty.FullName}中无法找到指定的属性{paths.Last()}");

            var converter = GetConvertByFilterType(singleItem.FilterType);
            var val = singleItem.Value;
            if (val == DBNull.Value) val = null; //忽略dbnull.value
            return converter.ConvertValue(exp, pinfo, val, singleItem.Items);
        }

        private static Expression FromContidtionInternal(Type entityType, FilterInfo filterInfo, ParameterExpression p)
        {
            return filterInfo.OpType switch
            {
                OpType.AndItems => FromAndConditionInternal(entityType, filterInfo, p),
                OpType.OrItems => FromOrConditionInternal(entityType, filterInfo, p),
                _ => FromItemConditionItemInternal(entityType, filterInfo, p)
            };
        }

        private static ExpressionConverter GetConvertByFilterType(FilterType searchType)
        {
            var instance = CreateConverterInstance(searchType);
            instance.FilterType = searchType;
            return instance;
        }

        private static readonly Dictionary<FilterType, ExpressionConverter> AllConverters =
            Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                    !type.IsAbstract && typeof(ExpressionConverter).IsAssignableFrom(type) &&
                    Attribute.IsDefined(type, typeof(FilterConverterAttribute)))
                // ReSharper disable once PossibleNullReferenceException
                .ToDictionary(p => p.GetCustomAttribute<FilterConverterAttribute>().FilterType,
                    p =>
                    {
                        var converter = Activator.CreateInstance(p) as ExpressionConverter;
                        // ReSharper disable once PossibleNullReferenceException
                        converter.FilterType = p.GetCustomAttribute<FilterConverterAttribute>().FilterType;
                        return converter;
                    });

        private static ExpressionConverter CreateConverterInstance(FilterType filterType)
        {
            if (AllConverters.TryGetValue(filterType, out ExpressionConverter converter))
            {
                return converter;
            }
            throw new NotSupportedException($"FilterType {filterType} not supported.");
        }
    }
}
