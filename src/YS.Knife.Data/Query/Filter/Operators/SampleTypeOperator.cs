using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Filter.Operators
{
    public abstract class SampleTypeOperator : IFilterOperator
    {
        public abstract Operator Operator { get; }
        

        public LambdaExpression CompareValue(ExpressionValue left, ExpressionValue right)

        {
            if (left.IsConst && !right.IsConst)
            {
                var parameter = Expression.Parameter(left.SourceType);
             
                var rightLambda = right.ValueLambda.GetLambda(parameter);
                // ensure both right and left use the same parameter
                var leftLambda = left.ValueLambda.GetLambda(parameter, rightLambda.ReturnType);
                var expression = CompareValue(leftLambda.Body, rightLambda.Body, rightLambda.ReturnType);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(left.SourceType, typeof(bool)), expression, parameter);

            }
            else
            {
                var parameter = Expression.Parameter(left.SourceType);
                var leftLambda = left.ValueLambda.GetLambda(parameter);
                // ensure both right and left use the same parameter
                var rightLambda = right.ValueLambda.GetLambda(parameter, leftLambda.ReturnType);
                var expression = CompareValue(leftLambda.Body, rightLambda.Body, leftLambda.ReturnType);
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(left.SourceType, typeof(bool)), expression, parameter);
            }
        }

        protected abstract Expression CompareValue(Expression left, Expression right, Type type);

        private object ChangeType(object value, Type valueType)
        {
            if (value == null)
            {
                return valueType.DefaultValue();
            }
            try
            {
                var originType = Nullable.GetUnderlyingType(valueType) ?? valueType;
                if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(originType))
                {
                    return Convert.ChangeType(value, originType);
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(originType);
                    return converter.ConvertFrom(value);
                }
            }
            catch (Exception ex)
            {
                throw FilterErrors.ConvertValueError(value, valueType, ex);
            }
        }


    }
}
