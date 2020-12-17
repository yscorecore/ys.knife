using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Hosting.Web.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            DateTime start = DateTime.Now;
            try
            {
                await _next(context);
            }
            finally
            {
                _logger.LogInformation(
                    "[{statusCode}] - [{time}] - {method} - {path}",
                    context.Response?.StatusCode,
                    DateTime.Now - start,
                    context.Request?.Method,
                    context.Request?.Path.Value);
            }
        }
    }
}
