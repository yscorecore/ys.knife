using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace YS.Knife.Rest.Client
{
    static class ObjectUtils
    {
        public static IEnumerable<KeyValuePair<string, string>> ObjectToStringKeyValuePairs(object obj)
        {
            if (obj == null) return Enumerable.Empty<KeyValuePair<string, string>>();
            if (obj is IEnumerable<KeyValuePair<string, string>>)
            {
                return obj as IEnumerable<KeyValuePair<string, string>>;
            }
            if (obj is IDictionary<string, object> objDic)
            {
                return objDic.Where(kv => !String.IsNullOrEmpty(kv.Key))
                    .SelectMany(p => GetMultiValues(p.Key, p.Value));
            }

            return obj.GetType().GetProperties()
                 .Where(p => p.CanRead)
                 .SelectMany(p => GetMultiValues(p.Name, p.GetValue(obj)));
        }
        public static IDictionary<string, string> ObjectToStringDictionary(object obj, bool ignoreCase = true)
        {

            var comparer = ignoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            if (obj == null) return new Dictionary<string, string>(comparer);

            if (obj is IDictionary<string, string> strDic)
            {
                return new Dictionary<string, string>(strDic, comparer);
            }

            if (obj is IDictionary<string, object> objDic)
            {
                var origins = objDic.Where(kv => !string.IsNullOrEmpty(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => ValueToString(kv.Value));
                return new Dictionary<string, string>(origins, comparer);
            }

            var keyValues = obj.GetType().GetProperties()
               .Where(p => p.CanRead)
               .ToDictionary(p => p.Name, p => ValueToString(p.GetValue(obj)));
            return new Dictionary<string, string>(keyValues, comparer);
        }
        private static IEnumerable<KeyValuePair<string, string>> GetMultiValues(string name, object value)
        {
            if ((value is IEnumerable valueList) && !(value is string))
            {
                foreach (var item in valueList)
                {
                    yield return new KeyValuePair<string, string>(name, ValueToString(item));
                }
            }
            else
            {
                yield return new KeyValuePair<string, string>(name, ValueToString(value));
            }
        }
        public static string ValueToString(object value)
        {
            if (value == null) return null;
            if (value is string) return value as string;
            var converter = TypeDescriptor.GetConverter(value);
            if (converter == null || !converter.CanConvertTo(typeof(string))) return value.ToString();
            return converter.ConvertToInvariantString(value);
        }
    }
}
