using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YS.Knife.Stages;
namespace YS.Knife.Hosting
{
    [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class InjectAttribute : System.Attribute
    {
        public InjectAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            this.Lifetime = lifetime;
        }
        public ServiceLifetime Lifetime { get; set; }
    }
}
