using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace YS.Knife.Loaders
{
    public class DictionaryLoader : IServiceLoader
    {
        public void LoadServices(IServiceCollection services, IRegisteContext context)
        {
            if (context.HasFiltered(typeof(IDictionary<,>))) return;
            services.AddTransient(typeof(IDictionary<,>), typeof(KnifeInjectionDictionary<,>));
        }

        protected class KnifeInjectionDictionary<Key, Value> : Dictionary<Key, Value>
        {
            public KnifeInjectionDictionary(IEnumerable<Value> items)
            {
                if (typeof(Key) != typeof(string))
                {
                    throw new InvalidOperationException("The key type of IDictionary<,> must be string.");
                }

                foreach (var item in items)
                {
                    if (item == null) continue;
                    Key key = (Key)(object)GetServiceKey(item.GetType());
                    if (this.ContainsKey(key))
                    {
                        throw new InvalidOperationException($"The key '{key}' has exists.");
                    }
                    this[key] = item;
                }
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
}
