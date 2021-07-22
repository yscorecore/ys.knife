using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter
{
    static class ExpressionErrors
    {
        public static FieldInfo2ExpressionException OperatorNotSupported(Operator @operator)
        {
            return new FieldInfo2ExpressionException($"the operator '{@operator}' not supported");
        }
        public static FieldInfo2ExpressionException CreateFilterExpressionError(Exception exception)
        {
            return new FieldInfo2ExpressionException($"create filter expression error: {exception.Message}", exception);
        }
        public static FieldInfo2ExpressionException ConvertValueError(object value, Type targetType, Exception exception = null)
        {
            return new FieldInfo2ExpressionException($"convert value '{value}' to target type '{targetType.Name}' error.", exception);
        }

        public static FieldInfo2ExpressionException CompareBothNullError()
        {
            return new FieldInfo2ExpressionException();
        }
        public static FieldInfo2ExpressionException TheOperatorCanOnlyUserForStringType(Operator @operator)
        {
            return new FieldInfo2ExpressionException($"the operator '{@operator}' can only use for string type");
        }

        public static FieldInfo2ExpressionException TheOperatorCanOnlyUserForComparableType(Operator @operator)
        {
            return new FieldInfo2ExpressionException($"the operator '{@operator}' can only use for comparable type");
        }
    }
}
