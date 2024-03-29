﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
{
    class StartsWithOperator : StringOperator
    {
        protected override string MethodName => nameof(string.StartsWith);
        public override Operator Operator => Operator.StartsWith;
    }
}
