using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static YS.Knife.Data.Filter.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Query.Functions.Collections
{
    public class Count : EmptyArgumentFunction
    {
        static readonly MethodInfo QueryableLongCountMethod1 = typeof(Queryable).GetMethods()
           .Single(p => p.Name == nameof(Queryable.LongCount) && p.GetParameters().Length == 1);
        static readonly MethodInfo QueryableLongCountMethod2 = typeof(Queryable).GetMethods()
            .Single(p => p.Name == nameof(Queryable.LongCount) && p.GetParameters().Length == 2);

        protected override FunctionResult OnExecute(ExecuteContext context)
        {
            throw new NotImplementedException();
        }

        //public override object[] ParseArguments(ParseContext parseContext)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override FunctionResult OnExecuteQueryable(CollectionFunctionContext context)
        //{
        //    //if (context.FunctionContext.SubFilter != null)
        //    //{
        //    //    var subFilter= FilterInfoExpressionBuilder.Default.CreateFilterLambdaExpression(context.ItemType, 
        //    //        context.FunctionContext.SubFilter, 
        //    //        context.FunctionContext.MemberExpressionProvider);
        //    //    var method = QueryableLongCountMethod2.MakeGenericMethod(context.ItemType);
        //    //    var paramExpression = Expression.Parameter(context.QueryableType);
        //    //    var methodCall = Expression.Call(method, context.Expression, subFilter);
        //    //    var lambda = Expression.Lambda(methodCall, paramExpression);
        //    //    return new FunctionResult
        //    //    {
        //    //        LambdaValueType = typeof(long),
        //    //        MemberProvider = IMemberExpressionProvider.GetObjectProvider(typeof(long)),
        //    //        LambdaExpression = lambda
        //    //    };
        //    //}
        //    //else
        //    {
        //        var method = QueryableLongCountMethod1.MakeGenericMethod(context.ItemType);
        //        var paramExpression = Expression.Parameter(context.QueryableType);
        //        var methodCall = Expression.Call(method, context.Expression);
        //        var lambda = Expression.Lambda(methodCall, paramExpression);
        //        return new FunctionResult
        //        {
        //            LambdaValueType = typeof(long),
        //            MemberProvider = IMemberExpressionProvider.GetObjectProvider(typeof(long)),
        //            LambdaExpression = lambda
        //        };
        //    }


        //}
    }
}
