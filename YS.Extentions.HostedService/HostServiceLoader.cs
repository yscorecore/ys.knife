using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace YS.Extentions.HostedService
{
    public class HostServiceLoader : IServiceLoader
    {
        public void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            foreach (var hostServiceType in AppDomain.CurrentDomain.FindInstanceTypesByAttributeAndBaseType<HostServiceClassAttribute, IHostedService>())
            {
                var attribute = hostServiceType.GetCustomAttributes(typeof(HostServiceClassAttribute), false)[0] as HostServiceClassAttribute;
                var instance = Activator.CreateInstance(typeof(HostServiceProxy<>).MakeGenericType(hostServiceType)) as IHostServiceProxy;
                instance.AddHostedService(services, configuration);
            }
        }

        #region HostService
        private interface IHostServiceProxy
        {
            void AddHostedService(IServiceCollection services, IConfiguration configuration);
        }
        private class HostServiceProxy<T> : IHostServiceProxy
            where T : class, IHostedService
        {
            public void AddHostedService(IServiceCollection services, IConfiguration configuration)
            {
                services.AddHostedService<T>();
            }
        }
        #endregion
    }
}
