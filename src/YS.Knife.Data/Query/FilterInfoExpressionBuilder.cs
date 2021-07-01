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

        public static FilterInfoExpressionBuilder Default = new FilterInfoExpressionBuilder();

        public Expression<Func<TSource, bool>> CreateSourceFilterExpression<TSource, TTarget>(
           ObjectMapper<TSource, TTarget> mapper, FilterInfo2 targetFilter)
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
        internal LambdaExpression CreateSourceFilterExpression(
           IObjectMapper mapper, FilterInfo2 targetFilter)
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var p = Expression.Parameter(mapper.SourceType, "p");

            if (targetFilter == null)
            {
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(mapper.SourceType, typeof(bool)), Expression.Constant(true), p);
            }
           

            var memberProvider = IFilterMemberInfoProvider.GetMapperProvider(mapper);
            var exp = FromSingleItemFilterInfo(targetFilter, p, memberProvider);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(mapper.SourceType, typeof(bool)), exp, p);
        }

        public Expression<Func<TTarget, bool>> CreateFilterExpression<TTarget>(
            FilterInfo2 targetFilter)
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
        public LambdaExpression CreateFilterExpression(Type targetType,
            FilterInfo2 targetFilter)
        {
            var p = Expression.Parameter(targetType, "p");

            if (targetFilter == null)
            {
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(targetType, typeof(bool)), Expression.Constant(true), p);
            }
            var memberProvider = IFilterMemberInfoProvider.GetObjectProvider(targetType);
            var exp = FromSingleItemFilterInfo(targetFilter, p, memberProvider);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(targetType, typeof(bool)), exp, p);

        }


        private IFilterValueContext CreateValueExpression(ParameterExpression p, IFilterMemberInfoProvider memberProvider, ValueInfo valueInfo)
        {
            if (valueInfo == null || valueInfo.IsValue)
            {
                // const value
                return CreateConstValueExpression(valueInfo?.Value);
            }
            else
            {
                return CreateExpressionValueExression(valueInfo.Segments);
            }
            IFilterValueContext CreateConstValueExpression(object value)
            {
                return new ConstFilterValueContext
                {
                    ExpressionValueType = value?.GetType(),
                    CurrentExpression = Expression.Constant(value),
                };
            }
            IFilterValueContext CreateExpressionValueExression(List<NameInfo> pathInfos)
            {

                IFilterMemberInfoProvider currentMemberProvider = memberProvider;
                Type currentExpressionType = currentMemberProvider.CurrentType;
                Expression currentExpression = p;
                foreach (var pathInfo in pathInfos)
                {
                    if (pathInfo.IsFunction)
                    {
                        // TODO ..
                        var functionContext = new FunctionContext();

                        var function = FilterFunction.GetFunctionByName(pathInfo.Name);
                        if (function == null)
                        {
                            throw Errors.NotSupportFunction(pathInfo.Name);
                        }
                        //TODO apply function
                        var functionResult = function.Execute(new FunctionContext
                        {
                            //FromType = context.ExpressionValueType,
                            //Args = singleItem.Function.Args,
                            //FieldNames = singleItem.Function.FieldNames,
                            //SubFilter = singleItem.Function.SubFilter
                        });
                        currentExpressionType = functionResult.LambdaValueType;
                        currentExpression = currentExpression.Connect(functionResult.LambdaExpression);
                        currentMemberProvider = functionResult.MemberProvider;
                    }
                    else
                    {
                        var memberInfo = currentMemberProvider.GetSubMemberInfo(pathInfo.Name);

                        if (memberInfo == null)
                        {
                            throw Errors.InvalidMemberNameInFieldName(pathInfo.Name);
                        }
                        else
                        {
                            currentExpressionType = memberInfo.ExpressionValueType;
                            currentExpression = currentExpression.Connect(memberInfo.SelectExpression);
                            currentMemberProvider = memberInfo.SubProvider;
                        }
                    }
                }
                return new ConstFilterValueContext
                {
                    ExpressionValueType = currentExpressionType,
                    CurrentExpression = currentExpression
                };
            }
        }
        
        private Expression FromSingleItemFilterInfo(FilterInfo2 singleItem,
            ParameterExpression p, IFilterMemberInfoProvider memberProvider)
        {
            var left = CreateValueExpression(p, memberProvider, singleItem.Left);
            var right = CreateValueExpression(p, memberProvider, singleItem.Right);
            return CompareFilterValue(left, singleItem.FilterType, right);
        }
        private Expression CompareFilterValue(IFilterValueContext left , FilterType filterType,
         IFilterValueContext right)
        {
            if (right.ExpressionValueType != left.ExpressionValueType)
            {
                return Expression.Equal(left.CurrentExpression, Expression.Convert( right.CurrentExpression, left.ExpressionValueType));
            }
            return Expression.Equal(left.CurrentExpression, right.CurrentExpression);
        }


        interface IFilterValueContext
        {
            public Type ExpressionValueType { get; }

            public Expression CurrentExpression { get; }
        }
        class ConstFilterValueContext : IFilterValueContext
        {
            public Type ExpressionValueType { get; set; }

            public Expression CurrentExpression { get; set; }
        }

        class FilterExpressionContext : List<FilerExpressionSegment>, IFilterValueContext
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

        public interface IFilterMemberInfo
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
       public  interface IFilterMemberInfoProvider
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
            public static Exception InvalidMemberNameInFieldName(string memberName)
            {
                return new FieldInfo2ExpressionException($"Invalid member name '{memberName}'.");
            }
            public static Exception NotSupportedFieldName(string name)
            {
                return new FieldInfo2ExpressionException($"Not supported field name '{name}' in filter info.");
            }
            public static Exception NotSupportFunction(string functionName)
            {
                return new FieldInfo2ExpressionException($"Not support function '{functionName}'."); ;
            }
            public static FieldInfo2ExpressionException OnlyCanUseFunctionInCollectionType(string fullField)
            {
                return new FieldInfo2ExpressionException("Only can use function in collection type");
            }
        }
    }
}
