using System;
using System.Collections.Generic;


namespace YS.Knife
{
    [Serializable]
    public class CodeException : ApplicationException
    {
        public string Code { get; set; }

        public CodeException()
        {
        }

        public CodeException(string code)
        {
            this.Code = code;
        }

        public CodeException(string code, string message) : base(message)
        {
            this.Code = code;
        }

        public CodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CodeException(string code, string message, Exception inner) : base(message, inner)
        {
            this.Code = code;
        }

        protected CodeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public CodeException WithException(Exception exception)
        {
            return new CodeException(this.Code, this.Message, exception);
        }

        public CodeException WithData(IDictionary<string, object> errorData)
        {
            if (errorData == null)
            {
                return this;
            }

            foreach (var item in errorData)
            {
                WithData(item.Key, item.Value);
            }
            return this;
        }

        public CodeException WithData(string key, object value)
        {
            this.Data[key] = value;
            return this;
        }
    }
}
