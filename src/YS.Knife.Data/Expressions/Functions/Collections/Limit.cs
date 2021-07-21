using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Expressions.Functions.Collections
{
    public class Limit : BaseFunction<LimitInfo>
    {
        protected override int MinArgLength => 1;

        protected override int MaxArgLength =>1;

        protected override FunctionResult OnExecute(List<LimitInfo> args, ExecuteContext context)
        {
            throw new NotImplementedException();
        }

        protected override LimitInfo OnParseArgument(ParseContext parseContext)
        {
            return parseContext.ParseLimitInfo();
        }
    }
}
