using Amazon.Extensions.NETCore.Setup;
using Amazon.KeyManagementService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
