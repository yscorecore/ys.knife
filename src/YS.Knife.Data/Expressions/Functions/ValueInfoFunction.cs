using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Expressions.Functions
{
    public abstract  class ValueInfoFunction : BaseFunction<ValueInfo>
    {
        protected override ValueInfo OnParseArgument(ParseContext parseContext)
        {
            return parseContext.ParseValueInfo();
        }
    }
}
