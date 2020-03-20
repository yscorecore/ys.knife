using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace YS.Knife.Api
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
            this.delegaterFactory = new Lazy<T>(this.HttpContext.RequestServices.GetRequiredService<T>);
        }
        private Lazy<T> delegaterFactory;
        protected T Delegater { get { return delegaterFactory.Value; } }
    }
}
