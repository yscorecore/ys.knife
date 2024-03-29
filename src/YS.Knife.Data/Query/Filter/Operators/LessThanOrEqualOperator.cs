﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
{
    class LessThanOrEqualOperator : ComparableOperator
    {
        public static LessThanOrEqualOperator Default = new LessThanOrEqualOperator();

        public override Operator Operator => Operator.LessThanOrEqual;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.LessThanOrEqual;
    }
}
