using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Data.Functions
{
    public abstract class FunctionExpression
    {
        private static readonly Dictionary<string, FunctionExpression> AllFunctions =
            AppDomain.CurrentDomain.FindInstanceTypesByBaseType<FunctionExpression>().ToDictionary(type => type.Name,
                type => Activator.CreateInstance(type) as FunctionExpression,
                StringComparer.InvariantCultureIgnoreCase);
           

       public static FunctionExpression GetFunctionByName(string funcName)
       {
           if (AllFunctions.TryGetValue(funcName, out FunctionExpression func))
           {
               return func;
           }

           return null;
       }
    }
}
