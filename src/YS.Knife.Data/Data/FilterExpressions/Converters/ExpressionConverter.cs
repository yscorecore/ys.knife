using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{


    internal abstract class ExpressionConverter
    {
        public FilterType FilterType { get; set; }
        public abstract Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters);
        protected object ChangeType(object value, Type changeType)
        {
            if (changeType.IsEnum)
            {
                return Enum.ToObject(changeType, value);
            }
            else
            {
                return Convert.ChangeType(value, changeType, CultureInfo.InvariantCulture);
            }
        }
        protected string ChangeToString(object value)
        {
            return ChangeType(value, typeof(string)) as string;
        }
        protected bool IsNull(object obj)
        {
            return obj == null || obj == DBNull.Value;
        }
    }
}
