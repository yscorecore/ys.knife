using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Aop
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CeAttribute : BaseAopAttribute
    {
        public ushort SubCode { get; }
        public string Message { get; }


        public CeAttribute(ushort code, string message)
        {
            this.SubCode = code;
            this.Message = message;
        }

        private static LocalCache<Type, int> baseCodeCache = new LocalCache<Type, int>();
        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            this.AssertReturnType(context);
            var baseErrorCode = GetBaseErrorCode(context);
            var errorCode = baseErrorCode + this.SubCode;
            var exception = new CodeException(errorCode, this.Message);
            context.ReturnValue = exception;
            return context.Break();
        }

        private int GetBaseErrorCode(AspectContext context)
        {
            return baseCodeCache.Get(context.ServiceMethod.DeclaringType,
                 (type) => type.GetCustomAttribute<CodeExceptionsAttribute>().BaseErrorCode);
        }

        private void AssertReturnType(AspectContext context)
        {
        }
    }
    public class CodeExceptionsAttribute : DynamicProxyAttribute
    {
        public CodeExceptionsAttribute(int baseErrorCode = 100000) : base(ServiceLifetime.Singleton)
        {
            BaseErrorCode = baseErrorCode;
        }

        public int BaseErrorCode { get; }
    }
}
