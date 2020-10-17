using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace YS.Knife.Hosting.Web.CorrelationId
{
    // from the blog https://www.stevejgordon.co.uk/asp-net-core-correlation-ids
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorrelationIdOptions _options;
        private readonly ILogger _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger, IOptions<CorrelationIdOptions> options)
        {

            _ = options ?? throw new ArgumentNullException(nameof(options));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            if (context.Request.Headers.TryGetValue(_options.Header, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }

            if (_options.IncludeInResponse)
            {
                // apply the correlation ID to the response header for client side tracking
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(_options.Header, new[] { context.TraceIdentifier });
                    return Task.CompletedTask;
                });
            }

            if (_options.AddToLoggingScope && !string.IsNullOrEmpty(_options.LoggingScopeKey) && !string.IsNullOrEmpty(correlationId))
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    [_options.LoggingScopeKey] = correlationId
                }))
                {
                    //Log.CorrelationIdProcessingEnd(_logger, correlationId);
                    await _next(context);
                }
            }
            else
            {
                //Log.CorrelationIdProcessingEnd(_logger, correlationId);
                await _next(context);
            }
        }
    }
}
