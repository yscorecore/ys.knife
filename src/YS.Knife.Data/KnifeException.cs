using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YS.Knife
{
    [Serializable]
    public class KnifeException : ApplicationException
    {
        public int Code { get; set; }
        public KnifeException()
        {
        }
        public KnifeException(int code)
        {
            this.Code = code;
        }

        public KnifeException(int code, string message) : base(message)
        {
            this.Code = code;
        }
        public KnifeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public KnifeException(int code, string message, Exception inner) : base(message, inner)
        {
            this.Code = code;
        }
        protected KnifeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public static KnifeException FromTemplate(int code, string message)
        {
            return FromTemplate(code, message, null);
        }
        public static KnifeException FromTemplate(int code, string template, object args)
        {
            return FromTemplate(null, code, template, args);
        }
        public static KnifeException FromTemplate(Exception inner, int code, string message)
        {
            return FromTemplate(inner, code, message, null);
        }
        public static KnifeException FromTemplate(Exception inner, int code, string template, object args)
        {
            var messageFormatter = new ValuesFormatter(template ?? string.Empty);
            var argumentMap = ObjectToStringDictionary(args);
            var argumentArray = messageFormatter.ValueNames.Select(p =>
            {
                if (argumentMap.TryGetValue(p, out var val))
                {
                    return val;
                }
                return null;
            }).ToArray();
            string message = messageFormatter.Format(argumentArray);
            var exception = new KnifeException(code, message, inner);

            foreach (var name in messageFormatter.ValueNames)
            {
                if (argumentMap.TryGetValue(name, out var value))
                {
                    exception.Data[name] = value;
                }
            }
            return exception;
        }
        private static IDictionary<string, object> ObjectToStringDictionary(object obj)
        {
            if (obj == null) return new Dictionary<string, object>();
            var objType = obj.GetType();
            if (Type.GetTypeCode(objType) != TypeCode.Object)
            {
                // sample value
                return new Dictionary<string, object>
                {
                    ["0"] = obj
                };
            }
            if (objType.GetInterfaces().Any(p => p == typeof(IDictionary)))
            {
                var dic = obj as IDictionary;
                return dic.Keys.OfType<string>().ToDictionary(p => p, p => dic[p]);
            }
            if (obj is IEnumerable enumerable)
            {
                return enumerable.OfType<object>().Select(
                    (p, index) => new KeyValuePair<string, object>(index.ToString(CultureInfo.InvariantCulture), p)
                ).ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            var keyValues = objType.GetProperties()
               .Where(p => p.CanRead)
               .ToDictionary(p => p.Name, p => p.GetValue(obj));
            return new Dictionary<string, object>(keyValues);
        }


    }
}
