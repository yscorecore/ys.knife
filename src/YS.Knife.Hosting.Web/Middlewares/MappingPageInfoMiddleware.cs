using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace YS.Knife.Hosting.Web.Middlewares
{
    public class MappingPageInfoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly MappingPageOptions _mappingPageOptions;

        public MappingPageInfoMiddleware(RequestDelegate next, ILogger<MappingPageInfoMiddleware> logger, MappingPageOptions mappingPageOptions)
        {
            _next = next;
            _logger = logger;
            _mappingPageOptions = mappingPageOptions;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Get)
            {
                var pageSize = GetQueryStringValue(context.Request.Query[_mappingPageOptions.PageSizeName]);
                if (pageSize.HasValue)
                {
                    var pageIndex = GetQueryStringValue(context.Request.Query[_mappingPageOptions.PageIndexName], 1);
                    var qb1 = new QueryBuilder(context.Request.Query)
                        {
                            {_mappingPageOptions.LimitName, pageSize.Value.ToString()},
                            {_mappingPageOptions.OffsetName, ((pageIndex - 1) * pageSize).ToString()}
                        };
                    context.Request.QueryString = qb1.ToQueryString();
                }
            }
            if (_next != null)
            {
                await _next(context);
            }

        }

        private static int? GetQueryStringValue(StringValues queryString)
        {
            if (queryString.Count > 0 && int.TryParse(queryString, out var indexValue))
            {
                return indexValue;
            }

            return null;
        }

        private static int GetQueryStringValue(StringValues queryString, int defaultValue)
        {
            if (queryString.Count > 0 && int.TryParse(queryString, out var indexValue))
            {
                return indexValue;
            }
            return defaultValue;
        }
    }

    [Options]
    public class MappingPageOptions
    {

        public string PageSizeName { get; set; } = "pageSize";
        public string PageIndexName { get; set; } = "pageIndex";

        public string LimitName { get; set; } = "limit";
        public string OffsetName { get; set; } = "offset";
    }
}
