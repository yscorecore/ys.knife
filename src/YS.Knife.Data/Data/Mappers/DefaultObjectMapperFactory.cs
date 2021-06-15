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
            private ObjectMapper<TSource, TTarget> mapper;
            private static Dictionary<string, MethodInfo> _allAppendMethods = typeof(ObjectMapper<TSource, TTarget>)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.IsDefined(typeof(DescriptionAttribute)))
                .ToDictionary(method => method.GetCustomAttribute<DescriptionAttribute>().Description);
           
            public ObjectMapperLoader(ObjectMapper<TSource, TTarget> mapper)
            {
                this.mapper = mapper;
            }

            public void LoadPropertyMapper(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                if (CanMapNullableProperty(targetProperty, sourceProperty))
                {
                    AppendNullableProperty(targetProperty,sourceProperty);
                }

                else if (targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    AppendProperty(targetProperty, sourceProperty);
                }
                else if (CanMapEnumerableNewComplexObject(targetProperty, sourceProperty))
                {
                    AppendEnumerableNewComplexObject(targetProperty, sourceProperty);
                }
                else if (CanMapNewComplexObject(targetProperty, sourceProperty))
                {
                    AppendComplexObject(targetProperty, sourceProperty);
                }
                
            }

            private static bool CanMapNullableProperty(PropertyInfo targetProperty, PropertyInfo sourceProperty)
            {
                return sourceProperty.PropertyType.IsValueType && targetProperty.PropertyType ==
                    typeof(Nullable<>).MakeGenericType(sourceProperty.PropertyType);
            }
            private bool CanMapEnumerableNewComplexObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
            {
                var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
                var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
                return targetItemType != null && sourceItemType != null
                       && Type.GetTypeCode(targetItemType) == TypeCode.Object
                       && Type.GetTypeCode(sourceItemType) == TypeCode.Object
                       && targetItemType.GetConstructor(Type.EmptyTypes) != null;
            }
            private bool CanMapNewComplexObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
            {
                return Type.GetTypeCode(targetProperty.PropertyType) == TypeCode.Object
                    && Type.GetTypeCode(sourceProperty.PropertyType) == TypeCode.Object
               && targetProperty.PropertyType.GetConstructor(Type.EmptyTypes) != null;
            }
            private void AppendNullableProperty(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = _allAppendMethods["append_nullable_property"];
                var genericMethod = method.MakeGenericMethod( sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                genericMethod.Invoke(this.mapper, new object[] {targetLambda, sourceLambda});
            }
            private void AppendProperty(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = _allAppendMethods["append_property"];
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType, sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                genericMethod.Invoke(this.mapper, new object[] {targetLambda, sourceLambda});
            }

            private void AppendComplexObject(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = _allAppendMethods["append_complex_object"];
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType, sourceProperty.PropertyType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                var innerMapper =
                    DefaultObjectMapperFactory.CreateDefault(sourceProperty.PropertyType, targetProperty.PropertyType);
                genericMethod.Invoke(this.mapper, new object[] {targetLambda, sourceLambda,innerMapper});
            }
            private void AppendEnumerableNewComplexObject(PropertyInfo targetProperty,
                PropertyInfo sourceProperty)
            {
                var method = _allAppendMethods["append_enumerable_new_object"];
                var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
                var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
                var genericMethod = method.MakeGenericMethod(targetProperty.PropertyType,
                    targetItemType,
                    sourceProperty.PropertyType,
                    sourceItemType);
                var targetLambda = CreateTargetLambda(targetProperty);
                var sourceLambda = CreateSourceLambda(sourceProperty);
                var innerMapper =
                    DefaultObjectMapperFactory.CreateDefault(sourceItemType,targetItemType);
                genericMethod.Invoke(this.mapper, new object[] {targetLambda, sourceLambda,innerMapper});
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
            return CreateDefaultMethod.MakeGenericMethod(source, target).Invoke(null,new object[0]);
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
        
        //         #region LoadDefault

        private static void LoadPropertyMapper<TSource, TTarget>(PropertyInfo targetProperty, PropertyInfo sourceProperty,ObjectMapper<TSource,TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            if (targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
            {
                LoadAssignableFromTargetPropertyMapper(targetProperty, sourceProperty,mapper);
            }
            // else if (CanConvertValueTypeToNullableType(targetProperty, sourceProperty))
            // {
            //     LoadConvertToTargetPropertyMapper(targetProperty, sourceProperty);
            // }
            // else if (CanMapQueryableAssignObject(targetProperty, sourceProperty))
            // {
            //     LoadQueryableAssignMapper(targetProperty, sourceProperty);
            // }
            // else if (CanMapEnumerableAssignObject(targetProperty, sourceProperty))
            // {
            //     LoadEnumerableAssignMapper(targetProperty, sourceProperty);
            // }
            // else if (CanMapQueryablePropertyBindingObject(targetProperty, sourceProperty))
            // {
            //     LoadQueryablePropertyBindingMapper(targetProperty, sourceProperty);
            // }
            // else if (CanMapEnumerablePropertyBindingObject(targetProperty, sourceProperty))
            // {
            //     LoadEnumerablePropertyBindingMapper(targetProperty, sourceProperty);
            // }
            // else if (CanMapComplexObject(targetProperty, sourceProperty))
            // {
            //     LoadComplexObjectMapper(targetProperty, sourceProperty);
            // }
            
            static void LoadAssignableFromTargetPropertyMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty,ObjectMapper<TSource,TTarget> mapper)
            {
                var paramExp = Expression.Parameter(typeof(TSource));
                var propertyExp = Expression.Property(paramExp, sourceProperty);
                var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
                // this.PropMappers[targetProperty.Name] = new FromPropertyMapperExpression(lambda, sourceProperty.PropertyType);
               // mapper.AppendProperty();
              
              // typeof(ObjectMapper<TSource,TTarget>).GetMethods(BindingFlags.Public| BindingFlags.Instance).Where(p=>p.)
            }
            
           
                
            
            
        }
       
        
        
        // private void LoadQueryableAssignMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
        //     var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);
        //     
        //     var paramExp = Expression.Parameter(typeof(TSource));
        //     var propertyExp = Expression.Property(paramExp, sourceProperty);
        //     var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
        //     
        //     var mapperExpressionType = typeof(FromQueryableAssignMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);
        //     var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, targetProperty.PropertyType });
        //     this.PropMappers[targetProperty.Name] = instance as IMapperExpression;
        //
        // }
        // private void LoadEnumerableAssignMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     // var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
        //     // var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
        //     //
        //     // var paramExp = Expression.Parameter(typeof(TSource));
        //     // var propertyExp = Expression.Property(paramExp, sourceProperty);
        //     // var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
        //     //
        //     // var mapperExpressionType = typeof(FromEnumerableAssignMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);
        //     // var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, targetProperty.PropertyType });
        //     // this.PropMappers[targetProperty.Name] = instance as IMapperExpression;
        //
        // }
        // private object NewMapper(Type target, Type source)
        // {
        //     var type = typeof(ObjectMapper<,>).MakeGenericType(target, source);
        //     return Activator.CreateInstance(type, new object[] { true });
        //
        // }
        // private void LoadQueryablePropertyBindingMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     // var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
        //     // var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);
        //     //
        //     // var paramExp = Expression.Parameter(typeof(TSource));
        //     // var propertyExp = Expression.Property(paramExp, sourceProperty);
        //     // var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
        //     //
        //     // var mapperExpressionType = typeof(FromQueryableNewObjectMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);
        //     //
        //     // var subMapper = NewMapper(targetItemType, sourceItemType);
        //     // var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, subMapper, targetProperty.PropertyType });
        //     // this.PropMappers[targetProperty.Name] = instance as IMapperExpression;
        // }
        //
        // private void LoadEnumerablePropertyBindingMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     // var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
        //     // var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);
        //     //
        //     // var paramExp = Expression.Parameter(typeof(TSource));
        //     // var propertyExp = Expression.Property(paramExp, sourceProperty);
        //     // var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
        //     //
        //     // var mapperExpressionType = typeof(FromEnumerableNewObjectMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);
        //     //
        //     // var subMapper = NewMapper(targetItemType, sourceItemType);
        //     // var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, subMapper, targetProperty.PropertyType });
        //     // this.PropMappers[targetProperty.Name] = instance as IMapperExpression;
        // }
        //

        // private bool CanConvertValueTypeToNullableType(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     return sourceProperty.PropertyType.IsValueType && targetProperty.PropertyType == typeof(Nullable<>).MakeGenericType(sourceProperty.PropertyType);
        // }
        // private void LoadConvertToTargetPropertyMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     var paramExp = Expression.Parameter(typeof(TSource));
        //     var propertyExp = Expression.Property(paramExp, sourceProperty);
        //     var convertExp = Expression.Convert(propertyExp, targetProperty.PropertyType);
        //     var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), typeof(Nullable<>).MakeGenericType(sourceProperty.PropertyType)), convertExp, paramExp);
        //    // this.PropMappers[targetProperty.Name] = new FromPropertyMapperExpression(lambda, sourceProperty.PropertyType);
        // }
        // private bool CanMapComplexObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     return Type.GetTypeCode(targetProperty.PropertyType) == TypeCode.Object
        //         && Type.GetTypeCode(sourceProperty.PropertyType) == TypeCode.Object
        //    && targetProperty.PropertyType.GetConstructor(Type.EmptyTypes) != null;
        // }
        // private void LoadComplexObjectMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //
        //     var paramExp = Expression.Parameter(typeof(TSource));
        //     var propertyExp = Expression.Property(paramExp, sourceProperty);
        //     var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
        //     var mapperType = typeof(FromNewComplexObjectMapperExpression<,>).MakeGenericType(sourceProperty.PropertyType, targetProperty.PropertyType);
        //
        //     var innerMapperType = typeof(ObjectMapper<,>).MakeGenericType(sourceProperty.PropertyType, targetProperty.PropertyType);
        //     var innerMapperInstance = Activator.CreateInstance(innerMapperType, new object[] { true });
        //     var mapperInstance = Activator.CreateInstance(mapperType, lambda, innerMapperInstance);
        //     this.PropMappers[targetProperty.Name] = mapperInstance as IMapperExpression;
        // }
        // private bool CanMapQueryableAssignObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
        //     var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);
        //     return targetItemType != null && sourceItemType != null
        //         && targetItemType.IsAssignableFrom(sourceItemType);
        // }
        // private bool CanMapEnumerableAssignObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
        //     var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
        //     return targetItemType != null && sourceItemType != null
        //          && targetItemType.IsAssignableFrom(sourceItemType);
        // }
        // private bool CanMapEnumerablePropertyBindingObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
        //     var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
        //     return targetItemType != null && sourceItemType != null
        //         && Type.GetTypeCode(targetItemType) == TypeCode.Object
        //         && Type.GetTypeCode(sourceItemType) == TypeCode.Object
        //         && targetItemType.GetConstructor(Type.EmptyTypes) != null;
        // }
        // private bool CanMapQueryablePropertyBindingObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        // {
        //     var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
        //     var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);
        //     return targetItemType != null && sourceItemType != null
        //            && Type.GetTypeCode(targetItemType) == TypeCode.Object
        //            && Type.GetTypeCode(sourceItemType) == TypeCode.Object
        //            && targetItemType.GetConstructor(Type.EmptyTypes) != null;
        // }
        // #endregion
        //
    }
}
