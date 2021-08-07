using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data.Filter.Operators;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Query.Filter.Operators
{
    class BetweenOperator : IFilterOperator
    {
        static GreaterThanOrEqualOperator MinOperator = new GreaterThanOrEqualOperator();
        static LessThanOrEqualOperator MaxOperator = new LessThanOrEqualOperator();
        public Operator Operator { get => Operator.Between; }

        public LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right)
        {

            if (!right.IsConst)
            {
                // TODO ..
                // throw 
            }

            if (right.ValueInfo.ConstantValue is IList list)
            {
                if (list.Count != 2)
                {
                    // TODO ..
                    // throw between only support 2 value
                }
                return CompareValue(left, list[0], list[1]);
            }
            else
            {
                // TODO ..
                // throw
            }

            throw new NotImplementedException();
        }
        protected LambdaExpression CompareValue(ExpressionValue left, object minValue, object maxValue)
        {
            List<LambdaExpression> lambdaList = new List<LambdaExpression>(2);
            if (minValue != null)
            {

                var minLambda = MinOperator.CompareValue(left,
                    new ExpressionValue(left.SourceType,
                    ValueInfo.FromConstantValue(minValue),
                    IMemberVisitor.GetObjectVisitor(left.SourceType)));

                lambdaList.Add(minLambda);

            }
            if (maxValue != null)
            {
                var maxLambda = MaxOperator.CompareValue(left,
                     new ExpressionValue(left.SourceType,
                     ValueInfo.FromConstantValue(maxValue),
                     IMemberVisitor.GetObjectVisitor(left.SourceType)));
                lambdaList.Add(maxLambda);

            }
            if (lambdaList.Count == 0)
            {
                return Expression.Lambda(Expression.Constant(true), Expression.Parameter(left.SourceType));
            }
            else if (lambdaList.Count == 1)
            {
                return lambdaList.First();
            }
            else
            {
                var parameter = Expression.Parameter(left.SourceType);
                var firstBody = lambdaList.First().ReplaceFirstParam(parameter);
                var secondBody = lambdaList.Last().ReplaceFirstParam(parameter);
                var body = Expression.AndAlso(firstBody, secondBody);
                return Expression.Lambda(body, parameter);
            }

        }

    }
}
