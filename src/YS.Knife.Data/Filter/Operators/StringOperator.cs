using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    abstract class StringOperator : SampleTypeOperator
    {
        public override Operator Operator { get; }
    }
}
