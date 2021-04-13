using System;
using System.Collections.Generic;


namespace YS.Knife
{
    public class LocalCache<TKey, TValue>
    {
        readonly Dictionary<TKey, TValue> cachedData = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key, Func<TKey, TValue> createFunc)
        {
            lock (cachedData)
            {
                if (cachedData.ContainsKey(key))
                {
                    return cachedData[key];
                }
                else
                {
                    return cachedData[key] = createFunc(key);
                }
            }
        }
    }
}
