using System;
using System.Collections.Generic;
using System.Text;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System.Linq;
namespace YS.Knife.Elasticsearch
{
    class ServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            services.AddSingleton<ElasticClient>((sp) =>
            {
                var options = sp.GetRequiredService<ElasticOptions>();
                var allUrls= (options.Urls ?? Enumerable.Empty<string>()).Select(p => new Uri(p));
                var pool = new StaticConnectionPool(allUrls);
                var settings = new ConnectionSettings(pool)
                .DefaultIndex(options.DefaultIndex);
                return new ElasticClient(settings);

            });
            services.AddSingleton<IElasticClient>(sp => sp.GetRequiredService<ElasticClient>());
        }
    }
}
