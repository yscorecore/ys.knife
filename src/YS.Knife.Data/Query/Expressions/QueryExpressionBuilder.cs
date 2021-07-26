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
        public IFuncLambdaProvider CreateValueLambda<TSource>(ValueInfo valueInfo)
        {
            _ = valueInfo ?? throw new ArgumentNullException(nameof(valueInfo));
            if (valueInfo.IsConstant)
            {
                return CreateConstValueLambda<TSource>(valueInfo.ConstantValue);
            }
            else
            {
                return CreateValueLambda<TSource>(valueInfo.NavigatePaths);
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
                return CreateValueLambda(valueInfo.NavigatePaths, mapper);
            }
        }
        public IFuncLambdaProvider CreateConstValueLambda<TSource>(object value)
        {
            return new ConstValueLambdaProvider<TSource>(value);
        }
        public IFuncLambdaProvider CreateValueLambda<TSource, TTarget>(List<ValuePath> valuePaths, ObjectMapper<TSource, TTarget> mapper)
                  where TSource : class
           where TTarget : class, new()
        {
            return null;
        }
        public IFuncLambdaProvider CreateValueLambda<TSource>(List<ValuePath> valuePaths)
        {
            return null;

        }
    }
}
