using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data.Query.Expressions;
using YS.Knife.Data.Query.Operators;

namespace YS.Knife.Data.Query.Operators
{
    class NotBetweenOperator : BetweenOperator
    {
        static IFilterOperator LeftOperator = new LessThanOperator();
        static IFilterOperator RightOperator = new GreaterThanOperator();
        public override Operator Operator { get => Operator.NotBetween; }

        protected override LambdaExpression CompareValue(ExpressionValue left, object minValue, object maxValue)
        {
            List<LambdaExpression> lambdaList = new List<LambdaExpression>(2);
            if (minValue != null)
            {

                var minLambda = LeftOperator.CompareValue(left,
                    new ExpressionValue(left.SourceType,
                    ValueInfo.FromConstantValue(minValue),
                    IMemberVisitor.GetObjectVisitor(left.SourceType)));

                lambdaList.Add(minLambda);

            }
            if (maxValue != null)
            {
                var maxLambda = RightOperator.CompareValue(left,
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
                var body = Expression.OrElse(firstBody, secondBody);
                return Expression.Lambda(body, parameter);
            }

        }
    }
}
