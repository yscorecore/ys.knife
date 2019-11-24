using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.Globalization
{
    /// <summary>
    /// 提供应用程序支持语言的方法
    /// </summary>
   public static class Languages
    {
        /// <summary>
        /// 获取应用程序支持的语言
        /// </summary>
        /// <returns></returns>
       public static CultureInfo[] GetSupportLanguageCultureInfos(bool includeInvariantCulture=false)
        {
            string path = System.IO.PathEx.GetLocalPathUnescaped(
                System.IO.PathEx.GetLocalPathUnescaped(Assembly.GetEntryAssembly().CodeBase)
                );
            string forlder = System.IO.Path.GetDirectoryName(path);
            return GetSupportLanguageCultureInfos(forlder,includeInvariantCulture);
        }

        /// <summary>
        /// 获取应用程序支持的语言
        /// </summary>
        /// <param name="forlder">应用程序所在的文件夹</param>
        /// <param name="includeInvariantCulture">是否包含固定的区域性</param>
        public static CultureInfo[] GetSupportLanguageCultureInfos(string forlder, bool includeInvariantCulture=false)
        {
            List<CultureInfo> lst = new List<CultureInfo>();
            if (includeInvariantCulture)
            {
                lst.Add(CultureInfo.InvariantCulture);
            }
            if (System.IO.Directory.Exists(forlder))
            {
                foreach (string s in System.IO.Directory.GetDirectories(forlder))
                {
                    string name = System.IO.Path.GetFileName(s);
                    var culture = CultureInfoEx.GetCulture(name);
                    if (culture != null)
                    {
                        lst.Add(culture);
                    }
                }
               
            }
           
            return lst.ToArray();
        }
    }
}
