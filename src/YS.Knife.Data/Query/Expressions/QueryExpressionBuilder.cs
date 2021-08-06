using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.Query.Expressions
{
    public class QueryExpressionBuilder
    {
        public static readonly QueryExpressionBuilder Default = new QueryExpressionBuilder();
        public IFuncLambdaProvider CreateValueLambda<TSource>(ValueInfo valueInfo)
        {
            _ = valueInfo ?? throw new ArgumentNullException(nameof(valueInfo));
            if (valueInfo.IsConstant)
            {
                return CreateConstValueLambda<TSource>(valueInfo.ConstantValue);
            }
            else
            {
                return CreateValuePathLambda<TSource>(valueInfo.NavigatePaths);
            }
        }
        public IFuncLambdaProvider CreateValueLambda<TSource, TTarget>(ValueInfo valueInfo, ObjectMapper<TSource, TTarget> mapper)
          where TSource : class
          where TTarget : class, new()
        {
            _ = valueInfo ?? throw new ArgumentNullException(nameof(valueInfo));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            if (valueInfo.IsConstant)
            {
                return CreateConstValueLambda<TSource>(valueInfo.ConstantValue);
            }
            else
            {
                return CreateValuePathLambda(valueInfo.NavigatePaths, mapper);
            }
        }
        public IFuncLambdaProvider CreateConstValueLambda<TSource>(object value)
        {
            return new ConstValueLambdaProvider<TSource>(value);
        }
        public IFuncLambdaProvider CreateValuePathLambda<TSource, TTarget>(List<ValuePath> valuePaths, ObjectMapper<TSource, TTarget> mapper)
                  where TSource : class
           where TTarget : class, new()
        {
            return new PathValueLambdaProvider<TSource>(valuePaths, IMemberVisitor.GetMapperVisitor(mapper));
        }
        public IFuncLambdaProvider CreateValuePathLambda<TSource>(List<ValuePath> valuePaths)
        {
            return new PathValueLambdaProvider<TSource>(valuePaths, IMemberVisitor.GetObjectVisitor(typeof(TSource)));

        }
    }
}
