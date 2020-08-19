using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{

    public class RestClientClassAttribute : YS.Knife.KnifeAttribute
    {
        public RestClientClassAttribute() : base(typeof(RestClient))
        {

        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            var method = typeof(RestClientClassAttribute).GetMethod(nameof(RegisteHttpClient), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services });
        }
        private void RegisteHttpClient<T>(IServiceCollection services)
           where T : class, new()
        {
            services.AddHttpClient<T>();
        }
    }
}
