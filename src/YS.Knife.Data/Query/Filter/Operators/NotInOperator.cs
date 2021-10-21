using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Query.Operators
{
    class NotInOperator : InOperator
    {
        public override Operator Operator { get => Operator.NotIn; }

        public override LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right)
        {
            var baseLambda = base.CompareValue(left, right);
            var parameter = baseLambda.Parameters.First();
            return Expression.Lambda(Expression.Not(baseLambda.Body), parameter);
        }
    }
}
