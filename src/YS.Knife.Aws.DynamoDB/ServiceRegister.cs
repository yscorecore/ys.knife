//using Amazon.KeyManagementService;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.Aws.DynamoDB
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            services.TryAddAWSService<IAmazonDynamoDB>();
        }
    }
}
