using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace YS.Knife
{
    public class LocalCache<TKey, TValue>
    {
        readonly ConcurrentDictionary<TKey, TValue> _cachedData = new ConcurrentDictionary<TKey, TValue>();

        public TValue Get(TKey key, Func<TKey, TValue> createFunc)
        {
            return _cachedData.GetOrAdd(key, createFunc);
        }

        public TValue Get(TKey key)
        {
            if (_cachedData.TryGetValue(key, out var val))
            {
                return val;
            }
            return default;
        }
    }
}
