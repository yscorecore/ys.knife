using System;
using YS.Knife.Hosting.Web.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseTraceId(this IApplicationBuilder app)
        {
            _ = app ?? throw new ArgumentNullException(nameof(app));
            return app.UseMiddleware<ResponseTraceIdMiddleware>();
        }
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            _ = app ?? throw new ArgumentNullException(nameof(app));
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
