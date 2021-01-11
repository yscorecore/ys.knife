using System.Collections.Generic;

namespace YS.Knife.Grpc.Client
{
    [Options]
    public class GrpcServicesOptions
    {
        public Dictionary<string, GrpcService> Services { get; set; } = new Dictionary<string, GrpcService>();
    }
    
    public class GrpcService
    {
        public string BaseAddress { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
