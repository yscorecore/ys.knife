using System;
using System.Collections.Generic;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Filter.Operators
{
    class EndsWithOperator : StringOperator
    {
        protected override string MethodName => nameof(string.EndsWith);
        public override Operator Operator => Operator.EndsWith;
    }
}

