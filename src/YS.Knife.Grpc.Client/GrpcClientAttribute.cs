using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YS.Knife.Grpc.Client;

namespace YS.Knife.Grpc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GrpcClientAttribute : YS.Knife.KnifeAttribute
    {
        public GrpcClientAttribute() : base(null)
        {

        }
        public string GrpcServiceName { get; set; }

        public Type InjectType { get; set; }

        /// <summary>
        /// Get or set a value, it means that when <see cref="InjectType"/> is null, auto deduce the interface type.
        /// </summary>
        public bool AutoDeduceInterfaceType { get; set; } = true;
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            //services.AddGrpcClient<>()
            var method = typeof(GrpcClientAttribute).GetMethod(nameof(RegisterGrpcClient), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services, context?.Configuration });

            var injectType = this.InjectType ?? (AutoDeduceInterfaceType ? DeduceInjectInterfaceType(declareType) : null);
            if (injectType != null && injectType != declareType)
            {
                // every time from service provider get the grpc client
                services.AddTransient(injectType, sp => sp.GetService(declareType));
            }
        }

        private void RegisterGrpcClient<T>(IServiceCollection services, IConfiguration configuration)
            where T : class
        {
            services.AddGrpcClient<T>((sp, configAction) =>
            {
                string serviceName = this.GrpcServiceName ?? typeof(T).FullName;
                var grpcInfo = sp.GetRequiredService<GrpcServicesOptions>().GetServiceInfoByName(serviceName);
                configAction.Address = new Uri(grpcInfo.BaseAddress);
            });
        }

        private Type DeduceInjectInterfaceType(Type serviceType)
        {
            var allInterfaces = serviceType.GetInterfaces();
            if (allInterfaces.Length > 1)
            {
                throw new InvalidOperationException($"Can not deduce the inject type from current type '{serviceType.FullName}', please configure the inject type manually.");
            }
            return allInterfaces.FirstOrDefault();
        }
    }
}
