using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace YS.Knife.Aop
{
    public class CodeExceptionsAttribute : DynamicProxyAttribute
    {
        public CodeExceptionsAttribute(string baseErrorCode = "100000") : base(ServiceLifetime.Singleton)
        {
            BaseErrorCode = baseErrorCode;
        }

        public string ErrorMessageKeyPrefix { get; set; } = "CE";
        public string BaseErrorCode { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CeAttribute : BaseAopAttribute
    {
        public string SubErrorCode { get; }
        public string MessageTemplate { get; }


        public CeAttribute(string errorCode, string messageTemplate)
        {
            this.SubErrorCode = errorCode;
            this.MessageTemplate = messageTemplate;
        }

        private static LocalCache<Type, CodeExceptionsAttribute> baseCodeCache =
            new LocalCache<Type, CodeExceptionsAttribute>();

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            this.AssertReturnType(context);
            var errorCode = GetErrorCode(context);
            var message = GetErrorMessage(context);
            var data = GetExceptionData(context);
            var innerException = GetInnerException(context);
            context.ReturnValue = BuildException(errorCode, message, data, innerException);
            return context.Break();
        }

        private Exception GetInnerException(AspectContext context)
        {
            return context.Parameters.OfType<Exception>().FirstOrDefault();
        }

        private Exception BuildException(string code, string message, IDictionary<string, object> data,
            Exception innerException)
        {
            var exception = new CodeException(code, message, innerException);
            // data ignore inner exception
            foreach (var (key, value) in data)
            {
                if (key != null && value != innerException)
                {
                    exception.Data[key] = value;
                }
            }
            return exception;
        }

        private string GetErrorCode(AspectContext context)
        {
            var codeExceptionsAttribute = GetCodeExceptionsAttributeInDeclareType(context);
            return codeExceptionsAttribute.BaseErrorCode + this.SubErrorCode;
        }

        private IDictionary<string, object> GetExceptionData(AspectContext context)
        {
            return context.ServiceMethod.GetParameters()
                .Zip(context.Parameters, (pInfo, val) => new KeyValuePair<string, object>(pInfo.Name, val))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private static CodeExceptionsAttribute GetCodeExceptionsAttributeInDeclareType(AspectContext context)
        {
            var codeExceptionsAttribute = baseCodeCache.Get(context.ServiceMethod.DeclaringType,
                (type) => type.GetCustomAttribute<CodeExceptionsAttribute>());
            return codeExceptionsAttribute;
        }

        private string GetErrorMessage(AspectContext context)
        {
            var template = GetErrorMessageTemplate(context);
            var formatter = ValuesFormatter.FromText(template ?? string.Empty);
            var kwArgs = context.ServiceMethod.GetParameters()
                .Zip(context.Parameters, (pInfo, val) => new KeyValuePair<string, object>(pInfo.Name, val))
                .ToList();
            return formatter.Format(kwArgs);
        }

        private string GetErrorMessageTemplate(AspectContext context)
        {
            var type = typeof(IStringLocalizer<>).MakeGenericType(context.ServiceMethod.DeclaringType);
            var localizer = context.ServiceProvider.GetRequiredService(type) as IStringLocalizer;
            var codeExceptionsAttribute = GetCodeExceptionsAttributeInDeclareType(context);
            var resourceKey = codeExceptionsAttribute.ErrorMessageKeyPrefix + this.SubErrorCode;
            var localizedString = localizer.GetString(resourceKey);
            return localizedString.ResourceNotFound ? this.MessageTemplate : localizedString.Value;
        }

        private void AssertReturnType(AspectContext context)
        {
            if (!context.ServiceMethod.ReturnType.IsAssignableFrom(typeof(CodeException)))
            {
                throw new InvalidOperationException(
                    $"The return type of the method '{context.ServiceMethod.Name}' in interface '{context.ServiceMethod.DeclaringType?.FullName}' should assignable from '{typeof(CodeException).FullName}'.");
            }
        }
    }
}
