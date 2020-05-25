using Microsoft.Extensions.DependencyInjection;
using Amazon.KeyManagementService;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;

namespace YS.Knife.Aws.Kms
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            services.TryAddAWSService<IAmazonKeyManagementService>();
        }
    }
}
