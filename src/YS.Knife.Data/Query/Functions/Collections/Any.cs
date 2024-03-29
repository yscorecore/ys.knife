﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Query.Functions.Collections
{
    class Any : BaseFunction<FilterInfo>
    {
        protected override int MinArgLength => 0;

        protected override int MaxArgLength => 1;

        protected override FunctionResult OnExecute(List<FilterInfo> args, ExecuteContext context)
        {
            FilterInfo filterInfo = args.Count > 0 ? args[0] : null;
            if (filterInfo != null)
            {

                return null;
            }
            else
            {
                return null;
            }
        }

        protected override FilterInfo OnParseArgument(ParseContext parseContext)
        {
            return parseContext.ParseFilterInfo();
        }
    }

}
