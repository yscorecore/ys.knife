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
        FunctionResult Execute(FunctionContext context);
        private static readonly Dictionary<string, IFilterFunction> AllFunctions =
              AppDomain.CurrentDomain.FindInstanceTypesByBaseType<IFilterFunction>().ToDictionary(type => type.Name,
                  type => Activator.CreateInstance(type) as IFilterFunction,
                  StringComparer.InvariantCultureIgnoreCase);

        internal static FunctionResult ExecuteFunction(FunctionContext context)
        {
            if (AllFunctions.TryGetValue(context.Name, out IFilterFunction func))
            {
                return func.Execute(context);
            }
            else
            {
                throw FunctionErrors.NotSupportFunction(context.Name);   
            }
        }
    }
}
