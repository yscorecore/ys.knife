using System;

namespace YS.Knife.Grpc
{
    public class GrpcClientAttribute : YS.Knife.KnifeAttribute
    {
        public GrpcClientAttribute() : base(null)
        {

        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {

        }
    }
}
