using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace YS.Knife
{

    public class HostedClassAttribute : KnifeAttribute
    {
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            this.ValidateType(declareType, typeof(IHostedService));
            var method = this.GetType().GetMethod(nameof(AddHostedService), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services });
        }
        private void AddHostedService<T>(IServiceCollection services)
            where T : class, IHostedService
        {
            services.AddHostedService<T>();
        }
    }
}
