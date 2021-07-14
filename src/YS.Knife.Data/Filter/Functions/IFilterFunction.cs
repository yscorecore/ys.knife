using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YS.Knife.Data.Filter.Functions
{
    // user can define self customize function, so make the interface public
    public interface IFilterFunction
    {
        string Name { get; }
        FunctionResult Execute(object[] args, ExecuteContext context);
        object[] ParseArguments(ParseContext parseContext);
        public static readonly Dictionary<string, IFilterFunction> AllFunctions =
              AppDomain.CurrentDomain.FindInstanceTypesByBaseType<IFilterFunction>().ToDictionary(type => type.Name,
                  type => Activator.CreateInstance(type) as IFilterFunction,
                  StringComparer.InvariantCultureIgnoreCase);

        internal static FunctionResult ExecuteFunction(string functionName,object[] args, ExecuteContext context)
        {
            if (AllFunctions.TryGetValue(functionName, out IFilterFunction func))
            {
                return func.Execute(args,context);
            }
            else
            {
                throw FunctionErrors.NotSupportFunction(functionName);   
            }
        }
        internal static object[] ParseFunctionArgument(string functionName, ParseContext parseContext)
        {
            if (AllFunctions.TryGetValue(functionName, out IFilterFunction func))
            {
                return func.ParseArguments(parseContext);
            }
            else
            {
                throw FunctionErrors.NotSupportFunction(functionName);
            }
        }
    }
}
