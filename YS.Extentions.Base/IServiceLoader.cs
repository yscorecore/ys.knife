using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace System
{
    public interface IServiceLoader
    {
        IServiceCollection LoadServices(IServiceCollection services,IConfiguration configuration);
    }
}
