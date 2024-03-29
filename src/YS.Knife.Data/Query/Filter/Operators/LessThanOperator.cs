﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
{
    class LessThanOperator : ComparableOperator
    {
        public static LessThanOperator Default = new LessThanOperator();

        public override Operator Operator => Operator.LessThan;

        protected override Func<Expression, Expression, BinaryExpression> CompareFunc => Expression.LessThan;
    }
}
