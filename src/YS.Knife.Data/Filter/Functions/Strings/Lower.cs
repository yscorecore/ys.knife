﻿using System;
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
            var para = Expression.Parameter(context.MemberExpressionProvider.CurrentType);
            FilterValueDesc valueDesc = FilterInfoExpressionBuilder.Default.CreateFilterValueDesc(para, context.MemberExpressionProvider, context.Args.First());
            var exp = Expression.Call(valueDesc.GetExpression(typeof(string)), ToLowerMethod);

            return new FunctionResult
            {
                LambdaValueType = typeof(string),
                MemberProvider = IMemberExpressionProvider.GetObjectProvider(typeof(string)),
                LambdaExpression = Expression.Lambda(exp, para)

            };
        }
    }
}