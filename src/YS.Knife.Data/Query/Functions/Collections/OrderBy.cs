using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Query.Functions.Collections
{
    public class OrderBy : BaseFunction<OrderInfo>
    {
        protected override int MinArgLength => 1;

        protected override int MaxArgLength => 1;

        protected override FunctionResult OnExecute(List<OrderInfo> args, ExecuteContext context)
        {
            throw new NotImplementedException();
        }

        protected override OrderInfo OnParseArgument(ParseContext parseContext)
        {
            return parseContext.ParseOrderInfo();
        }
    }
}
