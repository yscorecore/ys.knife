using System.Collections.Generic;
using System.Linq;

namespace YS.Knife
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> TrimNotNull<T>(this IEnumerable<T> items)
            where T : class
        {
            return items == null ? Enumerable.Empty<T>() : items.Where(p => p != null);
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> items)
        {
            return items != null && items.Any();
        }

        public static bool IsEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }

    }
}
