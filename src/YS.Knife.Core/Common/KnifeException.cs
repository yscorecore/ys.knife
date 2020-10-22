using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace YS.Knife
{


    [Serializable]
    public sealed class KnifeException : ApplicationException
    {
        public string Code { get; set; }
        
        public KnifeException(string code)
        {
            this.Code = code;
        }

        public KnifeException(string code, string message) : base(message)
        {
            this.Code = code;
        }
        public KnifeException(string code, string message, Exception inner) : base(message, inner)
        {
            this.Code = code;
        }
        protected KnifeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }


        public static KnifeException FromTemplate(string code, string template, params object[] args)
        {
            return FromTemplate(null, code, template, args);
        }
        public static KnifeException FromTemplate(Exception inner, string code, string template, params object[] args)
        {
            var messageFormatter = new LogValuesFormatter(template ?? code ?? string.Empty);
            var arguments = GetArguments(messageFormatter.ValueNames.Count, args);

            string message= messageFormatter.Format(arguments);
            var exception = new KnifeException(code, message, inner);
            foreach (var kv in messageFormatter.GetValues(arguments))
            {
                exception.Data[kv.Key] = kv.Value;
            }
            return exception;
        }
        private static object[] GetArguments(int placeholderLength, object[] args)
        {
            if (args == null)
            {
                return new object[placeholderLength];
            }
            else if (placeholderLength == args.Length)
            {
                return args;
            }
            else if (placeholderLength < args.Length)
            {
                return args.AsSpan(0, placeholderLength).ToArray();
            }
            else
            {
                var results = new object[placeholderLength];
                Array.Copy(args, results, args.Length);
                return results;
            }
        }
    }
}
