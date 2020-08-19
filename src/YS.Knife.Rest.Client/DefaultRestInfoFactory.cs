using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{
    [YS.Knife.ServiceClass(Lifetime = ServiceLifetime.Singleton)]
    public class DefaultRestInfoFactory : IRestInfoFactory
    {
        readonly ApiServicesOptions apiServicesOptions;

        public DefaultRestInfoFactory(ApiServicesOptions apiServicesOptions)
        {
            this.apiServicesOptions = apiServicesOptions;
        }

        public RestInfo GetRestInfo(string serviceName)
        {
            var dic = apiServicesOptions.Services ?? new Dictionary<string, ServiceOptions>();
            if (dic.TryGetValue(serviceName, out var so))
            {
                return new RestInfo
                {
                    BaseAddress = so.BaseAddress,
                    Timeout = TimeSpan.FromSeconds(so.Timeout > 0 ? so.Timeout : apiServicesOptions.Timeout),
                    MaxResponseContentBufferSize = so.MaxResponseContentBufferSize > 0 ? so.MaxResponseContentBufferSize : apiServicesOptions.MaxResponseContentBufferSize
                };
            }
            return new RestInfo
            {
                BaseAddress = null,
                Timeout = TimeSpan.FromSeconds(apiServicesOptions.Timeout),
                MaxResponseContentBufferSize = apiServicesOptions.MaxResponseContentBufferSize
            };
        }
    }
}
