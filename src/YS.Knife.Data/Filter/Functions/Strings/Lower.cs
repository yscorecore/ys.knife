using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static YS.Knife.Data.Filter.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Filter.Functions.Strings
{
    public class Lower : StringFunction
    {
        MethodInfo ToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
        public override FunctionResult Execute(FunctionContext context)
        {
            if (context.Args.Count != 1)
            {
                throw FunctionErrors.ArgumentCountNotMatched(this.Name);
            }

            FilterValueDesc valueDesc = FilterInfoExpressionBuilder.Default.CreateFilterValueDesc(context.CurrentExpression , context.MemberExpressionProvider, context.Args.First());
            Expression.Call(valueDesc.ValueExpression, ToLowerMethod);

            return new FunctionResult
            {
                 LambdaValueType = typeof(string),
                 MemberProvider = IMemberExpressionProvider.GetObjectProvider(typeof(string)),

            };
        }
    }
}
