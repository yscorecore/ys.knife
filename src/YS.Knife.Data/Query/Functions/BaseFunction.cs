using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace YS.Knife.Data.Query.Functions
{
    public abstract class BaseFunction : IFilterFunction
    {
        public string Name => this.GetType().Name.ToLower();

        public abstract FunctionResult Execute(object[] args, ExecuteContext context);

        public abstract object[] ParseArguments(ParseContext parseContext);

    }
    public abstract class BaseFunction<TArg> : BaseFunction 
    { 
        protected abstract int MinArgLength { get; }
        protected abstract int MaxArgLength { get; }
        public sealed override FunctionResult Execute(object[] args, ExecuteContext context)
        {
            Debug.Assert(args.Length >= MinArgLength, $"the function '{Name}' requires at least {MinArgLength} argument.");
            Debug.Assert(args.Length <= MaxArgLength, $"the function '{Name}' requires at most {MaxArgLength} argument.");
            return OnExecute(args.Cast<TArg>().ToList(), context);
        }
        protected abstract FunctionResult OnExecute(List<TArg> args, ExecuteContext context);

        public sealed override object[] ParseArguments(ParseContext context)
        {
            // skip open 
            List<TArg> args = new List<TArg>();
            context.SkipWhiteSpaceAndFirstChar('(');

            while (context.NotEnd())
            {
                if (!context.SkipWhiteSpace())
                {
                    break;
                }
                if (context.Current() == ')')
                {
                    break;
                }
                var arg = this.OnParseArgument(context);
                args.Add(arg);

                if (context.SkipWhiteSpace())
                {
                    if (context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else if (context.Current() == ')')
                    {
                        break;
                    }
                    else
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                }
            }
            context.SkipWhiteSpaceAndFirstChar(')');

            if (args.Count < MinArgLength) 
            {
                throw ParseErrors.FunctionArgumentLessThan(context, this.Name, MinArgLength);
            }

            if (args.Count > MaxArgLength)
            {
                throw ParseErrors.FunctionArgumentGreatThan(context, this.Name, MaxArgLength);
            }

            return args.Cast<object>().ToArray();
        }
        protected abstract TArg OnParseArgument(ParseContext parseContext);
    }

}
