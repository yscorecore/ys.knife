using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace System
{
    static class Extensions
    {
        public static string FormatEx(this string template, object arg)
        {
            var messageFormatter = new LogValuesFormatter(template ?? string.Empty);
            var arguments = ObjectToStringDictionary(arg, false);
            var formatArguments = messageFormatter.ValueNames.Select(p =>
            {
                if (arguments.TryGetValue(p, out var value))
                {
                    return value;
                }
                return null;

            }).ToArray();
            return template;
        }
        private static IDictionary<string, object> ObjectToStringDictionary(object obj, bool ignoreCase = true)
        {

            var comparer = ignoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            if (obj == null) return new Dictionary<string, object>(comparer);

            if (obj is IDictionary<string, object> strDic)
            {
                return new Dictionary<string, object>(strDic, comparer);
            }

            if (obj is IDictionary<string, object> objDic)
            {
                var origins = objDic.Where(kv => !string.IsNullOrEmpty(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                return new Dictionary<string, object>(origins, comparer);
            }

            var kvalues = obj.GetType().GetProperties()
               .Where(p => p.CanRead)
               .ToDictionary(p => p.Name, p => p.GetValue(obj));
            return new Dictionary<string, object>(kvalues, comparer);
        }
    }
}
