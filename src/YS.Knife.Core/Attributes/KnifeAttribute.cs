using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public abstract class KnifeAttribute : Attribute
    {
        public KnifeAttribute(Type validateFromType)
        {

            this.ValidateFromType = validateFromType;
        }
        public Type ValidateFromType { get; private set; }
        public abstract void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType);
    }



}
