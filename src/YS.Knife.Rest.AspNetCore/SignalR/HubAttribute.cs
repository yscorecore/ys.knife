using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HubAttribute : KnifeAttribute
    {
        public string HubPath { get; set; }
        public HubAttribute(string hubPath) : base(typeof(Microsoft.AspNetCore.SignalR.Hub))
        {
            this.HubPath = hubPath;
        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {

        }
    }
}
