using System;
using System.Threading.Tasks;
using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Aop.Logging
{
    public class LoggingAttribute : BaseAopAttribute
    {
        public LoggingAttribute()
        {
            this.Order = 1000;
        }
        [FromServiceContext]
        public ILogger<LoggingAttribute> Logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before ..............");
                await next(context);
                Console.WriteLine("After ..............");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
