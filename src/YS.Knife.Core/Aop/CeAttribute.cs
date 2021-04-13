using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace YS.Knife.Aop
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CeAttribute : BaseAopAttribute
    {
        public ushort SubErrorCode { get; }
        public string Message { get; }


        public CeAttribute(ushort errorCode, string message)
        {
            this.SubErrorCode = errorCode;
            this.Message = message;
        }

        private static LocalCache<Type, CodeExceptionsAttribute> baseCodeCache = new LocalCache<Type, CodeExceptionsAttribute>();
        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            this.AssertReturnType(context);
            var errorCode = GetErrorCode(context);
            var message = GetErrorMessageTemplate(context);
            context.ReturnValue = new CodeException(errorCode, message);
            return context.Break();
        }

        private int GetErrorCode(AspectContext context)
        {
            var codeExceptionsAttribute = GetCodeExceptionsAttributeInDeclareType(context);
            return codeExceptionsAttribute.BaseErrorCode + this.SubErrorCode;
        }

        private static CodeExceptionsAttribute GetCodeExceptionsAttributeInDeclareType(AspectContext context)
        {
            var codeExceptionsAttribute = baseCodeCache.Get(context.ServiceMethod.DeclaringType,
                (type) => type.GetCustomAttribute<CodeExceptionsAttribute>());
            return codeExceptionsAttribute;
        }

        private string GetErrorMessageTemplate(AspectContext context)
        {
            var type = typeof(IStringLocalizer<>).MakeGenericType(context.ServiceMethod.DeclaringType);
            var localizer = context.ServiceProvider.GetRequiredService(type) as IStringLocalizer;
            var codeExceptionsAttribute = GetCodeExceptionsAttributeInDeclareType(context);
            var resourceKey = codeExceptionsAttribute.ErrorMessageKeyPrefix + this.SubErrorCode;
            var localizedString = localizer.GetString(resourceKey);

            return this.Message;
        }

        private void AssertReturnType(AspectContext context)
        {
            if (context.ServiceMethod.ReturnType.IsAssignableFrom(typeof(CodeException)))
            {
                throw new InvalidOperationException($"The return type of the method '{context.ServiceMethod.Name}' in interface {context.ServiceMethod.DeclaringType?.FullName} should assignable from '{typeof(CodeException).FullName}'.");
            }
        }
    }
    public class CodeExceptionsAttribute : DynamicProxyAttribute
    {
        public CodeExceptionsAttribute(int baseErrorCode = 100000) : base(ServiceLifetime.Singleton)
        {
            BaseErrorCode = baseErrorCode;
        }

        public string ErrorMessageKeyPrefix { get; set; } = "E";
        public int BaseErrorCode { get; }
    }
}
