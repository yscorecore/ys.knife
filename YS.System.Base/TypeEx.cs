using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class TypeEx
    {
        /// <summary>
        /// 获取一个类型到指定的基类直接的距离,
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static int GetDistanceWith(this Type type, Type otherType)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (otherType == null) throw new ArgumentNullException("otherType");
            if (otherType.IsAssignableFrom(type))
            {
                int dep1 = GetDepth(type);
                int dep2 = GetDepth(otherType);
                return dep1 - dep2;
            }
            else
            {
                return -1;
            }

        }
        static Dictionary<Type, int> cache = new Dictionary<Type, int>();
        /// <summary>
        /// 获取一个类型的深度，即类型和System.Object之间的间距
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetDepth(this Type type)
        {
            if (cache.ContainsKey(type)) return cache[type];
            if (type == null) throw new ArgumentNullException("type");
            if (!type.IsClass) throw new ArgumentException("the type must by a class Type", "type");

            int depth = 0;
            Type currentType = type;
            Type objType = typeof(object);
            while (currentType != objType)
            {
                currentType = currentType.BaseType;
                depth++;
            }
            cache[type] = depth;
            return depth;
        }
        /// <summary>
        /// 调用泛型方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="genericTypes"></param>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeGenericMethod(this Type type, string methodName, Type[] genericTypes, object source, object[] parameters)
        {
            return type.GetMethod(methodName).MakeGenericMethod(genericTypes).Invoke(source, parameters);
        }

        public static bool IsNumericType(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type == typeof(int) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal) ||
                type == typeof(short) ||
                type == typeof(long) ||
                type == typeof(byte) ||
                type == typeof(uint) ||
                type == typeof(ushort) ||
                type == typeof(ulong) ||
                type == typeof(sbyte);
        }
        public static bool IsNullableNumericType(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var wraptype = Nullable.GetUnderlyingType(type);
            if (wraptype != null)
            {
                return IsNumericType(wraptype);
            }
            return false;
        }
        public static Type GetTypeInNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
        public static bool IsIntegerType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsNullableIntegerType(this Type type)
        {
            var wraptype = Nullable.GetUnderlyingType(type);
            if (wraptype != null)
            {
                return IsIntegerType(wraptype);
            }
            return false;
        }
        public static bool IsNullableType(this Type type)
        {
            if (type == null) return false;
            return Nullable.GetUnderlyingType(type) != null;
        }
        /// <summary>
        /// 判断类型是否是属于复杂属性（类，接口,集合等）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsComplexType(this Type type)
        {
            return type != typeof(string) && (type.IsClass || type.IsInterface);
        }
        /// <summary>
        /// 获取类型的全名称，包括程序集名称，但不包括Version，Culture，PublicKeyToken 等信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFullNameWithAssembly(this Type type)
        {
            var reg = new Regex(@"(?<ass>\w+(\.\w+)*)\s*,\s*Version\s*=\s*(?<ver>\d+\.\d+.\d+.\d+)\s*,\s*Culture\s*=\s*(?<cul>[a-z-]+)\s*,\s*PublicKeyToken\s*=\s*(?<key>\w+)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            return reg.Replace(type.AssemblyQualifiedName, (m)=> { return m.Groups["ass"].Value; }).RemoveSpace();
        }
    }
}
