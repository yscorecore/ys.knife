using System.Linq;
using YS.Knife;

namespace System.Collections.Generic
{
    public static class CollectionExtentions
    {
        public static IDictionary<string, T> ToServiceDictionary<T>(this IEnumerable<T> services)
        {
            return services.Where(p => p != null).ToDictionary(p => GetServiceKey(p.GetType()));
        }
        private static string GetServiceKey(Type type)
        {
            ServiceClassAttribute serviceImplClass = Attribute.GetCustomAttribute(type, typeof(ServiceClassAttribute)) as ServiceClassAttribute;
            if (serviceImplClass == null || string.IsNullOrEmpty(serviceImplClass.Key))
            {
                return type.FullName;
            }
            else
            {
                return serviceImplClass.Key;
            }
        }
    }
}
