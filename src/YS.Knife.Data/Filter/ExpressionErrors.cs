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
            return new FieldInfo2ExpressionException( $"create filter expression error: {exception.Message}",exception);
        }
    }
}
