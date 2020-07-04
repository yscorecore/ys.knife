using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Api
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiBase : ControllerBase
    {

    }
    public abstract class ApiBase<T> : ApiBase
        where T : class
    {
        public ApiBase()
        {
            this.delegaterFactory = new Lazy<T>(() => this.HttpContext.RequestServices.GetRequiredService<T>());
        }
        private Lazy<T> delegaterFactory;
        protected T Delegater { get { return delegaterFactory.Value; } }
    }
}
