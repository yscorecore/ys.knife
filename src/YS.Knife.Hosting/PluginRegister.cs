using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
namespace YS.Knife.Hosting
{
    class PluginRegister
    {
        public static void LoadPluginPaths(IEnumerable<string> paths, ILogger logger)
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var path in (paths ?? Enumerable.Empty<string>()).Distinct())
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;

                }
                string fullPath = Path.GetFullPath(Path.Combine(rootPath, path));
                if (!Directory.Exists(fullPath))
                {
                    continue;
                }
                foreach (var dllFile in Directory.GetFiles(fullPath, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    LoadAssembly(dllFile, logger);
                }
            }
        }

        private static void LoadAssembly(string dll, ILogger logger)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dll);
                logger.LogDebug("Load plugin assembly \"{@dll}\"", dll);
            }
#pragma warning disable CA1031 // 不捕获常规异常类型
            catch (Exception ex)
#pragma warning restore CA1031 // 不捕获常规异常类型
            {
                logger.LogError(ex, $"Load plugin assembly \"{@dll}\" error.", dll);
            }
        }
    }
}
