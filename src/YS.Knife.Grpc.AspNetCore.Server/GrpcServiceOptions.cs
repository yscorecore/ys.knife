using System;
using System.Collections.Generic;
using System.Text;
using Grpc.AspNetCore.Server;

namespace YS.Knife.Grpc.AspNetCore
{
    [Options("Knife.GrpcService")]
    public class GrpcServiceOptions : global::Grpc.AspNetCore.Server.GrpcServiceOptions
    {
        public bool EnableReflection { get; set; } = true;
    }
}
