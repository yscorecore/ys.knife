using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.EntityFrameworkCore
{
    public abstract class DynamicEFContextAttribute : Knife.KnifeAttribute
    {
        public DynamicEFContextAttribute() : base(typeof(DbContext))
        {

        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            services.AddScoped(declareType, this.OnCreateNewContext);
        }

        protected abstract DbContext OnCreateNewContext(IServiceProvider serviceProvider);
       
    }
}
