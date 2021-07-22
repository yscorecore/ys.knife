using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Query.Functions.Order
{
    public class Desc : EmptyArgumentFunction
    {
     
        protected override FunctionResult OnExecute(ExecuteContext context)
        {
            // always should not run to here
            throw new NotImplementedException();
        }

     
    }
}
