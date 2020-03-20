using Microsoft.Extensions.DependencyInjection;
using System;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public abstract class KnifeAttribute : Attribute
    {
        public abstract void RegisteService(IServiceCollection serviceCollections, IRegisteContext context,Type declareType);
    }


}