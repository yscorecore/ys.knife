using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class CsvFile
    {
        static Dictionary<string, string> replaceChars = new Dictionary<string, string> {
            {
                 "\"","\"\""
            }
        };
        /*
        每条记录占一行
        以逗号为分隔符
        逗号前后的空格会被忽略
        字段中包含有逗号，该字段必须用双引号括起来
        字段中包含有换行符，该字段必须用双引号括起来
        字段前后包含有空格，该字段必须用双引号括起来
        字段中的双引号用两个双引号表示
        字段中如果有双引号，该字段必须用双引号括起来
        */
        public static void SaveCsvFile<T>(IEnumerable<T> source, string fileName, Encoding encoding = null)
        {
            fileName = System.IO.PathEx.GetFullPath(fileName);
            using (Stream stream = File.OpenWrite(fileName))
            {
                using (StreamWriter sw = new StreamWriter(stream, encoding ?? Encoding.UTF8))
                {
                    var props = (from p
                                     in TypeDescriptor.GetProperties(typeof(T)).Cast<PropertyDescriptor>()
                                 where p.Converter != null && p.Converter.CanConvertTo(typeof(string))
                                 select p).ToArray();
                    //写头部
                    sw.WriteLine(string.Join(",", (from p in props select string.Format("\"{0}\"", p.DisplayName)).ToArray()));
                    foreach (var item in source)
                    {
                        string line = string.Join(",", (from p in props select string.Format("\"{0}\"", ToCSVItemText(p,item))).ToArray());
                        sw.WriteLine(line);
                    }
                }
            }
        }
        private static string ToCSVItemText(PropertyDescriptor pinfo, object obj)
        {
            if (obj == null) return "[NULL]";
            var val = pinfo.GetValue(obj);
            if (val == null) return "[NULL]";

            var text = pinfo.Converter.ConvertToString(val);
            return text.Replace(replaceChars);
          
        }

        public static IEnumerable<T> ReadCsvFile<T>(string fileName, Encoding encoding = null)
        {
            fileName = System.IO.PathEx.GetFullPath(fileName);
            using (Stream stream = File.OpenRead(fileName))
            {
                using (StreamReader sr = new StreamReader(stream, encoding ?? Encoding.UTF8))
                {
                    string title = sr.ReadLine();
                    if (title != null)
                    {
                        string[] propnames = title.Split(new char[] { ',' });
                        int columncount = propnames.Length;
                        Dictionary<int, System.ComponentModel.PropertyDescriptor> props = new Dictionary<int, PropertyDescriptor>();
                        var allproperties = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
                        for (int i = 0; i < propnames.Length; i++)
                        {
                            var p = allproperties.Find(propnames[i], true);
                            if (p == null) p = (from a in allproperties.Cast<PropertyDescriptor>() where a.DisplayName == propnames[i] select a).FirstOrDefault();
                            if (p != null && p.IsReadOnly == false && p.Converter != null && p.Converter.CanConvertFrom(typeof(string)))
                            {
                                props.Add(i, p);
                            }
                        }

                        //读内容
                        string contentline = null;
                        while ((contentline = sr.ReadLine()) != null)
                        {
                            string[] text = contentline.Split(new char[] { ',' });
                            if (text.Length == columncount)
                            {
                                T item = Activator.CreateInstance<T>();
                                foreach (var v in props)
                                {
                                    string desc = text[v.Key];
                                    v.Value.SetValue(item, v.Value.Converter.ConvertFromString(desc));
                                }
                                yield return item;
                            }
                        }
                    }
                }
            }
        }

    }
}
