using System;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Data.Filter;

namespace YS.Knife.Data.Query.Expressions
{
    public class ConstValueLambdaProvider<TSource> : IFuncLambdaProvider
    {
        public ConstValueLambdaProvider(object constValue)
        {
            this.ConstValue = constValue;
        }

        public object ConstValue { get; }

        public Type SourceType => typeof(TSource);

        public LambdaExpression GetLambda()
        {
            var parameter = Expression.Parameter(SourceType, "p");
            var targetType = ConstValue?.GetType() ?? typeof(string);
            var constExp = Expression.Constant(this.ConstValue, targetType);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, targetType), constExp, parameter);
        }

        public LambdaExpression GetLambda(Type targetType)
        {
            _ = targetType ?? throw new ArgumentNullException(nameof(targetType));
            var parameter = Expression.Parameter(typeof(TSource), "p");
            var constExp = Expression.Constant(ChangeType(this.ConstValue, targetType), targetType);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, targetType), constExp, parameter);
        }

        private object ChangeType(object value, Type valueType)
        {
            if (value == null)
            {
                return valueType.DefaultValue();
            }

            try
            {
                var originType = Nullable.GetUnderlyingType(valueType) ?? valueType;

                if (originType.IsEnum)
                {
                    return ChangeToEnumType(value, originType);
                }

                if (originType == typeof(DateTime)|| originType == typeof(DateTimeOffset))
                {
                    return ChangeNumberToDateTimeType(value, originType);
                }

              
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
                throw ExpressionErrors.ConvertValueError(value, valueType, ex);
            }
        }

        private object ChangeNumberToDateTimeType(object value, Type originType)
        {
            if (value is double or long or int or float or decimal)
            {
                // support unix time stamp
                long longValue = (long)ChangeType(value, typeof(long));
                var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(longValue);
                if (originType == typeof(DateTimeOffset))
                {
                    return dateTimeOffset;
                }
                else
                {
                    return dateTimeOffset.DateTime;
                }
            }
            var converter = TypeDescriptor.GetConverter(originType);
            return converter.ConvertFrom(value);
        }

        private static object ChangeToEnumType(object value, Type? originType)
        {
            if (value is string str)
            {
                return Enum.Parse(originType, str, true);
            }
            else
            {
                return Enum.ToObject(originType, Convert.ChangeType(value, Enum.GetUnderlyingType(originType)));
            }
        }
    }
}
