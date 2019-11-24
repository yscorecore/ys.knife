using System;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
namespace System
{
    public static class ObjectEx
    {
        public static void AttachObjectPropertys(object oldobj, object newobj, bool includeComplexProperty = true)
        {
            if (oldobj == null) return;
            if (newobj == null) return;
            Type curType = newobj.GetType();
            foreach (PropertyInfo p in oldobj.GetType().GetProperties())
            {
                if (!p.CanRead) continue;
                PropertyInfo cp = curType.GetProperty(p.Name);
                if (cp == null) continue;
                if (!cp.CanWrite) continue;
                if (!cp.PropertyType.IsAssignableFrom(p.PropertyType)) continue;//不是同一种类型不能赋值

                if (includeComplexProperty || !cp.PropertyType.IsComplexType())
                {
                    cp.SetValue(newobj, p.GetValue(oldobj, null), null);
                }

            }
        }
        public static T AttachNewObject<T>(object oldobj, bool includeComplexProperty = true)
        {
            if (oldobj == null) return default(T);
            Type curType = typeof(T);
            T curobj = Activator.CreateInstance<T>();
            foreach (PropertyInfo p in oldobj.GetType().GetProperties())
            {
                if (!p.CanRead) continue;
                PropertyInfo cp = curType.GetProperty(p.Name);
                if (cp == null) continue;
                if (!cp.CanWrite) continue;
                if (!cp.PropertyType.IsAssignableFrom(p.PropertyType)) continue;//不是同一种类型不能赋值

                if (includeComplexProperty || !cp.PropertyType.IsComplexType())
                {
                    cp.SetValue(curobj, p.GetValue(oldobj, null), null);
                }
            }
            return curobj;
        }
        public static T Clone<T>(this T currentobj)
            where T : class
        {
            if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
            {
                return (T)(currentobj as ICloneable).Clone();
            }
            else
            {
                return AttachNewObject<T>(currentobj, true);
            }
        }
        public static object CloneObject(object source)
        {
            if (source == null) return null;
            Type curType = source.GetType();


            if (typeof(ICloneable).IsAssignableFrom(curType))
            {
                return (source as ICloneable).Clone();
            }
            else
            {
                var newobj = Activator.CreateInstance(curType);
                AttachObjectPropertys(source, newobj, true);
                return newobj;
            }
        }
        public static object DeepCloneObject(object source)
        {
            if (source == null) return null;
            using (System.IO.MemoryStream ms = new IO.MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, source);
                ms.Seek(0, IO.SeekOrigin.Begin);
                return bf.Deserialize(ms);
            }
        }
        public static T DeepClone<T>(this T currentObject)
        {
            if (currentObject == null) return default(T);

            using (System.IO.MemoryStream ms = new IO.MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, currentObject);
                ms.Seek(0, IO.SeekOrigin.Begin);
                return (T)bf.Deserialize(ms);
            }

        }
        public static string CreateObjectInfo(object o, string silptChars = "\t=")
        {
            if (o != null)
            {
                StringBuilder sb = new StringBuilder();
                Type ty = o.GetType();
                PropertyInfo[] props = ty.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var p in props)
                {
                    if (p.CanRead)
                    {
                        try
                        {
                            sb.AppendFormat("{0}{2}{1}", p.Name, p.GetValue(o, null), silptChars);
                            sb.AppendLine();
                        }
                        catch (Exception ex)
                        {
                            sb.AppendFormat("{0}{2}{1}", p.Name, ex, silptChars);
                            sb.AppendLine();
                        }
                    }
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
