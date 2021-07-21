using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.Expressions.Functions.Collections
{
    public abstract class CollectionFunction : BaseFunction, IFilterFunction
    {
        private static Type[] SupportNumberTypes = new[]
        {
            typeof(decimal), typeof(long), typeof(double), typeof(float), typeof(int), typeof(decimal?),
            typeof(long?), typeof(double?), typeof(float?), typeof(int?)
        };

        public sealed override FunctionResult Execute(object[] args,ExecuteContext context)
        {
            var itemType = EnumerableTypeUtils.GetEnumerableItemType(context.CurrentType);
            if (itemType == null)
            {
                throw FunctionErrors.OnlyCanUseFunctionInCollectionType(this.Name);
            }

            if (!typeof(IQueryable<>).MakeGenericType(itemType).IsAssignableFrom(context.CurrentType))
            {
                var queryableExpression = Expression.Call(
                    MethodFinder.GetAsQueryable(itemType),
                    context.CurrentExpression);
                return OnExecuteQueryable(new CollectionFunctionContext(context, itemType, queryableExpression));
            }
            else
            {
                return OnExecuteQueryable(new CollectionFunctionContext(context, itemType, context.CurrentExpression));
            }
        }
        protected abstract FunctionResult OnExecuteQueryable(CollectionFunctionContext context);

        protected class CollectionFunctionContext  
        {
            public CollectionFunctionContext(ExecuteContext functionContext,Type itemType, Expression expression)
            {
                FunctionContext = functionContext;
                ItemType = itemType;
                Expression = expression;
                QueryableType = typeof(IQueryable<>).MakeGenericType(itemType);
            }
            public Type QueryableType { get; }

            public ExecuteContext FunctionContext { get; }

            public Expression Expression { get; }
            public Type ItemType { get; }
        }
    }
}
