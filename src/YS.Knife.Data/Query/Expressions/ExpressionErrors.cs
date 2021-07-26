using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Data.Query.Expressions
{
    internal static class ExpressionErrors
    {
        public static Exception ConvertValueError(object value, Type targetType, Exception exception = null)
        {
            return new QueryExpressionException($"convert value '{value}' to target type '{targetType.FullName}' error.", exception);
        }

        public static Exception MemberNotFound(string memberName)
        {
            return new QueryExpressionException($"property or field with name '{memberName}' not found.");
        }
    }
}
