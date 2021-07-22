using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

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

        Expression CompareValue(FilterValueDesc left, FilterValueDesc right);
        Operator Operator { get; }

        internal static Expression CreateOperatorExpression(FilterValueDesc left, Operator opType, FilterValueDesc right)
        {
            var @operator = GetOperator(opType);
            return @operator.CompareValue(left, right);
        }
        internal static IFilterOperator GetOperator(Operator @operator)
        {
            if (AllOperators.TryGetValue(@operator, out var instance))
            {
                return instance;
            }
            throw ExpressionErrors.OperatorNotSupported(@operator);
        }
    }
}
