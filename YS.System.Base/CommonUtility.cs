using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace System
{
    public static class CommonUtility
    {
        [Obsolete("请使用ObjectEx中的相应方法")]
        public static void AttachObjectPropertys(object oldobj, object newobj)
        {
            if (oldobj == null) return;
            if (newobj == null) return;
            Type curType = newobj.GetType();
            foreach (PropertyInfo p in oldobj.GetType().GetProperties())
            {
                PropertyInfo cp = curType.GetProperty(p.Name);
                if (cp != null && cp.PropertyType == p.PropertyType && p.CanRead && cp.CanWrite && cp.Name != "EntityKey" && cp.Name != "EntityState")
                {
                    cp.SetValue(newobj, p.GetValue(oldobj, null), null);
                }
            }
        }
         [Obsolete("请使用ObjectEx中的相应方法")]
        public static T AttachNewObject<T>(object oldobj)
        {
            if (oldobj == null) return default(T);
            Type curType = typeof(T);
            T curobj = Activator.CreateInstance<T>();
            foreach (PropertyInfo p in oldobj.GetType().GetProperties())
            {
                PropertyInfo cp = curType.GetProperty(p.Name);
                if (cp != null && cp.PropertyType == p.PropertyType && p.CanRead && cp.CanWrite && cp.Name != "EntityKey" && cp.Name != "EntityState")
                {
                    cp.SetValue(curobj, p.GetValue(oldobj, null), null);
                }
            }
            return curobj;
        }
         [Obsolete("请使用ObjectEx中的相应方法")]
        public static T Clone<T>(T currentobj)
            where T : class
        {
            if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
            {
                return (T)(currentobj as ICloneable).Clone();
            }
            else
            {
                return AttachNewObject<T>(currentobj);
            }
        }
         [Obsolete("请使用ObjectEx中的相应方法")]
        public static T DeepClone<T>(T currentObject)
        {
            if (currentObject != null)
            {
                using (System.IO.MemoryStream ms = new IO.MemoryStream())
                {
                    BinaryFormatter bf = new Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bf.Serialize(ms, currentObject);
                    ms.Seek(0, IO.SeekOrigin.Begin);
                    return (T)bf.Deserialize(ms);
                }
            }
            else
            {
                throw new ArgumentNullException("currentObject");
            }
        }
         [Obsolete("请使用ObjectEx中的相应方法")]
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
        /// <summary>
        /// 启动文本文件并显示内容
        /// </summary>
        /// <param name="text"></param>
        public static void QuickShowResult(string text)
        {
            string file = Path.GetTempFileName();
            file = file + ".txt";
            File.WriteAllText(file, text);
            Process.Start(file);
        }
        /// <summary>
        /// 启动文本文件并显示内容
        /// </summary>
        /// <param name="text"></param>
        public static void QuickShowResult(string[] lines)
        {
            string file = Path.GetTempFileName();
            file = file + ".txt";
            File.WriteAllLines(file, lines);
            Process.Start(file);
        }
        /// <summary>
        /// 启动csv文件并显示内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public static void QuickShowResult<T>(IEnumerable<T> items)
        {
            string file = Path.GetTempFileName();
            file = file + ".csv";
            System.IO.CsvFile.SaveCsvFile<T>(items, file);
            Process.Start(file);
        }

        /// <summary>
        /// 在原有的目录下面备份文件
        /// </summary>
        /// <param name="file">原文件的名称</param>
        /// <param name="bakExt">备份文件的后缀名</param>
        /// <returns>备份成功则返回true,否则返回false</returns>
        public static bool BackupFile(string file, string bakExt = ".obptbak")
        {
            if (File.Exists(file))
            {
                bakExt = string.IsNullOrEmpty(bakExt) ? ".obptbak" : bakExt;
                string newfile = file + bakExt;
                File.Copy(file, newfile, true);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 在原有的目录下面还原文件
        /// </summary>
        /// <param name="file">原文件的名称</param>
        /// <param name="bakExt">备份文件的后缀名</param>
        /// <returns>还原成功则返回true,否则返回false</returns>
        public static bool RestoreFile(string file, string bakExt = ".obptbak")
        {
            bakExt = string.IsNullOrEmpty(bakExt) ? ".obptbak" : bakExt;
            string newfile = file + bakExt;
            if (File.Exists(newfile))
            {
                File.Copy(newfile, file, true);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
