using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife;
using YS.Knife.Grpc;
using YS.Knife.Grpc.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderGrpcExtensions
    {
        public static void MapAllGrpcKnifeServices(this IEndpointRouteBuilder endpoints)
        {
            var grpcTypes = AppDomain.CurrentDomain.FindInstanceTypesByAttribute<GrpcServiceAttribute>();
            var genMethod = typeof(EndpointRouteBuilderGrpcExtensions)
                    .GetMethod(nameof(MapGrpcService), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var grpcType in grpcTypes)
            {
                var method = genMethod.MakeGenericMethod(grpcType);
                method.Invoke(null, new object[] { endpoints });
            }
            var options = endpoints.ServiceProvider.GetService<GrpcServiceOptions>();
            if (options.EnableReflection)
            {
                endpoints.MapGrpcReflectionService();
            }
        }
        private static void MapGrpcService<T>(IEndpointRouteBuilder endpoints) where T : class
        {
            endpoints.MapGrpcService<T>();
        }

    }
}
