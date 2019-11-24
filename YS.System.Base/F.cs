using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class F
    {
        private static List<Func<Type, object>> FactoryChains = new List<Func<Type, object>>();


        public static void RegistChain(Func<Type,object> factory)
        {
            if (factory != null)
            {
                FactoryChains.Add(factory);
            }
        }

        public static T Get<T>()
            where T:class
        {
            foreach (var factory in FactoryChains)
            {
                var instance = factory(typeof(T));
                if (instance != null)
                {
                    return instance as T;
                }
            }
            return default(T);
        }
    }
}
