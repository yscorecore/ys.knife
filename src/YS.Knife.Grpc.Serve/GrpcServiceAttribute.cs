using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Grpc.Serve
{
    public class GrpcServiceAttribute : YS.Knife.KnifeAttribute
    {
        public GrpcServiceAttribute() : base(null)
        {

        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {

        }
    }
}
