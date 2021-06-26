using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data
{
    public class FilterException : ApplicationException
    {
        public FilterException()
        {
        }

        public FilterException(string message) : base(message)
        {
        }

        public FilterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class FilterParseException : FilterException
    {
        public FilterParseException()
        {
        }

        public FilterParseException(string message) : base(message)
        {
        }

        public FilterParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
