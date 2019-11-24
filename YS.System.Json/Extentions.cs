using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace System.Json
{
    [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
    public static class Extentions
    {
        /// <summary>
        /// 将对象序列化成json字符串
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToJsonText(this object o)
        {
            return JsonConvert.SerializeObject(o);
        }
        public static string ToJsonText(this object o, bool indented)
        {
            if (indented)
            {
                return JsonConvert.SerializeObject(o, Formatting.Indented);
            }
            else
            {
                return JsonConvert.SerializeObject(o);
            }
        }
        /// <summary>
        /// 将Json格式的字符串转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T AsJsonObject<T>(this string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
        /// <summary>
        /// 将Json格式的字符串转换为动态对象
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static dynamic AsDynamicJsonObject(this string text)
        {
            return JObject.Parse(text);
        }
 
    }
}
