using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;

namespace System
{
    public static class ResourceManagerFactory
    {
        static Dictionary<string, ResourceManager> cache = new Dictionary<string, ResourceManager>(StringComparer.OrdinalIgnoreCase);
        public static ResourceManager GetResourceManager(Assembly assembly, string baseName, bool ignoreCase = true)
        {
           
            string key = string.Format("{0}_{1}_{2}", assembly.FullName, baseName, ignoreCase);
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }
            else
            {
                lock (cache)
                {
                    if (!cache.ContainsKey(key))
                    {
                        ResourceManager rm = new ResourceManager(baseName, assembly);
                        rm.IgnoreCase = ignoreCase;
                        cache.Add(key, rm);
                        return rm;
                    }
                    else
                    {
                        return cache[key];
                    }
                }
            }
        }
        /// <summary>
        /// 根据类型找对应的资源
        /// </summary>
        /// <param name="type"></param>
        /// <remarks>资源的名称必须和类型的名称相同并且在同一命名空间下面</remarks>
        /// <returns></returns>
        public static ResourceManager GetResourceManager(Type type, bool ignoreCase = true)
        {
            return GetResourceManager(type.Assembly, type.FullName, ignoreCase);
        }
        public static ResourceManager GetResourceManager(Type assemblyType, Type nameType, bool ignoreCase = true)
        {
            return GetResourceManager(assemblyType.Assembly, nameType.FullName, ignoreCase);
        }
    }
}
