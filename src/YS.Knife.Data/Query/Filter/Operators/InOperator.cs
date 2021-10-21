using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Query.Operators
{

    class InOperator : IFilterOperator
    {
        private static readonly LocalCache<Type, MethodInfo> ContainsMethods = new LocalCache<Type, MethodInfo>();
        public virtual LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right)
        {
            if (!right.IsConst)
            {
                // TODO ..
                // throw 
            }
            if (right.ValueInfo.ConstantValue is IList list)
            {

                var parameter = Expression.Parameter(left.SourceType);
                var leftExpression = left.GetLambda(parameter);
                var listType = typeof(List<>).MakeGenericType(leftExpression.ReturnType);
                var method = GetListContainsMethod(listType);
                var data = ConvertListItems(listType, leftExpression.ReturnType, list);
                if (data.Count == 0)
                {
                    return Expression.Lambda(Expression.Constant(false), parameter);
                }
                return Expression.Lambda(
                    Expression.Call(Expression.Constant(data!), method, leftExpression.Body),
                    parameter);
            }
            else
            {
                // TODO ..
                // throw
            }
            throw new NotImplementedException();
        }

        private IList ConvertListItems(Type targetListType, Type targetItemType, IList sourceData)
        {
            var result = Activator.CreateInstance(targetListType) as IList;
            foreach (var item in sourceData)
            {
                if (item == null)
                {
                    if (CanAssignNull(targetItemType))
                    {
                        result!.Add(null);
                    }
                }
                else
                {
                    result!.Add(ValueConverter.Instance.ChangeType(item, targetItemType));
                }
            }
            return result;
        }

        private MethodInfo GetListContainsMethod(Type listType)
        {
            return ContainsMethods.Get(listType, (type) => type.GetMethod(nameof(List<string>.Contains)));
        }
        static bool CanAssignNull(Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }
        public virtual Operator Operator { get => Operator.In; }
    }
}
