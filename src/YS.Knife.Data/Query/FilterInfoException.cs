using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data
{
    public class FilterInfoException : ApplicationException
    {
        public FilterInfoException()
        {
        }

        public FilterInfoException(string message) : base(message)
        {
        }

        public FilterInfoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class FilterInfoParseException : FilterInfoException
    {
        public FilterInfoParseException()
        {
        }

        public FilterInfoParseException(string message) : base(message)
        {
        }

        public FilterInfoParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class FieldInfo2ExpressionException : FilterInfoException
    {
        public FieldInfo2ExpressionException()
        {
        }

        public FieldInfo2ExpressionException(string message) : base(message)
        {
        }

        public FieldInfo2ExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
