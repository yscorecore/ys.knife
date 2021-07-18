using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Functions
{
    public abstract class EmptyArgumentFunction : BaseFunction
    {
        public sealed override FunctionResult Execute(object[] args, ExecuteContext context)
        {
            return OnExecute(context);
        }
        protected abstract FunctionResult OnExecute(ExecuteContext context);
        public sealed override object[] ParseArguments(ParseContext parseContext)
        {
            return null;
        }
    }
}
