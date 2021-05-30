using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.Mappers
{

    public class ObjectMapper<TSource, TTarget>
        where TSource : class
        where TTarget : class, new()

    {
        public static ObjectMapper<TSource, TTarget> Default { get; } = new ObjectMapper<TSource, TTarget>(true);

        public virtual IDictionary<string, IMapperExpression> PropMappers
        {
            get { return this.propMappers; }
        }

        private readonly Dictionary<string, IMapperExpression> propMappers =
            new Dictionary<string, IMapperExpression>();
        private Expression<Func<TSource, TTarget>> cachedExpression = null;
        private Func<TSource, TTarget> cachedFunc = null;
        public ObjectMapper(bool loadDefaultMapperExpressions = false)
        {
            if (loadDefaultMapperExpressions)
            {
                this.LoadDefaultMapperExpressions();
            }
        }
        #region Interface Methods
        public Expression<Func<TSource, TTarget>> BuildExpression()
        {
            if (cachedExpression == null)
            {
                cachedExpression = GetExpressionInternal();
            }
            return cachedExpression;
        }
        public Func<TSource, TTarget> BuildConvertFunc()
        {
            if (cachedFunc == null)
            {
                cachedFunc = this.BuildExpression().Compile();
            }
            return cachedFunc;
        }
        #endregion

        #region Append Methods
        public void AppendProperty<TValue>(Expression<Func<TTarget, TValue?>> targetMember,
            Expression<Func<TSource, TValue>> sourceExpression)
            where TValue : struct
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            var (memberName, memberType) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromNullablePropertyMapperExpression<TSource, TValue>(sourceExpression);
            this.DirtyCache();

        }


        public void AppendProperty<TTargetValue, TSourceValue>(Expression<Func<TTarget, TTargetValue>> targetMember,
            Expression<Func<TSource, TSourceValue>> sourceExpression)
            where TSourceValue : TTargetValue
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));

            var (memberName, memberType) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromPropertyMapperExpression(sourceExpression, typeof(TSourceValue));
            this.DirtyCache();

        }
        public void AppendObject<TTargetObject, TSourceObject>(Expression<Func<TTarget, TTargetObject>> targetMember, Expression<Func<TSource, TSourceObject>> sourceExpression, ObjectMapper<TSourceObject, TTargetObject> mapper)
            where TTargetObject : class, new()
            where TSourceObject : class
        {
            var (memberName, _) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromNewComplexObjectMapperExpression<TSourceObject, TTargetObject>(sourceExpression, mapper);
            this.DirtyCache();
        }

        public void AppendCollection<TTargetObject, TSourceObject>(Expression<Func<TTarget, IEnumerable<TTargetObject>>> targetMember, Expression<Func<TSource, IQueryable<TSourceObject>>> sourceExpression, ObjectMapper<TSourceObject, TTargetObject> mapper)
             where TTargetObject : class, new()
            where TSourceObject : class
        {
            var (memberName, type) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromQueryableNewObjectMapperExpression<TSource, TSourceObject, TTargetObject>(sourceExpression, mapper, type);
            this.DirtyCache();
        }
        public void AppendCollection<TTargetObject, TSourceObject>(Expression<Func<TTarget, IEnumerable<TTargetObject>>> targetMember, Expression<Func<TSource, IQueryable<TSourceObject>>> sourceExpression)
            where TSourceObject : TTargetObject
        {
            var (memberName, type) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromQueryableAssignMapperExpression<TSource, TSourceObject, TTargetObject>(sourceExpression, type);
            this.DirtyCache();
        }


        public void AppendCollection<TTargetObject, TSourceObject>(Expression<Func<TTarget, IEnumerable<TTargetObject>>> targetMember, Expression<Func<TSource, IEnumerable<TSourceObject>>> sourceExpression, ObjectMapper<TSourceObject, TTargetObject> mapper)
            where TTargetObject : class, new()
           where TSourceObject : class
        {
            var (memberName, type) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromEnumerableNewObjectMapperExpression<TSource, TSourceObject, TTargetObject>(sourceExpression, mapper, type);
            this.DirtyCache();
        }
        public void AppendCollection<TTargetObject, TSourceObject>(Expression<Func<TTarget, IEnumerable<TTargetObject>>> targetMember, Expression<Func<TSource, IEnumerable<TSourceObject>>> sourceExpression)
                where TTargetObject : class
              where TSourceObject : class, TTargetObject
        {
            var (memberName, type) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromEnumerableAssignMapperExpression<TSource, TSourceObject, TTargetObject>(sourceExpression, type);
            this.DirtyCache();
        }
        #endregion

        #region LoadDefault
        private void LoadDefaultMapperExpressions()
        {
            var targetPropertyMap = typeof(TTarget).GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
            var sourcePropertyMap = typeof(TSource).GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);

            foreach (var prop in targetPropertyMap)
            {
                if (sourcePropertyMap.TryGetValue(prop.Key, out var sourceProperty))
                {
                    LoadPropertyMapper(prop.Value, sourceProperty);
                }
            }
        }
        private void LoadPropertyMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            if (targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
            {
                LoadAssignableFromTargetPropertyMapper(targetProperty, sourceProperty);
            }
            else if (CanConvertValueTypeToNullableType(targetProperty, sourceProperty))
            {
                LoadConvertToTargetPropertyMapper(targetProperty, sourceProperty);
            }
            else if (CanMapQueryableAssignObject(targetProperty, sourceProperty))
            {
                LoadQueryableAssignMapper(targetProperty, sourceProperty);
            }
            else if (CanMapEnumerableAssignObject(targetProperty, sourceProperty))
            {
                LoadEnumerableAssignMapper(targetProperty, sourceProperty);
            }
            else if (CanMapQueryablePropertyBindingObject(targetProperty, sourceProperty))
            {
                LoadQueryablePropertyBindingMapper(targetProperty, sourceProperty);
            }
            else if (CanMapEnumerablePropertyBindingObject(targetProperty, sourceProperty))
            {
                LoadEnumerablePropertyBindingMapper(targetProperty, sourceProperty);
            }
            else if (CanMapComplexObject(targetProperty, sourceProperty))
            {
                LoadComplexObjectMapper(targetProperty, sourceProperty);
            }
        }

        private void LoadQueryableAssignMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);

            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp = Expression.Property(paramExp, sourceProperty);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);

            var mapperExpressionType = typeof(FromQueryableAssignMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);
            var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, targetProperty.PropertyType });
            this.PropMappers[targetProperty.Name] = instance as IMapperExpression;

        }
        private void LoadEnumerableAssignMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);

            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp = Expression.Property(paramExp, sourceProperty);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);

            var mapperExpressionType = typeof(FromEnumerableAssignMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);
            var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, targetProperty.PropertyType });
            this.PropMappers[targetProperty.Name] = instance as IMapperExpression;

        }
        private object NewMapper(Type target, Type source)
        {
            var type = typeof(ObjectMapper<,>).MakeGenericType(target, source);
            return Activator.CreateInstance(type, new object[] { true });

        }
        private void LoadQueryablePropertyBindingMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);

            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp = Expression.Property(paramExp, sourceProperty);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);

            var mapperExpressionType = typeof(FromQueryableNewObjectMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);

            var subMapper = NewMapper(targetItemType, sourceItemType);
            var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, subMapper, targetProperty.PropertyType });
            this.PropMappers[targetProperty.Name] = instance as IMapperExpression;
        }

        private void LoadEnumerablePropertyBindingMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);

            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp = Expression.Property(paramExp, sourceProperty);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);

            var mapperExpressionType = typeof(FromEnumerableNewObjectMapperExpression<,,>).MakeGenericType(typeof(TSource), sourceItemType, targetItemType);

            var subMapper = NewMapper(targetItemType, sourceItemType);
            var instance = Activator.CreateInstance(mapperExpressionType, new object[] { lambda, subMapper, targetProperty.PropertyType });
            this.PropMappers[targetProperty.Name] = instance as IMapperExpression;
        }

        private void LoadAssignableFromTargetPropertyMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp = Expression.Property(paramExp, sourceProperty);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
            this.PropMappers[targetProperty.Name] = new FromPropertyMapperExpression(lambda, sourceProperty.PropertyType);
        }
        private bool CanConvertValueTypeToNullableType(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            return sourceProperty.PropertyType.IsValueType && targetProperty.PropertyType == typeof(Nullable<>).MakeGenericType(sourceProperty.PropertyType);
        }
        private void LoadConvertToTargetPropertyMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp = Expression.Property(paramExp, sourceProperty);
            var convertExp = Expression.Convert(propertyExp, targetProperty.PropertyType);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), typeof(Nullable<>).MakeGenericType(sourceProperty.PropertyType)), convertExp, paramExp);
            this.PropMappers[targetProperty.Name] = new FromPropertyMapperExpression(lambda, sourceProperty.PropertyType);
        }
        private bool CanMapComplexObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            return Type.GetTypeCode(targetProperty.PropertyType) == TypeCode.Object
                && Type.GetTypeCode(sourceProperty.PropertyType) == TypeCode.Object
           && targetProperty.PropertyType.GetConstructor(Type.EmptyTypes) != null;
        }
        private void LoadComplexObjectMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {

            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp = Expression.Property(paramExp, sourceProperty);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
            var mapperType = typeof(FromNewComplexObjectMapperExpression<,>).MakeGenericType(sourceProperty.PropertyType, targetProperty.PropertyType);

            var innerMapperType = typeof(ObjectMapper<,>).MakeGenericType(sourceProperty.PropertyType, targetProperty.PropertyType);
            var innerMapperInstance = Activator.CreateInstance(innerMapperType, new object[] { true });
            var mapperInstance = Activator.CreateInstance(mapperType, lambda, innerMapperInstance);
            this.PropMappers[targetProperty.Name] = mapperInstance as IMapperExpression;
        }
        private bool CanMapQueryableAssignObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);
            return targetItemType != null && sourceItemType != null
                && targetItemType.IsAssignableFrom(sourceItemType);
        }
        private bool CanMapEnumerableAssignObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
            return targetItemType != null && sourceItemType != null
                 && targetItemType.IsAssignableFrom(sourceItemType);
        }
        private bool CanMapEnumerablePropertyBindingObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetEnumerableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(sourceProperty.PropertyType);
            return targetItemType != null && sourceItemType != null
                && Type.GetTypeCode(targetItemType) == TypeCode.Object
                && Type.GetTypeCode(sourceItemType) == TypeCode.Object
                && targetItemType.GetConstructor(Type.EmptyTypes) != null;
        }
        private bool CanMapQueryablePropertyBindingObject(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            var targetItemType = EnumerableTypeUtils.GetQueryableItemType(targetProperty.PropertyType);
            var sourceItemType = EnumerableTypeUtils.GetQueryableItemType(sourceProperty.PropertyType);
            return targetItemType != null && sourceItemType != null
                   && Type.GetTypeCode(targetItemType) == TypeCode.Object
                   && Type.GetTypeCode(sourceItemType) == TypeCode.Object
                   && targetItemType.GetConstructor(Type.EmptyTypes) != null;
        }
        #endregion

        private static void AssertSimpleMemberType(Type type)
        {
            var mainType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
            if (Type.GetTypeCode(mainType) == TypeCode.Object)
            {
                throw new InvalidOperationException($"{nameof(AppendProperty)} only support simple member type, '{type.FullName}' is not supported.");
            }
        }



        private static (string Name, Type Type) PickTargetMemberInfo<TValue>(Expression<Func<TTarget, TValue>> targetMember)
        {
            _ = targetMember ?? throw new ArgumentNullException(nameof(targetMember));
            if (targetMember.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new InvalidOperationException($"can not resolve target member from expression '{targetMember}'.");
            }
            var memberAccess = targetMember.Body as MemberExpression;
            AssertMemberCanWrite(memberAccess.Member);
            var memberInfo = memberAccess.Member;
            if (memberInfo is PropertyInfo prop)
            {
                return (prop.Name, prop.PropertyType);
            }
            if (memberInfo is FieldInfo field)
            {
                return (field.Name, field.FieldType);
            }
            throw new InvalidOperationException($"unknow member {targetMember}.");
        }

        private static void AssertMemberCanWrite(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo prop)
            {
                if (!prop.CanWrite)
                {
                    throw new InvalidOperationException($"target member '{memberInfo.Name}' can not write.");
                }
            }
        }

        private Expression<Func<TSource, TTarget>> GetExpressionInternal()
        {
            var p = Expression.Parameter(typeof(TSource));
            var memberBindings = this.PropMappers.Select(kv => CreateMemberBinding(kv.Key, kv.Value, p)).ToArray();
            var expressions = Expression.MemberInit(Expression.New(typeof(TTarget).GetConstructor(Type.EmptyTypes)!),
                memberBindings);
            // 处理为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(p, Expression.Constant(null))
                , Expression.Constant(null, typeof(TTarget)), expressions);
            return Expression.Lambda<Func<TSource, TTarget>>(resultExpression, p);
        }

        private MemberBinding CreateMemberBinding(string targetName, IMapperExpression sourceExpression,
            ParameterExpression p)
        {
            var memberInfo = typeof(TTarget).GetProperty(targetName) as MemberInfo ??
                             typeof(TTarget).GetField(targetName);
            return Expression.Bind(memberInfo!, sourceExpression.GetLambdaExpression().ReplaceFirstParam(p));
        }
        protected virtual void DirtyCache()
        {
            this.cachedExpression = null;
            this.cachedFunc = null;
        }
    }
}
