using System;
using System.Collections.Generic;


namespace YS.Knife
{
    public class LocalCache<TKey, TValue>
    {
        readonly Dictionary<TKey, TValue> _cachedData = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key, Func<TKey, TValue> createFunc)
        {
            lock (_cachedData)
            {
                if (_cachedData.ContainsKey(key))
                {
                    return _cachedData[key];
                }
                return _cachedData[key] = createFunc(key);
            }
        }
    }
}
