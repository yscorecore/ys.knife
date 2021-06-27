using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using YS.Knife.Data;
using YS.Knife.Data.Functions;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data
{
    public class FilterInfoExpressionBuilder
    {
        public static Regex ValidFieldNameRegex = new Regex("^\\w+[!\\?]?(\\.\\w+[!\\?]?)*$");

        public Expression<Func<TSource, bool>> CreateSourceFilterExpression<TSource, TTarget>(
           ObjectMapper<TSource, TTarget> mapper, FilterInfo targetFilter)
           where TSource : class
           where TTarget : class, new()
        {
            var p = Expression.Parameter(typeof(TSource), "p");

            if (targetFilter == null)
            {
                return Expression.Lambda<Func<TSource, bool>>(Expression.Constant(true), p);
            }
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));

            var memberProvider = IFilterMemberInfoProvider.GetMapperProvider(mapper);
            var exp = FromSingleItemFilterInfo(targetFilter, p, memberProvider);
            return Expression.Lambda<Func<TSource, bool>>(exp, p);
        }

        public Expression<Func<TTarget, bool>> CreateFilterExpression<TTarget>(
            FilterInfo targetFilter)
        {
            var p = Expression.Parameter(typeof(TTarget), "p");

            if (targetFilter == null)
            {
                return Expression.Lambda<Func<TTarget, bool>>(Expression.Constant(true), p);
            }
            var memberProvider = IFilterMemberInfoProvider.GetObjectProvider(typeof(TTarget));
            var exp = FromSingleItemFilterInfo(targetFilter, p, memberProvider);
            return Expression.Lambda<Func<TTarget, bool>>(exp, p);

        }

        private bool IsValidFieldName(string fieldName)
        {
            return !string.IsNullOrEmpty(fieldName) && ValidFieldNameRegex.IsMatch(fieldName);
        }

        private Expression FromSingleItemFilterInfo(FilterInfo singleItem,
            ParameterExpression p, IFilterMemberInfoProvider memberProvider)
        {
            if (!IsValidFieldName(singleItem.FieldName))
            {
                throw Errors.InvalidFieldName(singleItem.FieldName);
            }

            var context = new FilterExpressionContext();
            context.Add(new FilerExpressionSegment
            {
                MemberInfo = new FilterMemberInfo()
                {
                    ExpressionValueType = memberProvider.CurrentType,
                    SelectExpression = Expression.Lambda(p, p),
                    SubProvider = memberProvider
                },
                Expression = p,
                RequiredKind = FieldRequiredKind.None

            }); ;
            foreach (var field in ParseFilterNames(singleItem.FieldName))
            {
                var memberInfo = context.CurrentMemberProvider.GetSubMemberInfo(field.MemberName);

                if (memberInfo == null)
                {
                    throw Errors.InvalidMemberNameInFieldName(field.MemberName, singleItem.FieldName);
                }
                var currentExpression = context.CurrentExpression.Connect(memberInfo.SelectExpression);
                context.Add(new FilerExpressionSegment
                {
                    MemberInfo = memberInfo,
                    Expression = currentExpression,
                    RequiredKind = field.RequiredKind
                });
            }
            if (singleItem.Function != null)
            {
                //function
                if (!typeof(IEnumerable).IsAssignableFrom(context.ExpressionValueType))
                {
                    throw Errors.OnlyCanUseFunctionInCollectionType(singleItem.FieldName);
                }
                var function = FilterFunction.GetFunctionByName(singleItem.Function.Name);
                if (function == null)
                {
                    throw Errors.NotSupportFunction(singleItem.Function.Name, singleItem.FieldName);
                }
                //TODO apply function
                var functionResult = function.Execute(new FunctionContext
                {
                     FromType = context.ExpressionValueType
                });

                var currentExpression = context.CurrentExpression.Connect(functionResult.LambdaExpression);
                context.Add(new FilerExpressionSegment
                {
                    MemberInfo = new FilterMemberInfo()
                    {
                        SelectExpression = functionResult.LambdaExpression,
                        ExpressionValueType = functionResult.LambdaValueType,
                        SubProvider = IFilterMemberInfoProvider.GetObjectProvider(functionResult.LambdaValueType),
                    },
                    Expression = currentExpression,
                    RequiredKind = FieldRequiredKind.None
                });

            }

            return CompareFilterWithValue(context, singleItem.FilterType, singleItem.Value);
        }
        private IEnumerable<(string MemberName, FieldRequiredKind RequiredKind)> ParseFilterNames(string filterFieldName)
        {
            return filterFieldName.Split('.')
                 .Select(p =>
                 {
                     var requiredKind = p.EndsWith('?') ? FieldRequiredKind.Optional : (p.EndsWith('!') ? FieldRequiredKind.Must : FieldRequiredKind.None);
                     return (p.Trim(new char[] { '!', '?' }), requiredKind);
                 });
        }
        private Expression CompareFilterWithValue(FilterExpressionContext context, FilterType filterType,
           object value)
        {
            // a>3         a > 3
            // a?b > 3     a == null || a.b > 3
            // a!.b > 3     a != null && a.b >3   
            // a?.b?.c >3  a == null || (a.b == null || a.b.c >3) 
            // a!.b?.c > 3  a != null && (a.b == null or a.b.c >3)
            // a!.b!.c  >3   a != null && (a.b != null && a.b.c >3)
            // a?.b!.c > 3  a == null || (a.b != null && a.b.c >3)


            var compareExpression = Expression.Equal(context.CurrentExpression, Expression.Constant(Convert.ChangeType(value, context.ExpressionValueType)));

            return CombinNullCheckExpression(context, 0, compareExpression);
        }
        private Expression CombinNullCheckExpression(List<FilerExpressionSegment> segments, int index, Expression valueCompareExpression)
        {
            if (index == segments.Count - 1)
            {// last one 
                return valueCompareExpression;
            }
            var segment = segments[index];
            if (segment.RequiredKind == FieldRequiredKind.Optional)
            {// or
                if (IsValueType(segment.MemberInfo.ExpressionValueType))
                {
                    return CombinNullCheckExpression(segments, index + 1, valueCompareExpression);
                }
                else
                {
                    return Expression.OrElse(
                           Expression.Equal(segment.Expression, Expression.Constant(null))
                        , CombinNullCheckExpression(segments, index + 1, valueCompareExpression));
                }
            }
            else if (segment.RequiredKind == FieldRequiredKind.Must)
            { // and
                if (IsValueType(segment.MemberInfo.ExpressionValueType))
                {
                    return CombinNullCheckExpression(segments, index + 1, valueCompareExpression);
                }
                else
                {
                    return Expression.AndAlso(
                           Expression.NotEqual(segment.Expression, Expression.Constant(null))
                        , CombinNullCheckExpression(segments, index + 1, valueCompareExpression));
                }
            }
            else
            {
                return CombinNullCheckExpression(segments, index + 1, valueCompareExpression);
            }
            bool IsValueType(Type type) => type.IsValueType && Nullable.GetUnderlyingType(type) != null;
        }

        class FilterExpressionContext : List<FilerExpressionSegment>
        {

            public FilerExpressionSegment Current { get => this.Last(); }

            public Type ExpressionValueType { get => Current.MemberInfo.ExpressionValueType; }

            public Expression CurrentExpression { get => Current.Expression; }

            public IFilterMemberInfoProvider CurrentMemberProvider { get => Current.MemberInfo.SubProvider; }
        }
        struct FilerExpressionSegment
        {
            public IFilterMemberInfo MemberInfo { get; set; }
            public Expression Expression { get; set; }
            public FieldRequiredKind RequiredKind { get; set; }
        }

        interface IFilterMemberInfo
        {

            public Type ExpressionValueType { get; }

            public LambdaExpression SelectExpression { get; }

            public IFilterMemberInfoProvider SubProvider { get; }


        }
        class FilterMemberInfo : IFilterMemberInfo
        {
            public Type ExpressionValueType { get; set; }

            public LambdaExpression SelectExpression { get; set; }

            public IFilterMemberInfoProvider SubProvider { get; set; }
        }
        interface IFilterMemberInfoProvider
        {
            static ConcurrentDictionary<Type, IFilterMemberInfoProvider> ObjectMemberProviderCache =
               new ConcurrentDictionary<Type, IFilterMemberInfoProvider>();
            public Type CurrentType { get; }
            public IFilterMemberInfo GetSubMemberInfo(string memberName);

            public static IFilterMemberInfoProvider GetObjectProvider(Type type)
            {
                return ObjectMemberProviderCache.GetOrAdd(type, (ty) =>
                {
                    var objectProviderType = typeof(ObjectMemberProvider<>).MakeGenericType(ty);
                    return Activator.CreateInstance(objectProviderType) as IFilterMemberInfoProvider;
                });
            }

            public static IFilterMemberInfoProvider GetMapperProvider(IObjectMapper objectMapper)
            {
                return new ObjectMapperProvider(objectMapper);
            }
        }

        class ObjectMemberProvider<T> : IFilterMemberInfoProvider
        {
            static Dictionary<string, IFilterMemberInfo> AllMembers = new Dictionary<string, IFilterMemberInfo>(StringComparer.InvariantCultureIgnoreCase);

            static ObjectMemberProvider()
            {
                // if some member name equal when ignore case, next will over the pre one
                foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {

                    AllMembers[field.Name] = new ObjectFieldFilterMemberInfo(typeof(T), field);
                }
                foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (property.GetIndexParameters().Length == 0)
                    {
                        AllMembers[property.Name] = new ObjectPropertyFilterMemberInfo(typeof(T), property);
                    }

                }
            }

            public Type CurrentType => typeof(T);

            public IFilterMemberInfo GetSubMemberInfo(string memberName)
            {
                if (AllMembers.TryGetValue(memberName, out var filterMember))
                {
                    return filterMember;
                }
                return default;
            }

            class ObjectPropertyFilterMemberInfo : IFilterMemberInfo
            {
                private readonly PropertyInfo propertyInfo;

                public ObjectPropertyFilterMemberInfo(Type hostType, PropertyInfo propertyInfo)
                {
                    this.propertyInfo = propertyInfo;
                    var param0 = Expression.Parameter(hostType);
                    this.SelectExpression = Expression.Lambda(Expression.Property(param0, propertyInfo), param0);
                }
                public Type ExpressionValueType => propertyInfo.PropertyType;

                public LambdaExpression SelectExpression { get; }

                public IFilterMemberInfoProvider SubProvider { get => IFilterMemberInfoProvider.GetObjectProvider(ExpressionValueType); }
            }
            class ObjectFieldFilterMemberInfo : IFilterMemberInfo
            {
                private readonly FieldInfo fieldInfo;

                public ObjectFieldFilterMemberInfo(Type hostType, FieldInfo fieldInfo)
                {
                    this.fieldInfo = fieldInfo;
                    var param0 = Expression.Parameter(hostType);
                    this.SelectExpression = Expression.Lambda(Expression.Field(param0, fieldInfo), param0);
                }
                public Type ExpressionValueType => fieldInfo.FieldType;

                public LambdaExpression SelectExpression { get; }

                public IFilterMemberInfoProvider SubProvider { get => IFilterMemberInfoProvider.GetObjectProvider(ExpressionValueType); }
            }
        }

        class ObjectMapperProvider : IFilterMemberInfoProvider
        {
            private readonly IObjectMapper objectMapper;

            public ObjectMapperProvider(IObjectMapper objectMapper)
            {
                this.objectMapper = objectMapper;
            }
            public Type CurrentType => objectMapper.SourceType;

            public IFilterMemberInfo GetSubMemberInfo(string memberName)
            {
                var fieldExpression = objectMapper.GetFieldExpression(memberName, StringComparison.InvariantCultureIgnoreCase);
                if (fieldExpression != null)
                {
                    return new MapperMemberInfo(fieldExpression);
                }
                return default;

            }
            class MapperMemberInfo : IFilterMemberInfo
            {
                private readonly IMapperExpression mapperExpression;

                public MapperMemberInfo(IMapperExpression mapperExpression)
                {
                    this.mapperExpression = mapperExpression;
                }

                public Type ExpressionValueType => mapperExpression.SourceValueType;

                public LambdaExpression SelectExpression => mapperExpression.SourceExpression;

                public IFilterMemberInfoProvider SubProvider
                {
                    get
                    {
                        if (mapperExpression.SubMapper != null)
                        {
                            return new ObjectMapperProvider(mapperExpression.SubMapper);
                        }
                        else
                        {
                            return IFilterMemberInfoProvider.GetObjectProvider(ExpressionValueType);
                        }

                    }
                }
            }
        }
        enum FieldRequiredKind
        {
            None,
            Must,
            Optional,
        }


        class Errors
        {

            public static Exception InvalidFieldName(string name)
            {
                return new FieldInfo2ExpressionException($"Invalid field name '{name}' in filter info.");
            }
            public static Exception InvalidMemberNameInFieldName(string memberName, string fullField)
            {
                return new FieldInfo2ExpressionException($"Invalid member name '{memberName}' in filter field '{fullField}'.");
            }
            public static Exception NotSupportedFieldName(string name)
            {
                return new FieldInfo2ExpressionException($"Not supported field name '{name}' in filter info.");
            }
            public static Exception NotSupportFunction(string functionName, string fullField)
            {
                return new FieldInfo2ExpressionException($"Not support function '{functionName}' in filter field {fullField}."); ;
            }
            public static FieldInfo2ExpressionException OnlyCanUseFunctionInCollectionType(string fullField)
            {
                return new FieldInfo2ExpressionException("Only can use function in collection type");
            }
        }
    }
}
