using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Caching
{
    public static class CacheExtentions
    {
        public static T Get<T>(this ICacheManager cache, string key, TimeSpan ts, Func<T> fun)
        {
            return Get<T>(cache, key, ts, fun, null);
        }
        public static T Get<T>(this ICacheManager cache, string key, TimeSpan ts, Func<T> fun, Action<string, object, string> onRemove)
        {
            var res = cache.Get<T>(key);
            if (res)
            {
                return res.Item;
            }
            else
            {
                var data = fun();
                if (!object.ReferenceEquals(null, data))
                {
                    cache.Set(key, data, ts, onRemove);
                }
                return data;

            }
        }
        public static T Get<T>(this ICacheManager cache, string key, DateTime time, Func<T> fun)
        {
            return Get<T>(cache, key, time, fun, null);
        }
        public static T Get<T>(this ICacheManager cache, string key, DateTime time, Func<T> fun, Action<string, object, string> onRemove)
        {
            var res = cache.Get<T>(key);
            if (res)
            {
                return res.Item;
            }
            else
            {
                var data = fun();
                if (!object.ReferenceEquals(null, data))
                {
                    cache.Set(key, data, time, onRemove);
                }
                return data;
            }
        }

       
    }
}
