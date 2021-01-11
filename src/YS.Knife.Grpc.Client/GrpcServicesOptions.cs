using System.Collections.Generic;

namespace YS.Knife.Grpc.Client
{
    [Options]
    public class GrpcServicesOptions
    {
        public string BaseAddress { get; set; }
        public Dictionary<string, GrpcServiceInfo> Services { get; set; } = new Dictionary<string, GrpcServiceInfo>();
    }

    public class GrpcServiceInfo
    {
        public string BaseAddress { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }

    public static class GrpcServicesOptionsExtensions
    {
        public static GrpcServiceInfo GetServiceInfoByName(this GrpcServicesOptions options, string serviceName)
        {
            if (options.Services != null && options.Services.TryGetValue(serviceName, out var grpcServiceInfo))
            {
                if (string.IsNullOrEmpty(grpcServiceInfo?.BaseAddress))
                {
                    return new GrpcServiceInfo() { BaseAddress = options.BaseAddress, Headers = grpcServiceInfo?.Headers };
                }

                return grpcServiceInfo;
            }

            return new GrpcServiceInfo
            {
                BaseAddress = options.BaseAddress
            };
        }
    }
}
