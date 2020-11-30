using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace YS.Knife.Data
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class EntityKeyAttribute : System.Attribute
    {
        public string[] Keys { get; set; }
        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        // This is a positional argument
        public EntityKeyAttribute(string positionalString)
        {
            this.positionalString = positionalString;

            // TODO: Implement code here
            throw new System.NotImplementedException();
        }

        public string PositionalString
        {
            get { return positionalString; }
        }
    }

    public sealed class DefaultOrderKeyAttribute:System.Attribute
    {
        public string PropName { get; set; }
        public OrderType OrderType {get;set;} = OrderType.Asc;
    }

    internal class LocalCache<TKey, TValue>
    {
        Dictionary<TKey, TValue> cachedData = new Dictionary<TKey, TValue>();

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
    public static class EntityExtensions
    {
        static LocalCache<Type, PropertyInfo[]> KeyCache = new LocalCache<Type, PropertyInfo[]>();
        public static PropertyInfo[] GetEntityKeyProps(this Type type)
        {
            return KeyCache.Get(type, (entityType) =>
            {
                var keyAttr = entityType.GetCustomAttributes(typeof(EntityKeyAttribute), true).OfType<EntityKeyAttribute>().FirstOrDefault();
                if (keyAttr != null)
                {
                    return keyAttr.Keys.Select(p=>type.GetProperty(p)).ToArray();
                }
                var idProp = entityType.GetProperties()
                     .Where(p => p.Name.Equals($"{entityType.Name}Id", StringComparison.InvariantCultureIgnoreCase) || p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                     .FirstOrDefault();
                if (idProp == null)
                {
                    throw new InvalidOperationException($"Can not find entity key property from type '{entityType.FullName}', please use '{typeof(EntityKeyAttribute).FullName}' to define the keys.");
                }
                return new PropertyInfo[] { idProp };
            });
        }
    }


}