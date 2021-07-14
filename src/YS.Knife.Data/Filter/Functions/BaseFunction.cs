using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Functions
{
    public abstract class BaseFunction : IFilterFunction
    {
        public string Name => this.GetType().Name.ToLower();

        public abstract FunctionResult Execute(object[] args, ExecuteContext context);

        //TODO make parse argument as abstract
        public virtual object[] ParseArguments(ParseContext parseContext)
        {
            return null;
        }

    }
}
