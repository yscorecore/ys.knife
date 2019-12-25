using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace System
{
    public interface IServiceLoader
    {
        void LoadServices(IServiceCollection services,IConfiguration configuration);
    }
}
