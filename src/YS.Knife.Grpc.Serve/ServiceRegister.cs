using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Grpc.AspNetCore.Server
{
    class ServiceRegister : YS.Knife.IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            var options = context.Configuration.GetConfigOrNew<GrpcOptions>();
            services.AddGrpc(op =>
            {
                CopyProperties(options, op);

            });
            if (options.EnableReflection)
            {
                services.AddGrpcReflection();
            }
        }
        private void CopyProperties<F, T>(F from, T to)
        {
            var fromProperties = typeof(F).GetProperties();
            var toProperties = typeof(T).GetProperties();
            foreach (var p in fromProperties)
            {
                var toProperty = toProperties.Where(t => t.PropertyType == p.PropertyType && t.CanWrite).FirstOrDefault();
                if (p.CanRead && toProperty != null)
                {
                    toProperty.SetValue(to, p.GetValue(from));
                }
            }
        }
    }
}
