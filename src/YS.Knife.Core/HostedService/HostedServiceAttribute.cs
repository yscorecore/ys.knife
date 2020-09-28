using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]

    public sealed class HostedServiceAttribute : KnifeAttribute
    {
        public HostedServiceAttribute() : base(typeof(IHostedService))
        {

        }
        public override void RegisterService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            var method = typeof(HostedServiceAttribute).GetMethod(nameof(AddHostedService), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services });
        }
        private void AddHostedService<T>(IServiceCollection services)
            where T : class, IHostedService
        {
            services.AddHostedService<T>();
        }
    }
}
