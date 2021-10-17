using System;
using System.Collections.Generic;
using System.Text;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
{
    class ContainsOperator : StringOperator
    {
        protected override string MethodName => nameof(string.Contains);
        public override Operator Operator => Operator.Contains;
    }
}
