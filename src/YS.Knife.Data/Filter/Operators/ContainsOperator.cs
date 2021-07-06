using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    class ContainsOperator : StringOperator
    {
        protected override string MethodName => nameof(string.Contains);
        public override Operator Operator => Operator.Contains;
    }
}
