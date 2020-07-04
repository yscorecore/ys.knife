using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Registers
{
    public class DictionaryRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
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

            private static string GetServiceKey(Type type)
            {
                var dictionaryKeyAttr = type.GetCustomAttribute<DictionaryKeyAttribute>();
                if (dictionaryKeyAttr != null && !string.IsNullOrEmpty(dictionaryKeyAttr.Key))
                {
                    return dictionaryKeyAttr.Key;
                }
                else
                {
                    return type.FullName;
                }
            }
        }
    }
}
