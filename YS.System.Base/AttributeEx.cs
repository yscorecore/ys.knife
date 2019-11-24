using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class AttributeEx
    {
        public static T GetCustomAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            if (member == null) return default(T);
            return Attribute.GetCustomAttributes(member, typeof(T)).Cast<T>().FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this Assembly assembly)
            where T : Attribute
        {
            if (assembly == null) return default(T);
            return Attribute.GetCustomAttributes(assembly, typeof(T)).Cast<T>().FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this Module module)
           where T : Attribute
        {
            if (module == null) return default(T);
            return Attribute.GetCustomAttributes(module, typeof(T)).Cast<T>().FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this ParameterInfo parameter)
           where T : Attribute
        {
            if (parameter == null) return default(T);
            return Attribute.GetCustomAttributes(parameter, typeof(T)).Cast<T>().FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit)
           where T : Attribute
        {
            if (member == null) return default(T);
            return Attribute.GetCustomAttributes(member, typeof(T), inherit).Cast<T>().FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this Assembly assembly, bool inherit)
            where T : Attribute
        {
            if (assembly == null) return default(T);
            return Attribute.GetCustomAttributes(assembly, typeof(T), inherit).Cast<T>().FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this Module module, bool inherit)
           where T : Attribute
        {
            if (module == null) return default(T);
            return Attribute.GetCustomAttributes(module, typeof(T), inherit).Cast<T>().FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this ParameterInfo parameter, bool inherit)
           where T : Attribute
        {
            if (parameter == null) return default(T);
            return Attribute.GetCustomAttributes(parameter, typeof(T), inherit).Cast<T>().FirstOrDefault();
        }
        public static T[] GetCustomAttributes<T>(this MemberInfo member)
          where T : Attribute
        {
            if (member == null) return new T[0];
            return Attribute.GetCustomAttributes(member, typeof(T)).Cast<T>().ToArray();
        }
        public static T[] GetCustomAttributes<T>(this Assembly assembly)
            where T : Attribute
        {
            if (assembly == null) return new T[0];
            return Attribute.GetCustomAttributes(assembly, typeof(T)).Cast<T>().ToArray();
        }
        public static T[] GetCustomAttributes<T>(this Module module)
           where T : Attribute
        {
            if (module == null) return new T[0];
            return Attribute.GetCustomAttributes(module, typeof(T)).Cast<T>().ToArray();
        }
        public static T[] GetCustomAttributes<T>(this ParameterInfo parameter)
           where T : Attribute
        {
            if (parameter == null) return new T[0];
            return Attribute.GetCustomAttributes(parameter, typeof(T)).Cast<T>().ToArray();
        }
        public static T[] GetCustomAttributes<T>(this MemberInfo member, bool inherit)
           where T : Attribute
        {
            if (member == null) return new T[0];
            return Attribute.GetCustomAttributes(member, typeof(T), inherit).Cast<T>().ToArray();
        }
        public static T[] GetCustomAttributes<T>(this Assembly assembly, bool inherit)
            where T : Attribute
        {
            if (assembly == null) return new T[0];
            return Attribute.GetCustomAttributes(assembly, typeof(T), inherit).Cast<T>().ToArray();
        }
        public static T[] GetCustomAttributes<T>(this Module module, bool inherit)
           where T : Attribute
        {
            if (module == null) return new T[0];
            return Attribute.GetCustomAttributes(module, typeof(T), inherit).Cast<T>().ToArray();
        }
        public static T[] GetCustomAttributes<T>(this ParameterInfo parameter, bool inherit)
           where T : Attribute
        {
            if (parameter == null) return new T[0];
            return Attribute.GetCustomAttributes(parameter, typeof(T), inherit).Cast<T>().ToArray();
        }
    }
}
