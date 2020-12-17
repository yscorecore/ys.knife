using System;
using System.Reflection;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false, Inherited = false)]
    public  class FunctionAttribute:Attribute
    {
        public FunctionAttribute(string code)
        {
            this.Code = code??string.Empty;
        }

        public string Code { get;  }
    }

    public static class FunctionExtensions
    {
        public static string GetFunctionCode(this MethodInfo methodInfo)
        {
            _ = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            return methodInfo.GetCustomAttribute<FunctionAttribute>()?.Code ?? $"{methodInfo.DeclaringType?.FullName}.{methodInfo.Name}";
        }
    }
}
