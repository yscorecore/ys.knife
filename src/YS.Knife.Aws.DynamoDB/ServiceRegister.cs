//using Amazon.KeyManagementService;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.Aws.DynamoDB
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            services.TryAddAWSService<IAmazonDynamoDB>();
        }
    }
}
