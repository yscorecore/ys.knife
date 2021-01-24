using System;
using System.Collections.Generic;
using System.Text;
using Grpc.AspNetCore.Server;

namespace YS.Knife.Grpc.AspNetCore.Server
{
    [Knife.Options]
    public class GrpcOptions : GrpcServiceOptions
    {
        public bool EnableReflection { get; set; } = true;
    }
}
