using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.Functions
{
    public abstract class FunctionExpression
    {
        private static Type[] SupportNumberTypes = new[]
        {
            typeof(decimal), typeof(long), typeof(double), typeof(float), typeof(int), typeof(decimal?),
            typeof(long?), typeof(double?), typeof(float?), typeof(int?)
        };

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


       internal FunctionResult Execute(FunctionContext functionContext)
       {
          // var param = Expression.Parameter(functionContext.SourceType, "p");
          // typeof(IQueryable).IsAssignableFrom(sourceType)?MethodFinder.get
           return null;
       }
       
    }

    internal class FunctionContext
    {
        
     
    }

    internal class FunctionResult
    {

        
    }
}
