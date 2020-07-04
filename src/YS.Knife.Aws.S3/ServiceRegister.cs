using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
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
