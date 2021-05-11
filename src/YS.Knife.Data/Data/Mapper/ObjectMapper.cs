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

    public class ObjectMapper<TFrom, TTo> : ObjectMapper
        where TTo : new()
    {
        private readonly Dictionary<string, LambdaExpression> propMappers = new Dictionary<string, LambdaExpression>();
        private Expression<Func<TFrom, TTo>> cachedExpression = null;
        private Func<TFrom, TTo> cachedFunc = null;

        public ObjectMapper() : base(typeof(TFrom), typeof(TTo))
        {
        }

        public ObjectMapper<TFrom, TTo> LoadDefault()
        {
            return this;
        }

        public ObjectMapper<TFrom, TTo> Append<TValue>(Expression<Func<TTo, TValue>> targetProperty,
            Expression<Func<TFrom, TValue>> sourceExpression)
        {
            _ = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            if (targetProperty.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new InvalidOperationException("source member only supported.");
            }

            var memberAccess = targetProperty.Body as MemberExpression;
            var memberName = memberAccess.Member.Name;
            this.propMappers[memberName] = sourceExpression;
            this.DirtyCache();
            return this;
        }

        public ObjectMapper<TFrom, TTo> Ignore(Expression<Func<TTo, object>> targetProperty)
        {
            _ = targetProperty ?? throw new ArgumentNullException(nameof(targetProperty));
            if (targetProperty.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memberAccess = targetProperty.Body as MemberExpression;
                var memberName = memberAccess!.Member.Name;
                return Ignore(memberName);
            }
            return this;
        }

        public ObjectMapper<TFrom, TTo> Ignore(params string[] targetMembers)
        {
            Array.ForEach(targetMembers ?? new string[0], targetMember =>
            {
                if (this.propMappers.Remove(targetMember))
                {
                    this.DirtyCache();
                }
            });
            return this;
        }

        public Expression<Func<TFrom, TTo>> GetExpression()
        {
            if (cachedExpression == null)
            {
                var p = Expression.Parameter(typeof(TFrom), "p");
                var memberBindings = this.propMappers.Select(kv => CreateMemberBinding(kv.Key, kv.Value, p)).ToArray();
                var expressions = Expression.MemberInit(Expression.New(typeof(TTo).GetConstructor(Type.EmptyTypes)!),
                    memberBindings);
                cachedExpression = Expression.Lambda<Func<TFrom, TTo>>(expressions, p);
            }
            return cachedExpression;
        }

        private MemberBinding CreateMemberBinding(string targetName, LambdaExpression sourceExpression,
            ParameterExpression p)
        {
            var memberInfo = typeof(TTo).GetProperty(targetName) as MemberInfo ??
                             typeof(TTo).GetField(targetName);
            return Expression.Bind(memberInfo!, ReplaceFirstParam(sourceExpression, p));
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

        class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression m_parameter;
            private readonly Expression m_replacement;

            public ParameterReplacer(ParameterExpression parameter, Expression replacement)
            {
                this.m_parameter = parameter;
                this.m_replacement = replacement;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (ReferenceEquals(node, m_parameter))
                    return m_replacement;
                return node;
            }
        }

        static Expression ReplaceFirstParam(LambdaExpression expression, ParameterExpression newParameter)
        {
            var parameterToRemove = expression.Parameters.ElementAt(0);
            var replacer = new ParameterReplacer(parameterToRemove, newParameter);
            return replacer.Visit(expression.Body);
        }
    }
}
