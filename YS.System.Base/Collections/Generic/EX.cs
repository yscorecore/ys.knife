using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class EX
    {
        /// <summary>
        /// 判断一个可枚举类型是否为空或者元素的个数为0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || items.Count() == 0;
        }
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items != null && action != null)
            {
                foreach (var v in items)
                {
                    action(v);
                }
            }
        }
    }
}
