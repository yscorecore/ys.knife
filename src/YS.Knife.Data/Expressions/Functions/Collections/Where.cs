using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Expressions.Functions.Collections
{
    public class Where : BaseFunction<FilterInfo2>
    {
        protected override int MinArgLength => 1;

        protected override int MaxArgLength => 1;

        protected override FunctionResult OnExecute(List<FilterInfo2> args, ExecuteContext context)
        {
            throw new NotImplementedException();
        }

        protected override FilterInfo2 OnParseArgument(ParseContext parseContext)
        {
            return parseContext.ParseFilterInfo();
        }
    }
}
