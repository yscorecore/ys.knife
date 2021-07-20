using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Functions
{
    public abstract  class ValueInfoFunction : BaseFunction<ValueInfo>
    {
        protected override int MinArgLength => throw new NotImplementedException();

        protected override int MaxArgLength => throw new NotImplementedException();

        protected override FunctionResult OnExecute(List<ValueInfo> args, ExecuteContext context)
        {
            throw new NotImplementedException();
        }

        protected override ValueInfo OnParseArgument(ParseContext parseContext)
        {
            
            return null;
        }
    }
}
