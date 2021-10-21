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
        public override Operator Operator { get => Operator.NotBetween; }

        protected override IFilterOperator LeftOperator { get => LessThanOperator.Default; }
        protected override IFilterOperator RightOperator { get => GreaterThanOperator.Default; }
        protected override Func<Expression, Expression, BinaryExpression> CombinFunc { get => Expression.OrElse; }

        protected override ConstantExpression EmptyValueFunc { get => Expression.Constant(false); }
    }
}
