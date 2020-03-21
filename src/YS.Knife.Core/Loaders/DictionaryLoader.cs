using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YS.Knife.Loaders
{
    public class DictionaryLoader : IServiceLoader
    {
        public void LoadServices(IServiceCollection services, IRegisteContext context)
        {
            if (context.HasFiltered(typeof(IDictionary<,>))) return;
            services.AddTransient(typeof(IDictionary<,>), typeof(KnifeInjectionDictionary<,>));
        }

        protected class KnifeInjectionDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public KnifeInjectionDictionary(IEnumerable<TValue> items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                if (typeof(TKey) != typeof(string))
                {
                    throw new InvalidOperationException("The key type of IDictionary<,> must be string.");
                }

                foreach (var item in items)
                {
                    if (item == null) continue;
                    TKey key = (TKey)(object)GetServiceKey(item.GetType());
                    if (this.ContainsKey(key))
                    {
                        throw new InvalidOperationException($"The key '{key}' has exists.");
                    }
                    this[key] = item;
                }
            }

            [SuppressMessage("样式", "IDE0019:使用模式匹配", Justification = "<挂起>")]
            private static string GetServiceKey(Type type)
            {
                var serviceImplClass = Attribute.GetCustomAttribute(type, typeof(ServiceClassAttribute)) as ServiceClassAttribute;
                if (serviceImplClass != null && !string.IsNullOrEmpty(serviceImplClass.Key))
                {
                    return serviceImplClass.Key;
                }
                else
                {
                    return type.FullName;
                }
            }
        }
    }
}
