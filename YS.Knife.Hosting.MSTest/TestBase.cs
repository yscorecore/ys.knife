using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.DependencyInjection;

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
        public void UseAspNetCoreEnv(string envName) 
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", envName);
        }
        public void ReleaseAspNetCoreEnv(string envName)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", string.Empty);
        }
    }

    public class TestBase<T> : TestBase
    {
        public TestBase()
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
