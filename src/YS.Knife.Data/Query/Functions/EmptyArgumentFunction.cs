using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace YS.Knife.Data.Query.Functions
{
    public abstract class EmptyArgumentFunction : BaseFunction
    {
        public sealed override FunctionResult Execute(object[] args, ExecuteContext context)
        {
            Debug.Assert(args.Length == 0, $"the function '{Name}' should has no argument");
            return OnExecute(context);
        }
        protected abstract FunctionResult OnExecute(ExecuteContext context);

        public sealed override object[] ParseArguments(ParseContext context)
        {
            // skip open 
            context.SkipWhiteSpaceAndFirstChar('(');
            // skip close
            context.SkipWhiteSpaceAndFirstChar(')');

            return Array.Empty<object>();
        }
    }
}
