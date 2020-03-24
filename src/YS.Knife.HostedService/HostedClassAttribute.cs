using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]

    public sealed class HostedClassAttribute : KnifeAttribute
    {
        public HostedClassAttribute() : base(typeof(IHostedService))
        {

        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            var method = typeof(HostedClassAttribute).GetMethod(nameof(AddHostedService), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services });
        }
        private void AddHostedService<T>(IServiceCollection services)
            where T : class, IHostedService
        {
            services.AddHostedService<T>();
        }
    }
}
