using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{


    internal abstract class ExpressionConverter
    {
        public Operator FilterType { get; set; }
        public abstract Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters);
        protected object ChangeType(object value, Type changeType)
        {
            return changeType.IsEnum ? Enum.ToObject(changeType, value) : Convert.ChangeType(value, changeType, CultureInfo.InvariantCulture);
        }
        protected string ChangeToString(object value)
        {
            return ChangeType(value, typeof(string)) as string;
        }
        protected static bool IsNull(object obj)
        {
            return obj == null || obj == DBNull.Value;
        }
    }
}
