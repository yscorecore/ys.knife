﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Query.Functions
{
    public abstract class ValueInfoFunction : BaseFunction<ValueInfo>
    {
        protected sealed override ValueInfo OnParseArgument(ParseContext parseContext)
        {
            return parseContext.ParseValueInfo();
        }
    }
}
