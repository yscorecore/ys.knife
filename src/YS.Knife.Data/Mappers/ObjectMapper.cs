using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.Mappers
{
    public sealed class ObjectMapper<TSource, TTarget> : IObjectMapper
        where TSource : class
        where TTarget : class, new()
    {
        public static ObjectMapper<TSource, TTarget> Default { get; } =
            DefaultObjectMapperFactory.CreateDefault<TSource, TTarget>();


        private IDictionary<string, IMapperExpression> PropMappers
        {
            get { return this._propMappers; }
        }

        public Type SourceType => typeof(TSource);

        public Type TargetType => typeof(TTarget);

        private readonly Dictionary<string, IMapperExpression> _propMappers =
            new Dictionary<string, IMapperExpression>();
        private Expression<Func<TSource, TTarget>> _cachedExpression = null;
        private Func<TSource, TTarget> _cachedFunc = null;

        LambdaExpression IObjectMapper.BuildExpression() => this.BuildExpression();
        Delegate IObjectMapper.BuildConvertFunc() => this.BuildConvertFunc();

        IMapperExpression IObjectMapper.GetFieldExpression(string targetField, StringComparison stringComparison)
        {
            foreach (var kv in this._propMappers)
            {
                if (string.Equals(kv.Key, targetField, stringComparison))
                {
                    return kv.Value;
                }
            }
            return null;
        }

        public ObjectMapper<TSource, TTarget> Pick(string[] targetFields) =>
            Pick(targetFields, StringComparer.InvariantCultureIgnoreCase);
        public ObjectMapper<TSource, TTarget> Pick(string[] targetFields, StringComparer stringComparer)
        {
            // cache result
            var mapper = new ObjectMapper<TSource, TTarget>();
            var allKeys = this._propMappers.Keys.Where(p =>
                (targetFields ?? Array.Empty<string>()).Contains(p, stringComparer)).ToArray();
            foreach (var targetField in allKeys)
            {
                mapper._propMappers[targetField] = this._propMappers[targetField];
            }
            return mapper;
        }


        #region Core Methods
        public Expression<Func<TSource, TTarget>> BuildExpression()
        {
            if (_cachedExpression == null)
            {
                _cachedExpression = GetExpressionInternal();
            }
            return _cachedExpression;
        }

        public Func<TSource, TTarget> BuildConvertFunc()
        {
            if (_cachedFunc == null)
            {
                _cachedFunc = this.BuildExpression().Compile();
            }
            return _cachedFunc;
        }
        #endregion

        #region Append Methods
        [Description(DefaultObjectMapperFactory.AppendProperty)]
        public void Append<TTargetValue, TSourceValue>(Expression<Func<TTarget, TTargetValue>> targetMember,
            Expression<Func<TSource, TSourceValue>> sourceExpression)
            where TSourceValue : TTargetValue
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));

            var (memberName, memberType) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = FromPropertyMapperExpression<TSourceValue, TTargetValue>.Create(sourceExpression);
            this.DirtyCache();
        }
        [Description(DefaultObjectMapperFactory.AppendNewObject)]
        public void Append<TTargetObject, TSourceObject>(Expression<Func<TTarget, TTargetObject>> targetMember, Expression<Func<TSource, TSourceObject>> sourceExpression, ObjectMapper<TSourceObject, TTargetObject> mapper)
            where TTargetObject : class, new()
            where TSourceObject : class
        {
            var (memberName, _) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = FromNewComplexObjectMapperExpression<TSourceObject, TTargetObject>.Create(sourceExpression, mapper);
            this.DirtyCache();
        }
        [Description(DefaultObjectMapperFactory.AppendEnumerableNewObject)]
        public void AppendCollection<TTargetValueItem, TSourceValueItem>(Expression<Func<TTarget, IEnumerable<TTargetValueItem>>> targetMember, Expression<Func<TSource, IEnumerable<TSourceValueItem>>> sourceExpression, ObjectMapper<TSourceValueItem, TTargetValueItem> mapper)
            where TTargetValueItem : class, new()
            where TSourceValueItem : class
        {
            var (memberName, targetActualType) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromEnumerableNewObjectMapperExpression<TSourceValueItem, TTargetValueItem>(sourceExpression, targetActualType, mapper);
            this.DirtyCache();
        }
        [Description(DefaultObjectMapperFactory.AppendQueryableNewObject)]
        public void AppendCollection<TTargetValueItem, TSourceValueItem>(Expression<Func<TTarget, IEnumerable<TTargetValueItem>>> targetMember, Expression<Func<TSource, IQueryable<TSourceValueItem>>> sourceExpression, ObjectMapper<TSourceValueItem, TTargetValueItem> mapper)
            where TTargetValueItem : class, new()
            where TSourceValueItem : class
        {
            var (memberName, targetActualType) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromEnumerableNewObjectMapperExpression<TSourceValueItem, TTargetValueItem>(sourceExpression, targetActualType, mapper);
            this.DirtyCache();
        }

        [Description(DefaultObjectMapperFactory.AppendEnumerablePropertyAssign)]
        public void AppendCollection<TTargetValueItem, TSourceValueItem>(Expression<Func<TTarget, IEnumerable<TTargetValueItem>>> targetMember, Expression<Func<TSource, IEnumerable<TSourceValueItem>>> sourceExpression)
            where TSourceValueItem : TTargetValueItem
        {
            var (memberName, targetActualType) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromEnumerableAssignMapperExpression<TSourceValueItem, TTargetValueItem>(sourceExpression, targetActualType);
            this.DirtyCache();
        }
        [Description(DefaultObjectMapperFactory.AppendQueryablePropertyAssign)]
        public void AppendCollection<TTargetValueItem, TSourceValueItem>(Expression<Func<TTarget, IEnumerable<TTargetValueItem>>> targetMember, Expression<Func<TSource, IQueryable<TSourceValueItem>>> sourceExpression)
            where TSourceValueItem : TTargetValueItem
        {
            var (memberName, targetActualType) = PickTargetMemberInfo(targetMember);
            this.PropMappers[memberName] = new FromEnumerableAssignMapperExpression<TSourceValueItem, TTargetValueItem>(sourceExpression, targetActualType);
            this.DirtyCache();
        }
        #endregion






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
            return Expression.Bind(memberInfo!, sourceExpression.GetBindExpression().ReplaceFirstParam(p));
        }

        private void DirtyCache()
        {
            this._cachedExpression = null;
            this._cachedFunc = null;
        }
    }
}
