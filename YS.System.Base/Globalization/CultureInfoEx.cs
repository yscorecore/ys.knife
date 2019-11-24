using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Globalization
{
    /// <summary>
    /// 提供区域的一些辅助方法
    /// </summary>
    public static class CultureInfoEx
    {
        static CultureInfoEx()
        {
            foreach (var cul in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                cache.Add(cul.Name);
            }
        }
        private static HashSet<string> cache = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        /// <summary>
        /// 是否是有效的区域名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsCultureName(string name)
        {
            return cache.Contains(name);
        }
        /// <summary>
        /// 根据区域的名称取得一个区域信息,如果区域名称不存在，则返回空
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CultureInfo GetCulture(string name)
        {
            if (cache.Contains(name))
            {
                return new CultureInfo(name);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取默认的区域信息
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetDefaultCulture()
        {
            return new CultureInfo(string.Empty);
        }

    }
}
