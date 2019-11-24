using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class AssemblyExtentions
    {
        public static IEnumerable<string> FindRefrences(this Assembly entry, params string[] wildcardPatterns)
        {
            var regexs = (wildcardPatterns ?? Enumerable.Empty<string>()).Select(WildcardToRegex);
            return from p in DependencyContext.Load(entry).RuntimeLibraries
                   let assembly = p.Name
                   where regexs.Any(r => Regex.IsMatch(assembly, r, RegexOptions.IgnoreCase))
                   select assembly;

        }

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        private static string WildcardToRegex(string pattern)
        {
            //. 为正则表达式的通配符，表示：与除 \n 之外的任何单个字符匹配。
            //* 为正则表达式的限定符，表示：匹配上一个元素零次或多次
            //? 为正则表达式的限定符，表示：匹配上一个元素零次或一次
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }

        public static void LoadRefrenceAssembly(this Assembly entry, string[] wildcardPatterns,Action<Assembly> callback)
        {
            foreach (var assemblyName in entry.FindRefrences(wildcardPatterns))
            {
                if (callback != null)
                {
                    callback(Assembly.Load(assemblyName));
                }
            }
        }
        public static IServiceCollection AddAssemblies(this IServiceCollection services, string[] wildcardPatterns, Action<Assembly> callback)
        {
            Assembly.GetEntryAssembly().LoadRefrenceAssembly(wildcardPatterns, callback);
            return services;
        }
    }
}
