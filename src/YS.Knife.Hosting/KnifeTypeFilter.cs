using System;
using System.Collections.Generic;
using System.Reflection;

namespace YS.Knife.Hosting
{
    public class KnifeTypeFilter
    {
        public KnifeTypeFilter(KnifeOptions knifeOptions)
        {
            this.knifeOptions = knifeOptions;
        }
        private readonly KnifeOptions knifeOptions;
        public bool IsFilter(Type type)
        {
            return type != null && (IsFilterAssembly(type.Assembly) || IsFilterType(type));
        }
        private bool IsFilterAssembly(Assembly assembly)
        {
            var ignoreAssemblies = knifeOptions.Ignores?.Assemblies ?? new List<string>();
            return assembly.GetName().Name.IsMatchWildcardAnyOne(ignoreAssemblies, StringComparison.InvariantCultureIgnoreCase);
        }
        private bool IsFilterType(Type type)
        {
            var ignoreTypes = knifeOptions.Ignores?.Types ?? new List<string>();
            return type.FullName.IsMatchWildcardAnyOne(ignoreTypes, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
