using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Expressions.Functions.Collections
{
    class Any : BaseFunction<FilterInfo2>
    {
        protected override int MinArgLength => 0;

        protected override int MaxArgLength => 1;

        protected override FunctionResult OnExecute(List<FilterInfo2> args, ExecuteContext context)
        {
            FilterInfo2 filterInfo = args.Count > 0 ? args[0] : null;
            if (filterInfo!=null)
            {

                return null;
            }
            else
            {
                return null;
            }
        }

        protected override FilterInfo2 OnParseArgument(ParseContext parseContext)
        {
            return parseContext.ParseFilterInfo();
        }
    }
    
}
