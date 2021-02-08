using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using YS.Knife;
using YS.Knife.Rest.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderGrpcExtensions
    {
        public static void MapAllHubServices(this IEndpointRouteBuilder endpoints)
        {
            if (endpoints is null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            var hubTypes = AppDomain.CurrentDomain.FindInstanceTypesByAttribute<HubAttribute>();
            var genMethod = typeof(EndpointRouteBuilderGrpcExtensions)
                    .GetMethod(nameof(MapHub), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var hubType in hubTypes)
            {
                string hubPath = hubType.GetCustomAttribute<HubAttribute>().HubPath;
                var method = genMethod.MakeGenericMethod(hubType);
                method.Invoke(null, new object[] { endpoints, hubPath });
            }
        }

        private static void MapHub<T>(IEndpointRouteBuilder endpoints, string path) where T : Hub
        {
            endpoints.MapHub<T>(path);
        }
    }
}
