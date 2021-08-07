using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Data.Query.Expressions
{
    public class ExpressionValue
    {
        public ExpressionValue(Type sourceType, ValueInfo valueInfo, IMemberVisitor memberVisitor)
        {
            SourceType = sourceType;
            ValueInfo = valueInfo;
            MemberVisitor = memberVisitor;
            this.LambdaProvider = CreateValueLambda(sourceType, valueInfo, memberVisitor);
        }
        private IValueLambdaProvider LambdaProvider;
        public bool IsConst { get => ValueInfo == null || ValueInfo.IsConstant; }
        public Type SourceType { get; set; }
        public ValueInfo ValueInfo { get; set; }
        public IMemberVisitor MemberVisitor { get; set; }



        private IValueLambdaProvider CreateValueLambda(Type sourceType, ValueInfo valueInfo, IMemberVisitor memberVisitor)
        {
            _ = valueInfo ?? throw new ArgumentNullException(nameof(valueInfo));
            if (valueInfo.IsConstant)
            {
                return CreateConstValueLambda(valueInfo.ConstantValue);
            }
            else
            {
                return CreateValuePathLambda(valueInfo.NavigatePaths);
            }

            IValueLambdaProvider CreateValuePathLambda(List<ValuePath> valuePaths)
            {
                return new PathValueLambdaProvider(sourceType, valuePaths, memberVisitor);

            }
            IValueLambdaProvider CreateConstValueLambda(object value)
            {
                return new ConstValueLambdaProvider(sourceType, value);
            }
        }

        public LambdaExpression GetLambda(ParameterExpression parameterExpression)
        {
            return LambdaProvider.GetLambda(parameterExpression);
        }
        public LambdaExpression GetLambda(ParameterExpression parameterExpression, Type targetType)
        {
            return LambdaProvider.GetLambda(parameterExpression, targetType);
        }
        public LambdaExpression GetLambda()
        {
            var parameter = Expression.Parameter(SourceType, "p");
            return GetLambda(parameter);
        }
        public LambdaExpression GetLambda(Type targetType)
        {
            var parameter = Expression.Parameter(SourceType, "p");
            return GetLambda(parameter, targetType);
        }
    }
}
