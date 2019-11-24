using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public static class TypeExtentions
    {
        /// <summary>
        /// 获取指定类型下面的所有统一类型的静态属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ty"></param>
        /// <returns></returns>
        public static T[] GetStaticPropertyValues<T>(this Type ty)
        {
            List<T> lst = new List<T>();
            foreach (var pinfo in ty.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (pinfo.PropertyType == typeof(T) || typeof(T).IsAssignableFrom(pinfo.PropertyType))
                {
                    lst.Add((T)pinfo.GetValue(null, null));
                }
            }
            return lst.ToArray();
        }
        public static T GetStaticPropertyValue<T>(this Type ty, string name)
        {
            return (T)ty.GetProperty(name, BindingFlags.Public | BindingFlags.Static).GetValue(null, null);

        }
        /// <summary>
        /// 获取指定类型下面的所有统一类型的常量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ty"></param>
        /// <returns></returns>
        public static T[] GetConstValues<T>(this Type ty)
        {
            List<T> lst = new List<T>();
            foreach (var pinfo in ty.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (pinfo.FieldType == typeof(T) && pinfo.Attributes.HasFlagValue<FieldAttributes>(FieldAttributes.Literal))
                {
                    lst.Add((T)pinfo.GetValue(null));
                }
            }
            return lst.ToArray();
        }
        public static Dictionary<string, T> GetConstDictionary<T>(this Type ty, IEqualityComparer<string> keycomparer)
        {
            Dictionary<string, T> dic = keycomparer == null ? new Dictionary<string, T>() : new Dictionary<string, T>(keycomparer);
            foreach (var pinfo in ty.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                // pinfo.Attributes
                if (pinfo.FieldType == typeof(T) && pinfo.Attributes.HasFlagValue<FieldAttributes>(FieldAttributes.Literal))
                {
                    dic[pinfo.Name] = (T)pinfo.GetValue(null);
                }
            }
            return dic;
        }
        public static T GetConstValue<T>(this Type ty, string name)
        {
            return (T)ty.GetField(name, BindingFlags.Public | BindingFlags.Static).GetValue(null);
        }
        public static FieldInfo[] GetEnumFields(this Type ty)
        {
            return ty.GetFields(BindingFlags.Public | BindingFlags.Static);
        }
        public static FieldInfo GetEnumField(this Type ty, string name)
        {
            return ty.GetField(name, BindingFlags.Public | BindingFlags.Static);
        }
        #region 反射调用
        /// <summary>
        /// 反射调用对象或类型上的方法
        /// </summary>
        /// <param name="sourceOrType"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object InvokeMethod(this object sourceOrType, string methodName, params object[] args)
        {
            if (sourceOrType == null) throw new ArgumentNullException("sourceOrType");
            if (sourceOrType is Type)
            {
                Type ty = sourceOrType as Type;
                MethodInfo method = GetMethod(ty, methodName, args);
                if (method == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match method with the methodName='{0}'", methodName));
                }
                if (method.IsStatic)
                {
                    return method.Invoke(null, args);
                }
                else
                {
                    object instance = Activator.CreateInstance(ty);
                    return method.Invoke(instance, args);
                }
            }
            else
            {
                Type ty = sourceOrType.GetType();
                MethodInfo method = GetMethod(ty, methodName, args);
                if (method == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match method with the methodName='{0}'", methodName));
                }
                if (method.IsStatic)
                {
                    return method.Invoke(null, args);
                }
                else
                {
                    return method.Invoke(sourceOrType, args);
                }
            };
        }
        /// <summary>
        /// 反射获取对象或类型上的属性的值
        /// </summary>
        /// <param name="sourceOrType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object InvokeGetProperty(this object sourceOrType, string propertyName)
        {
            if (sourceOrType == null) throw new ArgumentNullException("sourceOrType");
            if (sourceOrType is Type)
            {
                Type ty = sourceOrType as Type;
                PropertyInfo property = GetProperty(ty, propertyName);
                if (property == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match property with the propertyName='{0}'", propertyName));
                }
                MethodInfo method = property.GetGetMethod(true);
                if (method.IsStatic)
                {
                    return method.Invoke(null, null);
                }
                else
                {
                    throw new InvalidOperationException("the property is an instance property ,but not provider the source");
                }
            }
            else
            {
                Type ty = sourceOrType.GetType();
                PropertyInfo property = GetProperty(ty, propertyName);
                if (property == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match property with the propertyName='{0}'", propertyName));
                }
                MethodInfo method = property.GetGetMethod(true);
                if (method.IsStatic)
                {
                    return method.Invoke(null, null);
                }
                else
                {
                    return method.Invoke(sourceOrType, null);
                }
            };
        }
        /// <summary>
        /// 反射设置对象或类型上的属性的值
        /// </summary>
        /// <param name="sourceOrType"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void InvokeSetProperty(this object sourceOrType, string propertyName, object value)
        {
            if (sourceOrType == null) throw new ArgumentNullException("sourceOrType");
            if (sourceOrType is Type)
            {
                Type ty = sourceOrType as Type;
                PropertyInfo property = GetProperty(ty, propertyName);
                if (property == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match property with the propertyName='{0}'", propertyName));
                }
                MethodInfo method = property.GetSetMethod(true);
                if (method.IsStatic)
                {
                    method.Invoke(null, new object[] { value });
                }
                else
                {
                    throw new InvalidOperationException("the property is an instance property ,but not provider the source");
                }
            }
            else
            {
                Type ty = sourceOrType.GetType();
                PropertyInfo property = GetProperty(ty, propertyName);
                if (property == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match property with the propertyName='{0}'", propertyName));
                }
                MethodInfo method = property.GetGetMethod(true);
                if (method.IsStatic)
                {
                    method.Invoke(null, new object[] { value });
                }
                else
                {
                    method.Invoke(sourceOrType, new object[] { value });
                }
            };
        }
        /// <summary>
        /// 反射获取对象或类型上的字段的值
        /// </summary>
        /// <param name="sourceOrType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object InvokeGetField(this object sourceOrType, string fieldName)
        {
            if (sourceOrType == null) throw new ArgumentNullException("sourceOrType");
            if (sourceOrType is Type)
            {
                Type ty = sourceOrType as Type;
                FieldInfo field = GetField(ty, fieldName);
                if (field == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match field with the fieldName='{0}'", fieldName));
                }
                if (field.IsStatic)
                {
                    return field.GetValue(null);
                }
                else
                {
                    throw new InvalidOperationException("the field is an instance field ,but not provider the source");
                }
            }
            else
            {
                Type ty = sourceOrType.GetType();
                FieldInfo field = GetField(ty, fieldName);
                if (field == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match field with the fieldName='{0}'", fieldName));
                }
                if (field.IsStatic)
                {
                    return field.GetValue(null);
                }
                else
                {
                    return field.GetValue(sourceOrType);
                }
            };
        }
        /// <summary>
        /// 反射设置对象或类型上的字段的值
        /// </summary>
        /// <param name="sourceOrType"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public static void InvokeSetField(this object sourceOrType, string fieldName, object value)
        {
            if (sourceOrType == null) throw new ArgumentNullException("sourceOrType");
            if (sourceOrType is Type)
            {
                Type ty = sourceOrType as Type;
                FieldInfo field = GetField(ty, fieldName);
                if (field == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match field with the filedName='{0}'", fieldName));
                }
                if (field.IsStatic)
                {
                    field.SetValue(null, value);
                }
                else
                {
                    throw new InvalidOperationException("the field is an instance field ,but not provider the source");
                }
            }
            else
            {
                Type ty = sourceOrType.GetType();
                FieldInfo field = GetField(ty, fieldName);
                if (field == null)
                {
                    throw new ArgumentException(string.Format("Can not find a match field with the filedName='{0}'", fieldName));
                }
                if (field.IsStatic)
                {
                    field.SetValue(null, value);
                }
                else
                {
                    field.SetValue(sourceOrType, value);
                }
            };
        }

        private static MethodInfo GetMethod(Type type, string methodName, object[] objs)
        {
            return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }
        private static PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }
        private static FieldInfo GetField(Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        }
        #endregion
        /// <summary>
        /// 获取泛型方法
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethod(this Type targetType, string name, BindingFlags flags, params Type[] parameterTypes)
        {
            var methods = targetType.GetMethods(flags).Where(m => m.Name == name && m.IsGenericMethod);
            foreach (MethodInfo method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != parameterTypes.Length)
                    continue;

                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        break;
                }
                return method;
            }
            return null;
        }
    }
}
