using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Filter.Operators
{
    // user can not extend new operator, so make filter operator internal
    internal interface IFilterOperator
    {
        private static readonly Dictionary<Operator, IFilterOperator> AllOperators =
          Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                  !type.IsAbstract && typeof(IFilterOperator).IsAssignableFrom(type))
            .Select(type => Activator.CreateInstance(type) as IFilterOperator)
              .ToDictionary(p => p.Operator, p => p);

      
        LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right);

        Operator Operator { get; }

       
        internal static IFilterOperator GetOperator(Operator @operator)
        {
            if (AllOperators.TryGetValue(@operator, out var instance))
            {
                return instance;
            }
            throw FilterErrors.OperatorNotSupported(@operator);
        }
        internal static LambdaExpression CreateOperatorLambda(ExpressionValue leftValue, Operator opType, ExpressionValue rightValue)
        {
            var @operator = GetOperator(opType);

            return @operator.CompareValue(leftValue,rightValue); 
        }
    }
}
