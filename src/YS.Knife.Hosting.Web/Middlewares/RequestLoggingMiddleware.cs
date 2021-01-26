using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
                bool isGrpc = IsGrpcRequest(context);
                var log = GetLogByStatusCode(context.Response.StatusCode);
                log(_logger,
                    context.Request.Protocol,
                    context.Request?.Method,
                    context.Request.GetDisplayUrl(),
                    (DateTime.Now - start).TotalSeconds,
                    context.Response.StatusCode, null);

            }
        }

        private static bool IsGrpcRequest(HttpContext context)
        {
            
            return context.GetEndpoint()?.DisplayName?.StartsWith("gRPC") ?? false;

        }

        private Action<ILogger, string, string, string, double, int, Exception> GetLogByStatusCode(int statusCode)
        {
            if (statusCode >= 400)
            {
                return Logs.ErrorLog;
            }
            else if (statusCode >= 300)
            {
                return Logs.WarningLog;
            }
            else
            {
                return Logs.InfoLog;
            }
        }

        private class Logs
        {
            public static Action<ILogger, string, string, string, double, int, Exception> InfoLog = LoggerMessage.Define<string, string, string, double, int>(
                LogLevel.Information,
                new EventId(1, nameof(InfoLog)),
                "[-] {protocol} {method} {url} took {time:f4} seconds, response code {statusCode}.");
            public static Action<ILogger, string, string, string, double, int, Exception> WarningLog = LoggerMessage.Define<string, string, string, double, int>(
               LogLevel.Warning,
               new EventId(2, nameof(WarningLog)),
               "[-] {protocol} {method} {url} took {time:f4} seconds, response code {statusCode}.");
            public static Action<ILogger, string, string, string, double, int, Exception> ErrorLog = LoggerMessage.Define<string, string, string, double, int>(
             LogLevel.Error,
             new EventId(3, nameof(WarningLog)),
             "[-] {protocol} {method} {url} took {time:f4} seconds, response code {statusCode}.");
        }
    }
}
