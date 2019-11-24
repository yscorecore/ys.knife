using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System
{
    [Obsolete()]
    public sealed class SmartTypeService:ITypeService
    {
        #region 单例
        private static SmartTypeService instance;

        private static object instancelock = new object();

        private SmartTypeService()
        {
        }


        public static SmartTypeService Instance
        {
            get
            {
                lock(instancelock)
                {
                    if(object.ReferenceEquals(instance,null))
                    {
                        instance = new SmartTypeService();
                    }
                    return instance;
                }
            }
        }
        #endregion

        Dictionary<string,Type> dic = new Dictionary<string,Type>();

        public void AddAssemblyCache(Assembly assembly)
        {
            lock(dic)
            {
                foreach(Type ty in assembly.GetTypes())
                {
                    if(!dic.ContainsKey(ty.FullName))
                    {
                        dic.Add(ty.FullName,ty);
                    }
                    if(!dic.ContainsKey(ty.AssemblyQualifiedName))
                    {
                        dic.Add(ty.AssemblyQualifiedName,ty);
                    }
                }
            }
        }

        public void AddAssemblyCache(string assembly)
        {
            this.AddAssemblyCache(Assembly.Load(assembly));
        }

        public void AddTypeCache(string type,Type ty)
        {
            lock(dic)
            {
                if(!dic.ContainsKey(type))
                {
                    dic.Add(type,ty);
                }
            }
        }

        public Type GetTypeByName(string typeName)
        {
            if(string.IsNullOrEmpty(typeName)) return null;
            typeName = typeName.Trim();
            if(dic.ContainsKey(typeName))
            {
                return dic[typeName];
            }
            else
            {
                return Type.GetType(typeName);
            }
        }
    }
}

