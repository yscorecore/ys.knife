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
        public static void LoadPlugins(IEnumerable<string> plugins, ILogger logger)
        {
            if (plugins == null) return;
            var (includes, ignores) = ParsePlugins(plugins);
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            foreach (var file in Directory.GetFiles(rootPath, "*.dll", SearchOption.AllDirectories))
            {
                string relativePath = file.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar);

                if (relativePath.IsMatchWildcardAnyOne(ignores, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (relativePath.IsMatchWildcardAnyOne(includes, StringComparison.InvariantCultureIgnoreCase))
                {
                    LoadAssembly(file, logger);
                }
            }
        }
        private static (List<string> Includes, List<string> Ignores) ParsePlugins(IEnumerable<string> plugins)
        {
            var allPlugins = plugins.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            var includes = allPlugins.Where(p => p[0] != '!').ToList();
            var ignores = allPlugins.Where(p => p[0] == '!')
                    .Select(p => p.Substring(1))
                    .Select(p => p.Replace('/', Path.DirectorySeparatorChar))
                    .Select(p => p.Replace('\\', Path.DirectorySeparatorChar))
                    .ToList();
            return (includes, ignores);
        }
        private static void LoadAssembly(string dll, ILogger logger)
        {
            try
            {
                Assembly.LoadFrom(dll);
                logger.LogInformation("Load plugin assembly \"{@dll}\"", dll);
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
