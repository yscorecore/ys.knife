using System;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Data.Filter;

namespace YS.Knife.Data.Query.Expressions
{
    internal class ConstValueLambdaProvider<TSource> : IValueLambdaProvider
    {
        public ConstValueLambdaProvider(object constValue)
        {
            this.ConstValue = constValue;
        }

        public object ConstValue { get; }

        public Type SourceType => typeof(TSource);

        public LambdaExpression GetLambda(ParameterExpression parameter)
        {
            var targetType = ConstValue?.GetType() ?? typeof(string);
            var constExp = Expression.Constant(this.ConstValue, targetType);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, targetType), constExp, parameter);
        }

        public LambdaExpression GetLambda(ParameterExpression parameter, Type targetType)
        {
            _ = targetType ?? throw new ArgumentNullException(nameof(targetType));
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

                if (originType == typeof(DateTime) || originType == typeof(DateTimeOffset))
                {
                    if (value is double or long or int or float or decimal)
                    {
                        // support unix time stamp
                        long longValue = (long)ChangeType(value, typeof(long));
                        var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(longValue);
                        return GetDateTimeByTargetType(originType, dateTimeOffset);
                    }
                    if (value is string str)
                    {
                        var dateTimeOffset = DateTimeOffset.Parse(str);
                        return GetDateTimeByTargetType(originType, dateTimeOffset);
                    }
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
            catch (QueryExpressionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ExpressionErrors.ConvertValueError(value, valueType, ex);
            }
        }



        private static object GetDateTimeByTargetType(Type targetType, DateTimeOffset dateTimeOffset)
        {
            if (targetType == typeof(DateTimeOffset))
            {
                return dateTimeOffset;
            }
            else
            {
                return dateTimeOffset.DateTime;
            }
        }

        private static object ChangeToEnumType(object value, Type enumType)
        {
            if (value is string str)
            {
                return Enum.Parse(enumType, str, true);
            }
            else
            {
                return Enum.ToObject(enumType, Convert.ChangeType(value, Enum.GetUnderlyingType(enumType)));
            }
        }
    }
}
