using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Query
{
    internal class ValueConverter
    {
        public static ValueConverter Instance { get; } = new ValueConverter();

        public object ChangeType(object value, Type valueType)
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
