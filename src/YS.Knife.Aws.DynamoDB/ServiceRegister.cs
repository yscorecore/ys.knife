using Microsoft.Extensions.DependencyInjection;
//using Amazon.KeyManagementService;
using Amazon.DynamoDBv2;
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
