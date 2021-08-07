using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YS.Knife.Data.Query.Functions.Strings
{
    //public class Lower : StringFunction
    //{
    //    MethodInfo ToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
    //    public override FunctionResult Execute(ExecuteContext context)
    //    {
    //        if (context.Args.Length != 1)
    //        {
    //            throw FunctionErrors.ArgumentCountNotMatched(this.Name);
    //        }
    //        var para = Expression.Parameter(context.MemberExpressionProvider.CurrentType);
    //        FilterValueDesc valueDesc = FilterInfoExpressionBuilder.Default.CreateFilterValueDesc(para, context.MemberExpressionProvider, context.Args.First() as ValueInfo);
    //        var exp = Expression.Call(valueDesc.GetExpression(typeof(string)), ToLowerMethod);

    //        return new FunctionResult
    //        {
    //            LambdaValueType = typeof(string),
    //            MemberProvider = IMemberExpressionProvider.GetObjectProvider(typeof(string)),
    //            LambdaExpression = Expression.Lambda(exp, para)

    //        };
    //    }
    //}
}
