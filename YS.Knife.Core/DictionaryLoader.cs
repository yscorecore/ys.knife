using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public class DictionaryLoader : IServiceLoader
    {
        public void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IDictionary<,>), typeof(KnifeInjectionDictionary<,>));
        }

        protected class KnifeInjectionDictionary<Key, Value> : Dictionary<string, Value>
        {
            public KnifeInjectionDictionary(IEnumerable<Value> items)
            {
                if (typeof(Key) != typeof(string))
                {
                    throw new InvalidOperationException("The key type of IDictionary<,> must be string.");
                }

                foreach (var item in items)
                {
                    if (item != null)
                    {
                        this[item.GetType().FullName] = item;
                    }
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
