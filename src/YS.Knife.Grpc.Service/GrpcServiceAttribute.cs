using System;

namespace YS.Knife.Grpc
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GrpcServiceAttribute : Attribute
    {
        //public GrpcServiceAttribute() : base(null)
        //{

        //}
        //public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        //{
        //    // do nothing, just need map grpc service to endpoints
        //}
    }
}
