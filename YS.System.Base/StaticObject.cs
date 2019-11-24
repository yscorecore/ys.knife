using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class StaticObject
    {
        private static Dictionary<Type, object> dic = new Dictionary<Type, object>();

        public static T GetOrCreateObject<T>(params object[] args)
        {
            var ty = typeof(T);
            lock (ty)
            {
                if (dic.ContainsKey(ty))
                {
                    return (T)dic[ty];
                }
                else
                {
                    return (T)(dic[ty] = Activator.CreateInstance(ty, args));
                }

            }
        }
        public static object GetOrCreateObject(Type ty, params object[] args)
        {
            if (ty == null) throw new ArgumentNullException("ty");
            lock (ty)
            {
                if (dic.ContainsKey(ty))
                {
                    return dic[ty];
                }
                else
                {
                    return (dic[ty] = Activator.CreateInstance(ty, args));
                }

            }
        }
        public static bool Release<T>()
        {
            var ty = typeof(T);
            lock (ty)
            {
               return dic.Remove(ty);
            }
        }
        public static bool Release(Type ty)
        {
            if (ty == null) throw new ArgumentNullException("ty");
            lock (ty)
            {
                return dic.Remove(ty);
            }
        }
    }
}
