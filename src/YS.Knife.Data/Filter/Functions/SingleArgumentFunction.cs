using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace YS.Knife.Data.Filter.Functions
{
    public abstract class SingleArgumentFunction<T> : BaseFunction
    {
        public sealed override FunctionResult Execute(object[] args, ExecuteContext context)
        {
            return OnExecute(args.Cast<T>().FirstOrDefault(), context);
        }
        protected abstract FunctionResult OnExecute(T arg, ExecuteContext context);

        public sealed override object[] ParseArguments(ParseContext parseContext)
        {
            var first = OnParseArgument(parseContext);
            return new object[] { first };
        }
        protected abstract T OnParseArgument(ParseContext parseContext);

    }
}
