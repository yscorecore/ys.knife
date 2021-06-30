using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace YS.Knife.Data.Functions
{
    class Count : CollectionFunction
    {

        static readonly MethodInfo QueryableLongCountMethod1 = typeof(Queryable).GetMethods()
           .Single(p => p.Name == nameof(Queryable.LongCount) && p.GetParameters().Length == 1);
        static readonly MethodInfo QueryableLongCountMethod2 = typeof(Queryable).GetMethods()
            .Single(p => p.Name == nameof(Queryable.LongCount) && p.GetParameters().Length == 2);

        static readonly MethodInfo EnumerableLongCountMethod1 = typeof(Enumerable).GetMethods()
            .Single(p => p.Name == nameof(Enumerable.LongCount) && p.GetParameters().Length == 1);
        static readonly MethodInfo EnumerableLongCountMethod2 = typeof(Enumerable).GetMethods()
            .Single(p => p.Name == nameof(Enumerable.LongCount) && p.GetParameters().Length == 2);

        private static MethodInfo GetEnumerableLongCount(Type elementType, bool predicate) => predicate
                ? EnumerableLongCountMethod2.MakeGenericMethod(elementType)
                : EnumerableLongCountMethod1.MakeGenericMethod(elementType);
        private static MethodInfo GetQueryableLongCount(Type elementType, bool predicate) => predicate
                ? QueryableLongCountMethod2.MakeGenericMethod(elementType)
                : QueryableLongCountMethod1.MakeGenericMethod(elementType);
        
        protected override FunctionResult ExecuteQueryable(FunctionContext functionContext, Type itemType)
        {
            var paramExpression = Expression.Parameter(functionContext.FromType);

            if (functionContext.SubFilter != null)
            {
                if (functionContext.SubMapper != null)
                {
                   var lambdaExp = FilterInfoExpressionBuilder.Default.CreateSourceFilterExpression(functionContext.SubMapper, functionContext.SubFilter);
                }
                else 
                {
                  var lambdaExp = FilterInfoExpressionBuilder.Default.CreateFilterExpression(itemType, functionContext.SubFilter);
                }
                
                return null;
            }
            else
            {
                var methodCall = Expression.Call(paramExpression, GetQueryableLongCount(itemType, false), Expression.Constant(null));
                return new FunctionResult
                {
                    LambdaValueType = typeof(long),
                    LambdaExpression = Expression.Lambda(methodCall, paramExpression),
                };
            }


        }
        protected override FunctionResult ExecuteEnumable(FunctionContext functionContext, Type itemType)
        {
            var paramExpression = Expression.Parameter(functionContext.FromType);
            var methodCall = Expression.Call(GetEnumerableLongCount(itemType, false),paramExpression);
            return new FunctionResult
            {
                LambdaValueType = typeof(long),
                LambdaExpression = Expression.Lambda(methodCall, paramExpression),

            };
        }
    }


    abstract class CollectionFunction : FilterFunction
    {
        public sealed override FunctionResult Execute(FunctionContext functionContext)
        {
            Type itemType = EnumerableTypeUtils.GetQueryableItemType(functionContext.FromType);
            if (itemType != null)
            {
                return ExecuteQueryable(functionContext, itemType);
            }
            itemType = EnumerableTypeUtils.GetEnumerableItemType(functionContext.FromType);
            if (itemType != null)
            {
                return ExecuteEnumable(functionContext, itemType);
            }
            throw Errors.NotSupportForCollectionType(this.GetType().Name);
        }

        protected abstract FunctionResult ExecuteEnumable(FunctionContext functionContext, Type itemType);


        protected abstract FunctionResult ExecuteQueryable(FunctionContext functionContext, Type itemType);
    }

}

