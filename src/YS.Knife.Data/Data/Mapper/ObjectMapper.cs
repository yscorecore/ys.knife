using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.Mapper
{
    public class ObjectMapper
    {
        public ObjectMapper(Type fromType, Type toType)
        {
        }
    }
    public interface IMapperExpression
    {
        LambdaExpression GetLambdaExpression();
    }
    public class PropMapperExpression<TFrom, TValue> : IMapperExpression
    {
        private readonly Expression<Func<TFrom, TValue>> sourceExpression;

        public PropMapperExpression(Expression<Func<TFrom, TValue>> sourceExpression)
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            this.sourceExpression = sourceExpression;
        }
        public LambdaExpression GetLambdaExpression()
        {
            return this.sourceExpression;
        }
    }
    public class ComplexObjectMapperExpression<TSource, TTarget>: IMapperExpression
         where TSource : class, new()
        where TTarget : class, new()
    {
        private readonly ObjectMapper<TSource, TTarget> objectMapper;
        private readonly LambdaExpression sourceExpression;

        public ComplexObjectMapperExpression(LambdaExpression sourceExpression, ObjectMapper<TSource, TTarget> objectMapper)
        {
            this.objectMapper = objectMapper;
            this.sourceExpression = sourceExpression;
        }

        public LambdaExpression GetLambdaExpression()
        {
            var newObjectExpression = this.objectMapper.GetExpression();
            var expression = newObjectExpression.ReplaceFirstParam(this.sourceExpression.Body);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
                , Expression.Constant(null,typeof(TTarget)), expression);

            return  Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());
        }
    }


    public class ObjectMapper<TFrom, TTo> : ObjectMapper
        where TTo : new()
    {
        private readonly Dictionary<string, IMapperExpression> propMappers =
            new Dictionary<string, IMapperExpression>(StringComparer.InvariantCultureIgnoreCase);
        private Expression<Func<TFrom, TTo>> cachedExpression = null;
        private Func<TFrom, TTo> cachedFunc = null;


        public ObjectMapper() : base(typeof(TFrom), typeof(TTo))
        {
        }
        private ObjectMapper(IEnumerable<KeyValuePair<string, IMapperExpression>> props) : base(typeof(TFrom), typeof(TTo))
        {
            foreach (var kv in props)
            {
                this.propMappers[kv.Key] = kv.Value;
            }
        }
        public ObjectMapper<TFrom, TTo> LoadDefault()
        {
            return this;
        }

        public void AppendProperty<TValue>(Expression<Func<TTo, TValue>> targetProperty,
            Expression<Func<TFrom, TValue>> sourceExpression)
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            var memberName = PickTargetMemberName(targetProperty);
            this.propMappers[memberName] = new PropMapperExpression<TFrom, TValue>(sourceExpression);
            this.DirtyCache();
        }

        private static string PickTargetMemberName<TValue>(Expression<Func<TTo, TValue>> targetProperty)
        {
            _ = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            if (targetProperty.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new InvalidOperationException($"can not resolve target member name from expression '{targetProperty}'.");
            }
            var memberAccess = targetProperty.Body as MemberExpression;
            var memberName = memberAccess.Member.Name;
            return memberName;
        }

        public void AppendObject<TToValue, TFromValue>(Expression<Func<TTo, TToValue>> targetProperty, Expression<Func<TFrom, TFromValue>> sourceExpression, ObjectMapper<TFromValue, TToValue> mapper)
            where TToValue : class, new()
            where TFromValue :class, new()
        {
            var memberName = PickTargetMemberName(targetProperty);
            this.propMappers[memberName] = new ComplexObjectMapperExpression<TFromValue, TToValue>(sourceExpression, mapper);
            this.DirtyCache();
        }
        public void AppendCollection<TToValue, TFromValue>(Expression<Func<TTo, IEnumerable<TToValue>>> targetProperty, Expression<Func<TFrom, IEnumerable<TFromValue>>> sourceExpression, ObjectMapper<TFromValue, TToValue> mapper)
             where TToValue : new()
            where TFromValue : new()
        {

        }

        public void Ignore(Expression<Func<TTo, object>> targetProperty)
        {
            _ = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            if (targetProperty.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memberAccess = targetProperty.Body as MemberExpression;
                var memberName = memberAccess!.Member.Name;
                Ignore(memberName);
            }
        }

        public void Ignore(params string[] targetMembers)
        {
            Array.ForEach(targetMembers ?? new string[0], targetMember =>
            {
                if (this.propMappers.Remove(targetMember))
                {
                    this.DirtyCache();
                }
            });
        }

        public ObjectMapper<TFrom, TTo> PickSub(params string[] targetmembers)
        {
            //Dictionary<string>
            var targets = targetmembers.ToHashSet();

            var subProps = propMappers.Where(p => targets.Contains(p.Key, propMappers.Comparer));


            return new ObjectMapper<TFrom, TTo>(subProps);

        }

        public Expression<Func<TFrom, TTo>> GetExpression()
        {
            if (cachedExpression == null)
            {
                cachedExpression = GetExpressionInternal();
            }
            return cachedExpression;
        }

        private Expression<Func<TFrom, TTo>> GetExpressionInternal()
        {
            var p = Expression.Parameter(typeof(TFrom), "p");
            var memberBindings = this.propMappers.Select(kv => CreateMemberBinding(kv.Key, kv.Value, p)).ToArray();
            var expressions = Expression.MemberInit(Expression.New(typeof(TTo).GetConstructor(Type.EmptyTypes)!),
                memberBindings);
            return Expression.Lambda<Func<TFrom, TTo>>(expressions, p);
        }

        private MemberBinding CreateMemberBinding(string targetName, IMapperExpression sourceExpression,
            ParameterExpression p)
        {
            var memberInfo = typeof(TTo).GetProperty(targetName) as MemberInfo ??
                             typeof(TTo).GetField(targetName);
            return Expression.Bind(memberInfo!, sourceExpression.GetLambdaExpression().ReplaceFirstParam(p));
        }

        public Func<TFrom, TTo> GetFunc()
        {
            if (cachedFunc == null)
            {
                cachedFunc = this.GetExpression().Compile();
            }
            return cachedFunc;
        }

        private void DirtyCache()
        {
            this.cachedExpression = null;
            this.cachedFunc = null;
        }

      
    }
}
