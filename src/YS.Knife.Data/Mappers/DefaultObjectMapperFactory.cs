using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.Mappers
{
    public class DefaultObjectMapperFactory
    {
        internal const string AppendQueryablePropertyAssign = "append_queryable_assign";
        internal const string AppendEnumerablePropertyAssign = "append_enumerable_assign";
        internal const string AppendQueryableNewObject = "append_queryable_new_object";
        internal const string AppendEnumerableNewObject = "append_enumerable_new_object";
        internal const string AppendNewObject = "append_new_object";
        internal const string AppendProperty = "append_property";

        class ObjectMapperLoader<TSource, TTarget>
            where TSource : class
            where TTarget : class, new()
        {
            private readonly ObjectMapper<TSource, TTarget> _mapper;

            private static readonly Dictionary<string, MethodInfo> AllAppendMethods =
                typeof(ObjectMapper<TSource, TTarget>)
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(method => method.IsDefined(typeof(DescriptionAttribute)))
                    .ToDictionary(method => method.GetCustomAttribute<DescriptionAttribute>().Description);

            public ObjectMapperLoader(ObjectMapper<TSource, TTarget> mapper)
            {
                this._mapper = mapper;
            }

            public void LoadPropertyMapper(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                if (IsCollectionMap(targetProperty, sourceProperty))
                {
                    var targetItemType = targetProperty.PropertyType.GetEnumerableItemType();
                    var sourceItemType = sourceProperty.PropertyType.GetEnumerableItemType();
                    if (CanAssignableFrom(targetItemType, sourceItemType))
                    {
                        AppendEnumerableAssign(targetProperty, sourceProperty, targetItemType, sourceItemType);
                    }
                    else if (CanMapNewComplexObject(targetItemType, sourceItemType))
                    {
                        AppendEnumerableNewComplexObject(targetProperty, sourceProperty, targetItemType,
                            sourceItemType);
                    }
                }
                else
                {
                    if (CanAssignableFrom(targetProperty.PropertyType, sourceProperty.PropertyType))
                    {
                        AppendPropertyAssign(targetProperty, sourceProperty);
                    }
                    else if (CanMapNewComplexObject(targetProperty.PropertyType, sourceProperty.PropertyType))
                    {
                        AppendComplexObject(targetProperty, sourceProperty);
                    }
                }
            }

            private static bool IsCollectionMap(PropertyInfo targetProperty, PropertyInfo sourceProperty)
            {
                if (targetProperty.PropertyType == typeof(string) || sourceProperty.PropertyType == typeof(string))
                {
                    return false;
                }

                return targetProperty.PropertyType.IsEnumerable() &&
                      sourceProperty.PropertyType.IsEnumerable();
            }

            private static bool CanAssignableFrom(Type targetType, Type sourceType)
            {
                if (targetType == sourceType) return true;
                return Type.GetTypeCode(targetType) == Type.GetTypeCode(sourceType) &&
                       targetType.IsAssignableFrom(sourceType);
            }

            private bool CanMapNewComplexObject(Type targetType, Type sourceType)
            {
                return Type.GetTypeCode(targetType) == TypeCode.Object
                       && Type.GetTypeCode(sourceType) == TypeCode.Object
                       && targetType.GetConstructor(Type.EmptyTypes) != null;
            }

            private void AppendPropertyAssign(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = AllAppendMethods[AppendProperty];
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType, sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda});
            }

            private void AppendComplexObject(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = AllAppendMethods[AppendNewObject];
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType, sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                var innerMapper =
                    CreateDefault(sourceProperty.PropertyType, targetProperty.PropertyType);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda, innerMapper});
            }

            private void AppendEnumerableNewComplexObject(PropertyInfo targetProperty,
                PropertyInfo sourceProperty, Type targetItemType, Type sourceItemType)
            {
                
                var sourceIsQueryable =sourceProperty.PropertyType.IsQueryable();
                var method = sourceIsQueryable
                    ? AllAppendMethods[AppendQueryableNewObject]
                    : AllAppendMethods[AppendEnumerableNewObject];
                
                var targetLambdaResultType = typeof(IEnumerable<>).MakeGenericType(targetItemType);
                var sourceLambdaResultType = sourceIsQueryable
                    ? typeof(IQueryable<>).MakeGenericType(sourceItemType)
                    : typeof(IEnumerable<>).MakeGenericType(sourceItemType);
                var genericMethod = method.MakeGenericMethod(
                    targetItemType,
                    
                    sourceItemType);
                var targetLambda = CreateTargetLambda(targetProperty,targetLambdaResultType);
                var sourceLambda = CreateSourceLambda(sourceProperty,sourceLambdaResultType);
                var innerMapper =
                    CreateDefault(sourceItemType, targetItemType);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda, innerMapper});
            }


            private void AppendEnumerableAssign(PropertyInfo targetProperty,
                PropertyInfo sourceProperty, Type targetItemType, Type sourceItemType)
            {
                var sourceIsQueryable =sourceProperty.PropertyType.IsQueryable();
                var method = sourceIsQueryable
                    ? AllAppendMethods[AppendQueryablePropertyAssign]
                    : AllAppendMethods[AppendEnumerablePropertyAssign];
                var targetLambdaResultType = typeof(IEnumerable<>).MakeGenericType(targetItemType);
                var sourceLambdaResultType = sourceIsQueryable
                    ? typeof(IQueryable<>).MakeGenericType(sourceItemType)
                    : typeof(IEnumerable<>).MakeGenericType(sourceItemType);

                var genericMethod = method.MakeGenericMethod(
                    targetItemType,
             
                    sourceItemType);
                var targetLambda = CreateTargetLambda(targetProperty,targetLambdaResultType);
                var sourceLambda = CreateSourceLambda(sourceProperty,sourceLambdaResultType);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda});
            }

            private LambdaExpression CreateSourceLambda(PropertyInfo sourceProperty)
            {
                return  CreateSourceLambda(sourceProperty, sourceProperty.PropertyType);
            }
            private LambdaExpression CreateSourceLambda(PropertyInfo sourceProperty,Type lambdaResultType)
            {
                var paramExp = Expression.Parameter(typeof(TSource));
                var propertyExp = Expression.Property(paramExp, sourceProperty);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), lambdaResultType),
                    propertyExp, paramExp);
            }

            private LambdaExpression CreateTargetLambda(PropertyInfo targetProperty)
            {
                return CreateTargetLambda(targetProperty, targetProperty.PropertyType);
            }
            private LambdaExpression CreateTargetLambda(PropertyInfo targetProperty,Type lambdaResultType)
            {
                var paramExp = Expression.Parameter(typeof(TTarget));
                var propertyExp = Expression.Property(paramExp, targetProperty);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TTarget), lambdaResultType),
                    propertyExp, paramExp);
            }
        }

        private static readonly MethodInfo CreateDefaultMethod =
            typeof(DefaultObjectMapperFactory).GetMethod(nameof(CreateDefault),
                BindingFlags.Static | BindingFlags.Public);

        internal static object CreateDefault(Type source, Type target)
        {
            return CreateDefaultMethod.MakeGenericMethod(source, target).Invoke(null, Array.Empty<object>());
        }

        public static ObjectMapper<TSource, TTarget> CreateDefault<TSource, TTarget>()
            where TSource : class
            where TTarget : class, new()
        {
            ObjectMapper<TSource, TTarget> mapper = new ObjectMapper<TSource, TTarget>();
            ObjectMapperLoader<TSource, TTarget> loader = new ObjectMapperLoader<TSource, TTarget>(mapper);
            var targetPropertyMap =
                typeof(TTarget).GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
            var sourcePropertyMap =
                typeof(TSource).GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);

            foreach (var prop in targetPropertyMap)
            {
                if (sourcePropertyMap.TryGetValue(prop.Key, out var sourceProperty))
                {
                    loader.LoadPropertyMapper(prop.Value, sourceProperty);
                }
            }

            return mapper;
        }
    }
}
