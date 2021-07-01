using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Data.Mappers;
using static YS.Knife.Data.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Functions
{
    public abstract class FilterFunction
    {
        private static Type[] SupportNumberTypes = new[]
        {
            typeof(decimal), typeof(long), typeof(double), typeof(float), typeof(int), typeof(decimal?),
            typeof(long?), typeof(double?), typeof(float?), typeof(int?)
        };
        public virtual string Name { get => this.GetType().Name; }
        private static readonly Dictionary<string, FilterFunction> AllFunctions =
            AppDomain.CurrentDomain.FindInstanceTypesByBaseType<FilterFunction>().ToDictionary(type => type.Name,
                type => Activator.CreateInstance(type) as FilterFunction,
                StringComparer.InvariantCultureIgnoreCase);
           

       public static FilterFunction GetFunctionByName(string funcName)
       {
           if (AllFunctions.TryGetValue(funcName, out FilterFunction func))
           {
               return func;
           }

           return null;
       }


        public abstract FunctionResult Execute(FunctionContext functionContext);

        public class Errors
        {
            public static Exception NotSupportForCollectionType(string function)
            {
                return new FieldInfo2ExpressionException($"Function {function} only support for collection type.");
            }
        }
        
    }

    public class FunctionContext
    {
        public Type FromType { get; set; }
        // subfilter and sub mapper only for collection type
        public FilterInfo2 SubFilter { get; set; }
        public List<ValueInfo> Args { get; set; }
    }

    public class FunctionResult
    {
        public LambdaExpression LambdaExpression { get; set; }
        public Type LambdaValueType { get; set; }

        public IFilterMemberInfoProvider MemberProvider{get;set;}

    }
}
