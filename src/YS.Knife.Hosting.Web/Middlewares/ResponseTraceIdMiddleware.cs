using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace YS.Knife.Hosting.Web.Middlewares
{
    public class ResponseTraceIdMiddleware
    {
        private readonly RequestDelegate next;

        public ResponseTraceIdMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add("TraceId", new[] { GetTraceId(Activity.Current)??context.TraceIdentifier });
                return Task.CompletedTask;
            });
            await next(context);
        }
        // from  https://github.com/dotnet/aspnetcore/pull/11211/files#diff-43cb511301470939b501cf4fd0a2a662d13b586aa3bef3e4ce83709d63cbb6b9R23
        public static string GetTraceId(Activity activity)
        {
            return activity?.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.RootId,
                ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
                _ => null,
            };
        }
    }
}
