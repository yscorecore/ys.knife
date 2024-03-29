﻿using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Query.Operators
{
    class GreaterThanOperator : ComparableOperator
    {
        public static GreaterThanOperator Default = new GreaterThanOperator();
        public override Operator Operator => Operator.GreaterThan;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.GreaterThan;
    }
}
