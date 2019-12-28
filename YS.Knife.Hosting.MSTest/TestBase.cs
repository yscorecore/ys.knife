using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Knife.Hosting.MSTest
{
    public abstract class TestBase
    {
        [TestInitialize]
        public void Setup()
        {
            this.OnSetup();
        }
        protected IHost host;
        protected virtual void OnSetup()
        {
            this.host = Host.CreateHost();
        }

        protected virtual void OnTearDown()
        {
            if (host != null)
            {
                this.host.Dispose();
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            this.OnTearDown();
        }
        public T Get<T>()
        {
            return this.host.Services.GetService<T>();
        }
    }

    public class TestBase<T> : TestBase
    {
        public TestBase()
        {
            this.instanceFactory = new Lazy<T>(this.Get<T>, true);
        }
        private Lazy<T> instanceFactory;
        protected T Instance 
        {
            get
            {
                return instanceFactory.Value;
            }
        }

    }

}
