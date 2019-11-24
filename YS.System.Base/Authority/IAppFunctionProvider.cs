using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Authority
{
    public interface IAppFunctionProvider
    {
        IEnumerable<FunctionInfo> GetFunctions(FunctionLevel deepLevel);
        IEnumerable<FunctionInfo> GetChildrenFunctions(string functionCode);
        FunctionInfo GetFunction(string functionCode);
        string ApplicationId { get; }
        string GetFunctionCode(Type type);
        string GetOperationCode(Type type, MethodInfo method);
      
    }


    public abstract class BaseAppFunctionProvider : IAppFunctionProvider
    {
        public virtual string GetOperationCode(Type type, MethodInfo method)
        {
            var function = method.GetCustomAttribute<OperationAttribute>();
            if (function == null) return null;
            function.InitFunctionCode(type, method);
            return function.Code;
        }
        public virtual string GetFunctionCode(Type type)
        {
            var function = type.GetCustomAttribute<FunctionAttribute>();
            if (function == null) return null;
            function.InitFunctionCode(type);
            return function.Code;
        }
        public virtual string ApplicationId
        {
            get
            {
                return ApplicationInfo.Guid;
            }
        }
        public abstract IEnumerable<FunctionInfo> GetChildrenFunctions(string functionCode);
        public abstract IEnumerable<FunctionInfo> GetFunctions(FunctionLevel deepLevel);
        public abstract FunctionInfo GetFunction(string functionCode);

    }
    public class LocalAssemblyAppFunctionProvider : BaseAppFunctionProvider
    {
        public LocalAssemblyAppFunctionProvider()
        {
            this.Functions = LoadAssemblies().ToList();
        }
        protected List<FunctionInfo> Functions { get; private set; }
        protected virtual IEnumerable<FunctionInfo> LoadAssemblies()
        {
            AssemblysFunctionFinder finder = new Authority.AssemblysFunctionFinder();
            var files = System.IO.DirectoryEx.GetFiles(EnvironmentEx.ApplicationDirectory, "*.dll|*.exe",string.Empty);
            finder.AssemblyFilePaths.AddRange(files);
            var allFunctions = finder.FindFunctions();
            return this.Filter(allFunctions);
        }
        protected virtual IEnumerable<FunctionInfo> Filter(IEnumerable<FunctionInfo> functions)
        {
            var trees = functions.BuildTree((a) => { return a.Id; }, a => { return a.ParentId; });
            var appFunctions = trees.Where(p => p.Value.Id == this.ApplicationId).SelectMany(p => p.FlattenBefore()).Select(p => p.Value);
            return appFunctions;
        }
        public override IEnumerable<FunctionInfo> GetChildrenFunctions(string functionCode)
        {
            var current = this.Functions.Where(p => p.Code == functionCode).SingleOrDefault();
            if (current == null) return new List<FunctionInfo>();
            return (from p in this.Functions
                    where p.ParentId == current.Id
                    select p).ToList();
        }
        public override IEnumerable<FunctionInfo> GetFunctions(FunctionLevel deepLevel)
        {
            List<FunctionLevel> levels = new List<FunctionLevel>();

            foreach (FunctionLevel level in Enum.GetValues(typeof(FunctionLevel)))
            {
                if ((int)level <= (int)deepLevel)
                {
                    levels.Add(level);
                }
            }
            return this.Functions.Where(p => levels.Contains(p.Level)).ToList();
        }
        public override FunctionInfo GetFunction(string functionCode)
        {
            return this.Functions.Where(p => p.Code == functionCode).SingleOrDefault();
        }
    }
    public static class AppFunctionProviderExtentions
    {
        public static bool HasPermission(this IAppFunctionProvider provider, string functionCode)
        {
            return provider.GetFunction(functionCode) != null;
        }
        public static bool HasPermission(this IAppFunctionProvider provider, Type type)
        {
            var code = provider.GetFunctionCode(type);
            return HasPermission(provider, code);
        }
        public static bool HasPermission(this IAppFunctionProvider provider, Type type, MethodInfo method)
        {
            var code = provider.GetOperationCode(type, method);
            return HasPermission(provider, code);
        }
        public static bool ContainsPermission(this  IEnumerable<FunctionInfo> functions, string functionCode)
        {
            if (functions == null) return false;
            return functions.Any(p => p.Code == functionCode);
        }

    }
}
