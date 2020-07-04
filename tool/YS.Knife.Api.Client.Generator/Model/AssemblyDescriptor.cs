using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace YS.Knife.Api.Client.Generator.Model
{
    public class AssemblyDescriptor
    {
        public string Version { get; private set; }
        public string FileVersion { get; private set; }
        public string InformationalVersion { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Product { get; private set; }
        public string Configuration { get; private set; }
        public string Company { get; private set; }
        public string Description { get; private set; }

        public IList<ControllerDescriptor> Controllers { get; private set; }
        public static AssemblyDescriptor FromAssembly(Assembly assembly)
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));
            return new AssemblyDescriptor
            {
                Name = assembly.GetName().Name,
                Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title,
                Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
                Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company,
                Configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration,
                Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product,
                Version = assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version,
                FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version,
                InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
                Controllers = FindControllerTypes(assembly).Select(ControllerDescriptor.FromControllerType).ToList()
            };
        }
        public static AssemblyDescriptor FromAssembly(string assemblyFile)
        {
            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
            return FromAssembly(assembly);
        }
        private static IEnumerable<Type> FindControllerTypes(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }
    }
}
