using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Caching
{
    /// <summary>
    /// 缓存管理接口
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 获取缓存，取不到则抛异常
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">该缓存项的唯一标识符</param>
        /// <returns>The value associated with the specified key.</returns>
        FlagData<T> Get<T>(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">该缓存项的唯一标识符</param>
        /// <param name="data">要插入的对象</param>
        /// <param name="cacheTime"> 该值指示如果某个缓存项在给定时段内未被访问，是否应被逐出。</param>
        void Set(string key, object data, TimeSpan cacheTime, Action<string, object, string> onRemove);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expireTime"></param>
        void Set(string key, object data, DateTime expireTime, Action<string, object, string> onRemove);
        /// <summary>
        /// 是否存在缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool Contains(string key);

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 清空缓存
        /// </summary>
        void Clear();
    }


}
