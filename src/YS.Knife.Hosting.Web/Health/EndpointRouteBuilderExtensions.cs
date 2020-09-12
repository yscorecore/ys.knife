using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
namespace YS.Knife.Hosting.Web.Health
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapKnifeHealthCheckWhenProvided(this IEndpointRouteBuilder endpoints)
        {
            var options = endpoints?.ServiceProvider.GetService<IOptions<HealthCheckServiceOptions>>().Value;
            if (options.Registrations != null && options.Registrations.Count > 0)
            {
                var healthOptions = endpoints.ServiceProvider.GetService<HealthOptions>();
                endpoints?.MapHealthChecks(healthOptions.RequestPath, new HealthCheckOptions
                {
                    ResponseWriter = WriteResponse,
                    AllowCachingResponses = healthOptions.CachingResponse,
                    ResultStatusCodes = new Dictionary<HealthStatus, int>
                    {
                        [HealthStatus.Healthy] = healthOptions.HealthyCode,
                        [HealthStatus.Unhealthy] = healthOptions.UnhealthyCode,
                        [HealthStatus.Degraded] = healthOptions.DegradedCode
                    }
                });
            }
        }
        private static async Task WriteResponse(HttpContext context, HealthReport result)
        {
            var content = new
            {
                Status = result.Status.ToString(),
                TotalDuration = Convert.ToInt32(result.TotalDuration.TotalMilliseconds),
                Entries = result.Entries.ToDictionary(p => p.Key,
                p => new
                {
                    p.Value.Tags,
                    Status = p.Value.Status.ToString(),
                    Duration = Convert.ToInt32(p.Value.Duration.TotalMilliseconds),
                    p.Value.Description,
                    p.Value.Exception,
                    p.Value.Data,
                }),
            };

            var json = JsonSerializer.Serialize(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }

    }
}
