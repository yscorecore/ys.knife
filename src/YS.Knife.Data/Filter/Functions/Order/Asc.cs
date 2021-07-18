using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Functions.Order
{
    public class Asc : EmptyArgumentFunction
    {
        protected override FunctionResult OnExecute(ExecuteContext context)
        {
            // always should not run to here
            throw new NotImplementedException();
        }
    }
}
