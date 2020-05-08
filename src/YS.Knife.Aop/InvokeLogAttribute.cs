using System;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Aop
{
    public class InvokeLogAttribute : BaseAopAttribute
    {
        public InvokeLogAttribute()
        {
            this.Order = 1000;
        }
        [FromServiceContext]
        public ILogger<InvokeLogAttribute> Logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            string methodFullName = FormatMethodName(context.ImplementationMethod);
            try
            {
                LogStart(methodFullName);
                await next(context);
                LogEnd(methodFullName);
            }
            catch (Exception ex)
            {
                LogException(methodFullName, ex);
                throw;
            }
        }

        protected virtual string FormatMethodName(MethodInfo methodInfo)
        {
            _ = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            return $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name}";
        }
        protected virtual void LogStart(string methodFullName)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("Start invoke method \"{Method}\".", methodFullName);
            }
        }
        protected virtual void LogEnd(string methodFullName)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("End invoke method \"{Method}\".", methodFullName);
            }
        }
        protected virtual void LogException(string methodFullName, Exception ex)
        {
            if (Logger.IsEnabled(LogLevel.Error))
            {
                Logger.LogError(ex, "Error occurred when invoke method \"{Method}\".", methodFullName);
            }
        }

    }
}
