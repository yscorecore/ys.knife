using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Mappers
{
    public static class ExpressionExtensions
    {
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

        public static Expression ReplaceFirstParam(this LambdaExpression expression, Expression newParameter)
        {
            var parameterToRemove = expression.Parameters.ElementAt(0);
            var replacer = new ParameterReplacer(parameterToRemove, newParameter);
            return replacer.Visit(expression.Body);
        }

        public static Expression Connect(this Expression parent, LambdaExpression child)
        {
            return child.ReplaceFirstParam(parent);
        }
    }
}
