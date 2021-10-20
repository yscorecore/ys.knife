using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data.Query.Operators;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Query.Operators
{
    class BetweenOperator : IFilterOperator
    {
      
        public virtual Operator Operator { get => Operator.Between; }

        public LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right)
        {

            if (!right.IsConst)
            {
                // TODO ..
                // throw 
            }

            if (right.ValueInfo.ConstantValue is IList list)
            {
                if (list.Count != 2)
                {
                    // TODO ..
                    // throw between only support 2 value
                }
                return CompareValue(left, list[0], list[1]);
            }
            else
            {
                // TODO ..
                // throw
            }

            throw new NotImplementedException();
        }
        protected virtual LambdaExpression CompareValue(ExpressionValue left, object minValue, object maxValue)
        {
            List<LambdaExpression> lambdaList = new List<LambdaExpression>(2);
            if (minValue != null)
            {

                var minLambda = LeftOperator.CompareValue(left,
                    new ExpressionValue(left.SourceType,
                    ValueInfo.FromConstantValue(minValue),
                    IMemberVisitor.GetObjectVisitor(left.SourceType)));
                
                lambdaList.Add(minLambda);

            }
            if (maxValue != null)
            {
                var maxLambda = RightOperator.CompareValue(left,
                     new ExpressionValue(left.SourceType,
                     ValueInfo.FromConstantValue(maxValue),
                     IMemberVisitor.GetObjectVisitor(left.SourceType)));
                lambdaList.Add(maxLambda);

            }
            if (lambdaList.Count == 0)
            {
                return Expression.Lambda(EmptyValueFunc, Expression.Parameter(left.SourceType));
            }
            else if (lambdaList.Count == 1)
            {
                return lambdaList.First();
            }
            else
            {
                var parameter = Expression.Parameter(left.SourceType);
                var firstBody = lambdaList.First().ReplaceFirstParam(parameter);
                var secondBody = lambdaList.Last().ReplaceFirstParam(parameter);
                var body = CombinFunc(firstBody, secondBody);
                return Expression.Lambda(body, parameter);
            }

        }

        protected virtual IFilterOperator LeftOperator { get => GreaterThanOrEqualOperator.Default; } 
        protected virtual IFilterOperator RightOperator { get => LessThanOrEqualOperator.Default; }
        protected virtual Func<Expression, Expression, BinaryExpression> CombinFunc { get => Expression.AndAlso; }
        
        protected virtual ConstantExpression EmptyValueFunc { get => Expression.Constant(true); }
    }

     class InOperator:IFilterOperator
    {
        private static readonly LocalCache<Type, MethodInfo> ContainsMethods = new LocalCache<Type, MethodInfo>();
        public LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right)
        {
            if (!right.IsConst)
            {
                // TODO ..
                // throw 
            }
            if (right.ValueInfo.ConstantValue is IList list)
            {
                if (list.Count == 0)
                {
                    return Expression.Lambda(Expression.Constant(false), Expression.Parameter(left.SourceType));
                }
                var parameter = Expression.Parameter(left.SourceType);
                var leftExpression = left.GetLambda(parameter);
                
                var listType = typeof(List<>).MakeGenericType(leftExpression.ReturnType);
                var method = GetListContainsMethod(listType);
                var data = ConvertListItems(listType, leftExpression.ReturnType, list);
               
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

        private IList ConvertListItems(Type targetListType,Type targetItemType,  IList sourceData)
        {
            var result = Activator.CreateInstance(targetListType) as IList;
            foreach (var item in sourceData)
            {
                result!.Add(ValueConverter.Instance.ChangeType(item, targetItemType));
            }
            return result;
        }

        private MethodInfo GetListContainsMethod(Type listType)
        {
            return  ContainsMethods.Get(listType, (type) => type.GetMethod(nameof(List<string>.Contains)));
        }

        public virtual Operator Operator { get => Operator.In; }
    }
}
