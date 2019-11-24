using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace System
{
    public static class EnvironmentEx
    {
       
      


       

        /// <summary>
        /// 获取应用程序的路径(winform or webfrom)
        /// </summary>
        public static string ApplicationDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

       
       
        public static void SetDataDirectory(this AppDomain domain, string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = domain.BaseDirectory;
            }
            domain.SetData("DataDirectory", path);
        }
        public static void SetCurrentIdentity(this AppDomain domain, string currentIdentityName, string[] roles)
        {
            var gi = new GenericIdentity(currentIdentityName);
            var principal = new GenericPrincipal(gi, roles);
            domain.SetThreadPrincipal(principal);
        }
        /// <summary>
        /// 获取当前的用户标示名称
        /// </summary>
        public static string CurrentIdentityName
        {
            get
            {
                var cp = System.Threading.Thread.CurrentPrincipal;
                var id = cp != null ? cp.Identity : null;
                return id != null ? id.Name : string.Empty;
            }
        }
        /// <summary>
        /// 获取EntryAssembly
        /// </summary>
        public static Assembly EntryAssembly
        {
            get
            {
                return EntryAssemblyAttribute.GetEntryAssembly();
            }
        }
    }


}
