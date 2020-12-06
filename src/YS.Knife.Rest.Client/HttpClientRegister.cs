using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{
    public class HttpClientRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            services.AddHttpClient();
        }
    }
}
