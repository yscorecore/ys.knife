using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace YS.Knife.Grpc.TestServer
{
    [GrpcService]
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            HelloReply helloReply = new HelloReply
            {
                Message = "hello"
            };
            return Task.FromResult(helloReply);
        }
    }
}
