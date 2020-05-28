using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
namespace YS.Knife.Aws.S3
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            services.TryAddAWSService<IAmazonS3>();
        }
    }
}