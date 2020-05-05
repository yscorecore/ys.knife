using System;
using System.Threading.Tasks;
using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Aop.Logging
{
    public class LoggingAttribute : AbstractInterceptorAttribute
    {
        [FromServiceContext]
        public ILogger<LoggingAttribute> Logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Logger.LogInformation("Before ..............");
                await next(context);
                Logger.LogInformation("After ..............");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}