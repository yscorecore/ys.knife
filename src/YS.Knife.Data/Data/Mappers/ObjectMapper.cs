using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.Mappers
{

    public class ObjectMapper<TSource, TTarget>
        where TSource: class
        where TTarget :class, new()

    {
        public static ObjectMapper<TSource, TTarget> Default { get; } = new ObjectMapper<TSource, TTarget>();

        public virtual IDictionary<string, IMapperExpression> PropMappers
        {
            get { return this.propMappers; }
        }

        private readonly Dictionary<string, IMapperExpression> propMappers =
            new Dictionary<string, IMapperExpression>();
        private Expression<Func<TSource, TTarget>> cachedExpression = null;
        private Func<TSource, TTarget> cachedFunc = null;

        public ObjectMapper(bool loadDefaultMapperExpressions=false)
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
        public void AppendProperty<TValue>(Expression<Func<TTarget, TValue>> targetMember,
            Expression<Func<TSource, TValue>> sourceExpression)
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            AssertSimpleMemberType(typeof(TValue));
            var memberName = PickTargetMemberName(targetMember);
            this.PropMappers[memberName] = new PropertyMapperExpression(sourceExpression);
            this.DirtyCache();
        }
        public void AppendObject<TTargetObject, TSourceObject>(Expression<Func<TTarget, TTargetObject>> targetMember, Expression<Func<TSource, TSourceObject>> sourceExpression, ObjectMapper<TSourceObject, TTargetObject> mapper)
            where TTargetObject : class, new()
            where TSourceObject : class
        {
            var memberName = PickTargetMemberName(targetMember);
            this.PropMappers[memberName] = new ComplexObjectMapperExpression<TSourceObject, TTargetObject>(sourceExpression, mapper);
            this.DirtyCache();
        }
        public void AppendObject<TObject>(Expression<Func<TTarget, TObject>> targetMember, Expression<Func<TSource, TObject>> sourceExpression)
           where TObject : class, new()
        {
            // clone new object
            AppendObject(targetMember, sourceExpression, ObjectMapper<TObject, TObject>.Default);
        }
        public void AppendCollection<TTargetObject, TSourceObject>(Expression<Func<TTarget, IEnumerable<TTargetObject>>> targetMember, Expression<Func<TSource, IQueryable<TSourceObject>>> sourceExpression, ObjectMapper<TSourceObject, TTargetObject> mapper)
             where TTargetObject : class, new()
            where TSourceObject : class, new()
        {
            var (memberName, type) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new CollectionMapperExpression<TSourceObject, TTargetObject>(sourceExpression, mapper, type);
            this.DirtyCache();
        }
        public void AppendCollection<TObject>(Expression<Func<TTarget, IEnumerable<TObject>>> targetMember, Expression<Func<TSource, IQueryable<TObject>>> sourceExpression)
            where TObject : class, new()
        {
            this.AppendCollection(targetMember, sourceExpression, ObjectMapper<TObject, TObject>.Default);
        }
        public void AppendCollection<TTargetObject, TSourceObject>(Expression<Func<TTarget, IEnumerable<TTargetObject>>> targetMember, Expression<Func<TSource, IEnumerable<TSourceObject>>> sourceExpression, ObjectMapper<TSourceObject, TTargetObject> mapper)
            where TTargetObject : class, new()
           where TSourceObject : class, new()
        {
            var (memberName, type) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new CollectionMapperExpression<TSourceObject, TTargetObject>(sourceExpression, mapper, type);
            this.DirtyCache();
        }
        public void AppendCollection<TObject>(Expression<Func<TTarget, IEnumerable<TObject>>> targetMember, Expression<Func<TSource, IEnumerable<TObject>>> sourceExpression)
            where TObject : class, new()
        {
            this.AppendCollection(targetMember, sourceExpression, ObjectMapper<TObject, TObject>.Default);
        }
        #endregion

        #region LoadDefault
        private void LoadDefaultMapperExpressions()
        {
            var targetPropertyMap = typeof(TTarget).GetProperties().Where(p=>p.CanWrite).ToDictionary(p=>p.Name,p=>p);
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
            if (IsSimpleType(targetProperty.PropertyType))
            {
                LoadSimplePropertyMapper(targetProperty, sourceProperty);
            }
            else if (IsEnumerableType(targetProperty.PropertyType))
            {
                // TODO ..
            }
            else
            { 
                // TODO ...
            }
        }

        private void LoadSimplePropertyMapper(PropertyInfo targetProperty, PropertyInfo sourceProperty)
        {
            if (targetProperty.PropertyType != sourceProperty.PropertyType) return;
            var paramExp = Expression.Parameter(typeof(TSource));
            var propertyExp =  Expression.Property(paramExp, sourceProperty);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TSource), sourceProperty.PropertyType), propertyExp, paramExp);
            this.PropMappers[targetProperty.Name] = new PropertyMapperExpression(lambda);
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

        private static bool IsSimpleType(Type type)
        {
            var mainType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
            return Type.GetTypeCode(mainType) != TypeCode.Object;
        }
        private static bool IsEnumerableType(Type type)
        {
            return false;
        }
        private static bool IsComplexObjectType(Type type)
        {
            return false;
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
            throw new NotSupportedException($"unknow member {targetMember}.");
        }
        private static string PickTargetMemberName<TValue>(Expression<Func<TTarget, TValue>> targetMember)
        {
            var (name, _) = PickTargetMemberInfo(targetMember);
            return name;
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
