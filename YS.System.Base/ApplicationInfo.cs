using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// 提供应用程序的一些信息
    /// </summary>
    public class ApplicationInfo
    {
        #region 程序集特性访问器
        /// <summary>
        /// 获取应用程序的标题
        /// </summary>
        public static string Title
        {
            get
            {
                object[] attributes = EnvironmentEx.EntryAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }
        /// <summary>
        /// 获取应用程序的Guid
        /// </summary>
        public static string Guid
        {
            get
            {
                object[] attributes = EnvironmentEx.EntryAssembly.GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length > 0)
                {
                    GuidAttribute guidAttribute = (GuidAttribute)attributes[0];
                    return guidAttribute.Value;
                }
                return null;
            }
        }
        /// <summary>
        /// 获取应用程序的版本(AssemblyVersion)
        /// </summary>
        public static string Version
        {
            get
            {
                return EnvironmentEx.EntryAssembly.GetName().Version.ToString();
            }
        }
        /// <summary>
        /// 获取应用程序的描述信息
        /// </summary>
        public static string Description
        {
            get
            {
                object[] attributes = EnvironmentEx.EntryAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        /// <summary>
        /// 获取应用程序的产品信息
        /// </summary>
        public static string Product
        {
            get
            {
                object[] attributes = EnvironmentEx.EntryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        /// <summary>
        /// 获取应用程序的版权信息
        /// </summary>
        public static string Copyright
        {
            get
            {
                object[] attributes = EnvironmentEx.EntryAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        /// <summary>
        /// 获取应用程序的公司信息
        /// </summary>
        public static string Company
        {
            get
            {
                object[] attributes = EnvironmentEx.EntryAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }



        #endregion
    }


}
