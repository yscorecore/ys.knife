﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Data.Expressions.Functions;
using YS.Knife.Data.Filter.Operators;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.Filter
{
    public class FilterInfoExpressionBuilder
    {
        internal static FilterInfoExpressionBuilder Default = new FilterInfoExpressionBuilder();

        public Expression<Func<TSource, bool>> CreateFilterLambdaExpression<TSource, TTarget>(
           ObjectMapper<TSource, TTarget> mapper, FilterInfo targetFilter)
           where TSource : class
           where TTarget : class, new()
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var memberExpressionProvider = IMemberExpressionProvider.GetMapperProvider(mapper);
            return CreateFilterLambdaExpression<TSource>(targetFilter, memberExpressionProvider);
        }
        
        public Expression<Func<T, bool>> CreateFilterLambdaExpression<T>(
            FilterInfo filter)
        {
            var memberExpressionProvider = IMemberExpressionProvider.GetObjectProvider(typeof(T));
            return CreateFilterLambdaExpression<T>(filter, memberExpressionProvider);

        }
        public Expression<Func<T, bool>> CreateFilterLambdaExpression<T>(FilterInfo filterInfo, IMemberExpressionProvider memberExpressionProvider)
        {
            var p = Expression.Parameter(typeof(T), "p");
            var expression = CreateFilterExpression(p, filterInfo, memberExpressionProvider);
            return Expression.Lambda<Func<T, bool>>(expression, p);
        }
        public LambdaExpression CreateFilterLambdaExpression(Type objectType, FilterInfo filterInfo, IMemberExpressionProvider memberExpressionProvider)
        {
            var p = Expression.Parameter(objectType, "p");
            var expression = CreateFilterExpression(p, filterInfo, memberExpressionProvider);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(objectType, typeof(bool)), expression, p);

        }
        internal Expression CreateFilterExpression(ParameterExpression p, FilterInfo filterInfo, IMemberExpressionProvider memberExpressionProvider)
        {
            if (filterInfo == null)
            {
                return Expression.Constant(true);
            }
            else
            {
                return CreateCombinGroupsFilterExpression(p, filterInfo, memberExpressionProvider);
            }
        }
        private Expression CreateCombinGroupsFilterExpression(ParameterExpression p, FilterInfo filterInfo, IMemberExpressionProvider memberExpressionProvider)
        {
            return filterInfo.OpType switch
            {
                CombinSymbol.AndItems => CreateAndConditionFilterExpression(p, filterInfo, memberExpressionProvider),
                CombinSymbol.OrItems => CreateOrConditionFilterExpression(p, filterInfo, memberExpressionProvider),
                _ => CreateSingleItemFilterExpression(p, filterInfo, memberExpressionProvider)
            };
        }
        private Expression CreateOrConditionFilterExpression(ParameterExpression p, FilterInfo orGroupFilterInfo, IMemberExpressionProvider memberExpressionProvider)
        {
            Expression current = Expression.Constant(false);
            foreach (FilterInfo item in orGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item, memberExpressionProvider);
                current = Expression.OrElse(current, next);
            }
            return current;
        }
        private Expression CreateAndConditionFilterExpression(ParameterExpression p, FilterInfo andGroupFilterInfo, IMemberExpressionProvider memberExpressionProvider)
        {

            Expression current = Expression.Constant(true);
            foreach (FilterInfo item in andGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item, memberExpressionProvider);
                current = Expression.AndAlso(current, next);
            }
            return current;
        }
        private Expression CreateSingleItemFilterExpression(ParameterExpression p, FilterInfo singleItemFilter
          , IMemberExpressionProvider memberExpressionProvider)
        {
            /*** 
            a.b.c = null    >  a==null || a.b==null || a.b.c == null
            a.b.c ="abc"   >  a!=null && a.b!=null && a.b.c = "abc"
            a.b.c != null    > a!=null && a.b!=null && a.b.c !=null
            a.b.c != "abc"  > a==null || a.b==null || a.b.c

            a.b.c ct "abc"  > a!=null && b!=null && a.b.c!=null && a.b.c ct "abc" 
            a.b.c nct "abc" > a==null || b==null || c==null || a.b.c nct "abc"

            a.b.c sw "abc"  > a!=null && b!=null && a.b.c!=null && a.b.c sw"abc" 
            a.b.c nsw "abc" > a==null || b==null || c==null || a.b.c nsw "abc"

            a.b.c sw "abc"  > a!=null && b!=null && a.b.c!=null && a.b.c sw"abc" 
            a.b.c nsw "abc" > a==null || b==null || c==null || a.b.c nsw "abc"

            a.b.c > 5  > a!=null && a.b!=null && a.b.c > 5
            a.b.c > null > false

            a.b.c in [1,2,3] > a!=null && b!=null && c!=null && [1,2,3] contains(a.b.c)
            a.b.c in [1,2,3,null] >  a==null || a.b==null || a.b.c==null || [1,2,3] contains(a.b.c)
            a.b.c not in [1,2,3] > a==null || a.b==null || a.b.c==null || [1,2,3] not contains(a.b.c)
            a.b.c not in [1,2,3,null] > a!=null || a.b!=null || a.b.c!=null || [1,2,3] not contains(a.b.c)



            a.b.c = c.d.e

            ((a=null || a.b =null || a.b.c =null) &&(c==null || c.d==null || c.d.e==null))
                or
            (a!=null && a.b!=null && c!=null && c.d!=null && a.b.c == c.d.e)

            a.b.c != c.d.e
	

             * 
             * */

            var left = CreateFilterValueDesc(p, memberExpressionProvider, singleItemFilter.Left);
            var right = CreateFilterValueDesc(p, memberExpressionProvider, singleItemFilter.Right);
            return IFilterOperator.CreateOperatorExpression(left, singleItemFilter.Operator, right);
        }

        public FilterValueDesc CreateFilterValueDesc(Expression p, IMemberExpressionProvider memberProvider, ValueInfo valueInfo)
        {
            if (valueInfo == null || valueInfo.IsConstant)
            {
                // const value
                return CreateConstValueExpression(valueInfo?.ConstantValue);
            }
            else
            {
                return CreatePathValueExpression(valueInfo.NavigatePaths);
            }
            FilterValueDesc CreateConstValueExpression(object value)
            {
                return new FilterValueDesc
                {
                    ConstValue = value
                };
            }
            FilterValueDesc CreatePathValueExpression(List<ValuePath> pathInfos)
            {

                IMemberExpressionProvider currentMemberProvider = memberProvider;
                Type currentExpressionType = currentMemberProvider.CurrentType;
                Expression currentExpression = p;
                foreach (var pathInfo in pathInfos)
                {
                    if (pathInfo.IsFunction)
                    {
                        var functionResult = IFilterFunction.ExecuteFunction(pathInfo.Name,pathInfo.FunctionArgs, new ExecuteContext
                        {
                            CurrentExpression = currentExpression,
                            MemberExpressionProvider = currentMemberProvider,
                            CurrentType = currentExpressionType,
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
                return new FilterValueDesc
                {
                    PathValueType = currentExpressionType,
                    PathValueExpression = currentExpression
                };
            }
        }









        public interface IFilterMemberInfo
        {

            public Type ExpressionValueType { get; }

            public LambdaExpression SelectExpression { get; }

            public IMemberExpressionProvider SubProvider { get; }


        }
        class FilterMemberInfo : IFilterMemberInfo
        {
            public Type ExpressionValueType { get; set; }

            public LambdaExpression SelectExpression { get; set; }

            public IMemberExpressionProvider SubProvider { get; set; }
        }
        public interface IMemberExpressionProvider
        {
            static ConcurrentDictionary<Type, IMemberExpressionProvider> ObjectMemberProviderCache =
               new ConcurrentDictionary<Type, IMemberExpressionProvider>();
            public Type CurrentType { get; }
            public IFilterMemberInfo GetSubMemberInfo(string memberName);

            public static IMemberExpressionProvider GetObjectProvider(Type type)
            {
                return ObjectMemberProviderCache.GetOrAdd(type, (ty) =>
                {
                    var objectProviderType = typeof(ObjectMemberProvider<>).MakeGenericType(ty);
                    return Activator.CreateInstance(objectProviderType) as IMemberExpressionProvider;
                });
            }

            public static IMemberExpressionProvider GetMapperProvider(IObjectMapper objectMapper)
            {
                return new ObjectMapperProvider(objectMapper);
            }
        }

        class ObjectMemberProvider<T> : IMemberExpressionProvider
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

                public IMemberExpressionProvider SubProvider { get => IMemberExpressionProvider.GetObjectProvider(ExpressionValueType); }
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

                public IMemberExpressionProvider SubProvider { get => IMemberExpressionProvider.GetObjectProvider(ExpressionValueType); }
            }
        }

        class ObjectMapperProvider : IMemberExpressionProvider
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

                public IMemberExpressionProvider SubProvider
                {
                    get
                    {
                        if (mapperExpression.SubMapper != null)
                        {
                            return new ObjectMapperProvider(mapperExpression.SubMapper);
                        }
                        else
                        {
                            return IMemberExpressionProvider.GetObjectProvider(ExpressionValueType);
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
           
        }
    }
}
