using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public class PluginLoader
    {
        public static Assembly[] LoadPlugins(string rootPath, IEnumerable<string> wildcardPatterns, bool throwIfException = true)
        {
            List<Assembly> loadedAssemblies = new List<Assembly>();

            foreach (var dll in DirectoryEx.GetFiles(rootPath, wildcardPatterns, null))
            {
                try
                {
                    loadedAssemblies.Add(Assembly.LoadFrom(dll));
                }
                catch (Exception ex)
                {
                    if (throwIfException)
                    {
                        throw ex;
                    }
                    else
                    {
                        Trace.TraceError($"Load plugin \"{dll}\" error. {ex.ToString()}");
                    }
                }
            }

            return loadedAssemblies.ToArray();
        }
    }
}
