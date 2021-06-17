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
        class ObjectMapperLoader<TSource,TTarget>
            where TSource : class
            where TTarget : class, new()
        {
            private readonly ObjectMapper<TSource, TTarget> _mapper;
            private static readonly Dictionary<string, MethodInfo> AllAppendMethods = typeof(ObjectMapper<TSource, TTarget>)
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
                    var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
                    var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
                    if (CanMapNullable(targetItemType, sourceItemType))
                    {
                        AppendEnumerableNullableAssign(targetProperty,sourceProperty,targetItemType,sourceItemType);
                    }
                    else if (CanAssignableFrom(targetItemType, sourceItemType))
                    {
                        AppendEnumerableAssign(targetProperty, sourceProperty, targetItemType, sourceItemType);
                    }
                    else if (CanMapNewComplexObject(targetItemType, sourceItemType))
                    {
                        AppendEnumerableNewComplexObject(targetProperty, sourceProperty,targetItemType,sourceItemType);
                    }
                }
                else
                {
                    if (CanMapNullable(targetProperty.PropertyType, sourceProperty.PropertyType))
                    {
                        AppendNullableProperty(targetProperty,sourceProperty);
                    }
                    else if (CanAssignableFrom(targetProperty.PropertyType, sourceProperty.PropertyType))
                    {
                        AppendProperty(targetProperty,sourceProperty);
                    }
                    else if (CanMapNewComplexObject(targetProperty.PropertyType, sourceProperty.PropertyType))
                    {
                        AppendComplexObject(targetProperty,sourceProperty);
                    }
                }
            }

            private static bool IsCollectionMap(PropertyInfo targetProperty, PropertyInfo sourceProperty)
            {
                if (targetProperty.PropertyType == typeof(string)|| sourceProperty.PropertyType==typeof(string))
                {
                    return false;
                }

                return EnumerableTypeUtils.IsEnumerable(targetProperty.PropertyType) &&
                       EnumerableTypeUtils.IsEnumerable(sourceProperty.PropertyType);
            }

            private static bool CanAssignableFrom(Type targetType, Type sourceType)
            {
                return targetType.IsAssignableFrom(sourceType);
            }

            private static bool CanMapNullable(Type targetType, Type sourceType)
            {
                return targetType!=sourceType &&  
                       sourceType.IsValueType && 
                       !sourceType.IsNullableType() && 
                       targetType.IsNullableType() && 
                       targetType ==typeof(Nullable<>).MakeGenericType(sourceType);
            }

            private bool CanMapNewComplexObject(Type targetType, Type sourceType)
            {
                return Type.GetTypeCode(targetType) == TypeCode.Object
                    && Type.GetTypeCode(sourceType) == TypeCode.Object
               && targetType.GetConstructor(Type.EmptyTypes) != null;
            }
            private void AppendNullableProperty(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = AllAppendMethods["append_nullable_property"];
                var genericMethod = method.MakeGenericMethod( sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda});
            }
            private void AppendProperty(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = AllAppendMethods["append_property"];
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType, sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda});
            }

            private void AppendComplexObject(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = AllAppendMethods["append_new_object"];
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType, sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                var innerMapper =
                    DefaultObjectMapperFactory.CreateDefault(sourceProperty.PropertyType, targetProperty.PropertyType);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda,innerMapper});
            }
            private void AppendEnumerableNewComplexObject(PropertyInfo targetProperty,
                PropertyInfo sourceProperty,Type targetItemType,Type sourceItemType)
            {
                var method = AllAppendMethods["append_enumerable_new_object"];
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType,
                    targetItemType,
                    sourceProperty.PropertyType,
                    sourceItemType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                var innerMapper =
                    DefaultObjectMapperFactory.CreateDefault(sourceItemType,targetItemType);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda,innerMapper});
            }
            private void AppendEnumerableNullableAssign(PropertyInfo targetProperty,
                PropertyInfo sourceProperty,Type targetItemType,Type sourceItemType)
            {
                var method = AllAppendMethods["append_enumerable_nullable_assign"];

                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType,
                    
                    sourceProperty.PropertyType,
                    sourceItemType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda});
            }
            
            private void AppendEnumerableAssign(PropertyInfo targetProperty,
                PropertyInfo sourceProperty,Type targetItemType,Type sourceItemType)
            {
                var method = AllAppendMethods["append_enumerable_assign"];

                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType,
                    targetItemType,
                    sourceProperty.PropertyType,
                    sourceItemType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                genericMethod.Invoke(this._mapper, new object[] {targetLambda, sourceLambda});
            }
            
            private LambdaExpression CreateSourceLambda(PropertyInfo sourceProperty)
            {
                var paramExp = Expression.Parameter(typeof(TSource));
                var propertyExp = Expression.Property(paramExp, sourceProperty);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
            }

            private LambdaExpression CreateTargetLambda(PropertyInfo targetProperty)
            {
                var paramExp = Expression.Parameter(typeof(TTarget));
                var propertyExp = Expression.Property(paramExp, targetProperty);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TTarget), targetProperty.PropertyType), propertyExp, paramExp);
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
            ObjectMapper<TSource,TTarget> mapper = new ObjectMapper<TSource, TTarget>();
            ObjectMapperLoader<TSource,TTarget> loader = new ObjectMapperLoader<TSource, TTarget>(mapper);
            var targetPropertyMap = typeof(TTarget).GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
            var sourcePropertyMap = typeof(TSource).GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);
            
            foreach (var prop in targetPropertyMap)
            {
                if (sourcePropertyMap.TryGetValue(prop.Key, out var sourceProperty))
                {
                    loader.LoadPropertyMapper(prop.Value,sourceProperty);
                }
            }
            return mapper;
        }

    }
}
