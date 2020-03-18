using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public class KnifeServiceContext
    {
        public IServiceCollection Services { get; set; }
        public IConfiguration Configuration { get; set; }
        public Type InstanceType { get; set; }
    }
}