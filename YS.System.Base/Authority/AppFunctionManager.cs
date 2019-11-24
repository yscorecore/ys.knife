using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Authority
{
    public static class AppFunctionManager
    {
        private static IAppFunctionProvider provider;
        public static IAppFunctionProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    provider = new LocalAssemblyAppFunctionProvider();
                }
                return provider;
            }
            set
            {
                provider = value;
            }
        }
        public static string ApplicationId
        {
            get
            {
                return Provider.ApplicationId;
            }
        }
        public static IEnumerable<FunctionInfo> GetChildrenFunctions(string functionCode)
        {
            return Provider.GetChildrenFunctions(functionCode);
        }
        public static FunctionInfo GetFunction(string functionCode)
        {
            return Provider.GetFunction(functionCode);
        }
        public static string GetFunctionCode( Type type)
        {
            return Provider.GetFunctionCode(type);
        }
        public static IEnumerable<FunctionInfo> GetFunctions(FunctionLevel deepLevel)
        {
            return Provider.GetFunctions(deepLevel);
        }
        public static string GetOperationCode(Type type, MethodInfo method)
        {
            return Provider.GetOperationCode(type, method);
        }
        public static bool HasPermission(string functionCode)
        {
            return Provider.HasPermission(functionCode);
        }
        public static bool HasPermission(Type type)
        {
            return Provider.HasPermission(type);
        }
        public static bool HasPermission(Type type, MethodInfo method)
        {
            return Provider.HasPermission(type, method);
        }



    }
}
