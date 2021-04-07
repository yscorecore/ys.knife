using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Aop
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CodeErrorAttribute : BaseAopAttribute
    {
        public short SubCode { get; }
        public string Message { get; }

        public CodeErrorAttribute(short code, string message)
        {
            this.SubCode = code;
            this.Message = message;
        }

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            if (context.ProxyMethod.ReturnType == typeof(Exception))
            {
                var exception = new Exception(this.Message);
                context.ReturnValue = exception;
                return context.Break();
            }
            else
            {
                return next.Invoke(context);
            }

        }
    }
    public class CodeErrorProviderAttribute : DynamicProxyAttribute
    {
        public CodeErrorProviderAttribute(ushort baseErrorCode = 0) : base(ServiceLifetime.Singleton)
        {
            BaseErrorCode = baseErrorCode;
        }

        public ushort BaseErrorCode { get; }
    }
}
