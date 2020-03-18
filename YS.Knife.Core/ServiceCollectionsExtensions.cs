using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace YS.Knife
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection RegisteKnifeServices(this IServiceCollection services, IConfiguration configuration)
        {
            foreach (var loaderType in AppDomain.CurrentDomain.FindInstanceTypesByBaseType<IServiceLoader>())
            {
                var loader = Activator.CreateInstance(loaderType) as IServiceLoader;
                loader.LoadServices(services, configuration);
            }
            return services;
        }
        public static bool IsFilter(this IServiceCollection services, Type type, IConfiguration configuration)
        {
            if (!KnifeOptionsCache.ContainsKey(configuration))
            {
                KnifeOptionsCache.Add(configuration, configuration.GetConfigOrNew<KnifeOptions>());
            }
            var knifeOptions = KnifeOptionsCache[configuration];
            return IsFilterAssembly(knifeOptions, type.Assembly) || IsFilterType(knifeOptions, type);
        }
        private static bool IsFilterAssembly(KnifeOptions knifeOptions, Assembly assembly)
        {
            var ignoreAssemblies = knifeOptions.Ignores?.Assemblies ?? new List<string>();
            return assembly.GetName().Name.IsMatchWildcardAnyOne(ignoreAssemblies, StringComparison.InvariantCultureIgnoreCase);
        }
        private static bool IsFilterType(KnifeOptions knifeOptions, Type type)
        {
            var ignoreTypes = knifeOptions.Ignores?.Types ?? new List<string>();
            return type.FullName.IsMatchWildcardAnyOne(ignoreTypes, StringComparison.InvariantCultureIgnoreCase);
        }
        private static IDictionary<IConfiguration, KnifeOptions> KnifeOptionsCache = new Dictionary<IConfiguration, KnifeOptions>();
    }
}