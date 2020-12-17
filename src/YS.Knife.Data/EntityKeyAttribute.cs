using System;
using System.Linq;
using System.Reflection;

namespace YS.Knife.Data
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class EntityKeyAttribute : System.Attribute
    {
        public string[] Keys { get; set; }
        public EntityKeyAttribute(params string[] keys)
        {
            _ = keys ?? throw new ArgumentNullException(nameof(keys));
            if (keys.Length == 0)
            {
                throw new ArgumentException("Must provide one key.");
            }
            this.Keys = keys;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DefaultOrderByAttribute : System.Attribute
    {
        public DefaultOrderByAttribute(string propName, OrderType orderType = OrderType.Asc)
        {
            this.OrderInfo = new OrderItem(propName, orderType).ToString();
        }
        public string OrderInfo { get;  }
    }


    public static class EntityExtensions
    {
        static readonly LocalCache<Type, PropertyInfo[]> KeyCache = new LocalCache<Type, PropertyInfo[]>();
        static readonly LocalCache<Type, OrderInfo> OrderByCache = new LocalCache<Type, OrderInfo>();
        public static PropertyInfo[] GetEntityKeyProps(this Type type)
        {
            return KeyCache.Get(type, (entityType) =>
            {
                var keyAttr = entityType.GetCustomAttributes(typeof(EntityKeyAttribute), true).OfType<EntityKeyAttribute>().FirstOrDefault();
                if (keyAttr != null)
                {
                    return keyAttr.Keys.Select(type.GetProperty).ToArray();
                }
                var idProp = entityType
                    .GetProperties()
                    .FirstOrDefault(p => p.Name.Equals($"{entityType.Name}Id", StringComparison.InvariantCultureIgnoreCase) || p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase));
                if (idProp == null)
                {
                    throw new InvalidOperationException($"Can not find entity key property from type '{entityType.FullName}', please use '{typeof(EntityKeyAttribute).FullName}' to define the keys.");
                }
                return new PropertyInfo[] { idProp };
            });
        }
        public static OrderInfo GetEntityDefaultOrderInfo(this Type type)
        {
            return OrderByCache.Get(type, (entityType) =>
            {
                var orderByAttr = entityType.GetCustomAttribute<DefaultOrderByAttribute>();
                if (orderByAttr != null)
                {
                    return OrderInfo.Parse(orderByAttr.OrderInfo);
                }
                else
                {
                    var keyOrderItems = type.GetEntityKeyProps().Select(p => new OrderItem(p.Name, OrderType.Asc));
                    return new OrderInfo(keyOrderItems);
                }
            });
        }
    }


}
