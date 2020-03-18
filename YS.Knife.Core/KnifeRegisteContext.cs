using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public class KnifeRegisteContext
    {
        public IServiceCollection Services { get; set; }
        public IConfiguration Configuration { get; set; }
        public Type DeclaredType { get; set; }
    }
}