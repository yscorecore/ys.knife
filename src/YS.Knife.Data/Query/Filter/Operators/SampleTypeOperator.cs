using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Query.Operators
{
    public abstract class SampleTypeOperator : IFilterOperator
    {
        public abstract Operator Operator { get; }


        public LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right)

        {
            if (left.IsConst && !right.IsConst)
            {
                var parameter = Expression.Parameter(left.SourceType);

                var rightLambda = right.GetLambda(parameter);
                // ensure both right and left use the same parameter
                var leftLambda = left.GetLambda(parameter, rightLambda.ReturnType);
                var expression = CompareValue(leftLambda.Body, rightLambda.Body, rightLambda.ReturnType);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(left.SourceType, typeof(bool)), expression, parameter);

            }
            else
            {
                var parameter = Expression.Parameter(left.SourceType);
                var leftLambda = left.GetLambda(parameter);
                // ensure both right and left use the same parameter
                var rightLambda = right.GetLambda(parameter, leftLambda.ReturnType);
                var expression = CompareValue(leftLambda.Body, rightLambda.Body, leftLambda.ReturnType);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(left.SourceType, typeof(bool)), expression, parameter);
            }
        }

        protected abstract Expression CompareValue(Expression left, Expression right, Type type);

    }
}
