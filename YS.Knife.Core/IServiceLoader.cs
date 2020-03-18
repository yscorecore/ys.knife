using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife
{
    public interface IServiceLoader
    {
        void LoadServices(IServiceCollection services, IConfiguration configuration);
    }
}
