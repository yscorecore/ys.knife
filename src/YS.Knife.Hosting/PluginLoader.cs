using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace YS.Knife.Hosting
{
    class PluginLoader
    {
        public static void LoadPlugins(IEnumerable<string> plugins, bool throwIfException = true)
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            foreach (var file in Directory.GetFiles(rootPath, "*.dll", SearchOption.AllDirectories))
            {
                if (file.IsMatchWildcardAnyOne(plugins, StringComparison.InvariantCultureIgnoreCase))
                {
                    LoadAssembly(file, throwIfException);
                }
            }
        }
        private static void LoadAssembly(string dll, bool throwIfException)
        {
            try
            {
                Assembly.LoadFrom(dll);
            }
            catch (Exception ex)
            {
                if (throwIfException)
                {
                    throw;
                }
                else
                {
                    Trace.TraceError($"Load plugin \"{dll}\" error. {ex.ToString()}");
                }
            }
        }
    }
}
