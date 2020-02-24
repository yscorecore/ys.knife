using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Knife.Hosting.MSTest
{
    public class TestBase<T> : KnifeHost
    {
        public TestBase(Action<IServiceCollection, IConfiguration> configureDelegate = null) : base(new string[0], configureDelegate)
        {
            this.testObjectFactory = new Lazy<T>(this.Get<T>, true);
        }

        private Lazy<T> testObjectFactory;
        protected T TestObject
        {
            get
            {
                return testObjectFactory.Value;
            }
        }
    }

}
