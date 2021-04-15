using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Testing.Logging;

namespace YS.Knife.Aop.UnitTest
{
    [TestClass]
    public class InvokeLogAttributeTest
    {

        private IServiceProvider provider;
        [TestInitialize]
        public void Setup()
        {
            provider = Utility.BuildProvider((sc, config) =>
            {
                sc.RemoveAll(typeof(ILoggerFactory));
                sc.RemoveAll(typeof(ILogger<>));
                sc.AddLogging(builder =>
                {
                    builder.ClearProviders().AddProvider(new TestLoggerProvider(loggerStore));
                });
            });
        }
        private readonly TestLoggerStore loggerStore = new TestLoggerStore();

        [TestMethod]
        public void ShouldHasStartLoggerWhenInvokeMethod()
        {
            var service = provider.GetService<ILoggingTestService>();
            service.Hello();
            var loggerEntry = loggerStore.First();
            Assert.AreEqual(LogLevel.Information, loggerEntry.LogLevel);
            Assert.IsNull(loggerEntry.Exception);
            Assert.AreEqual(typeof(InvokeLogAttribute).FullName, loggerEntry.CategoryName);
            Assert.AreEqual("Start invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Hello\".", loggerEntry.Message);
        }

        [TestMethod]
        public void ShouldHasEndLoggerWhenInvokeMethod()
        {
            var service = provider.GetService<ILoggingTestService>();
            service.Hello();
            var loggerEntry = loggerStore.Last();
            Assert.AreEqual(LogLevel.Information, loggerEntry.LogLevel);
            Assert.IsNull(loggerEntry.Exception);
            Assert.AreEqual(typeof(InvokeLogAttribute).FullName, loggerEntry.CategoryName);
            Assert.AreEqual("End invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Hello\".", loggerEntry.Message);
        }

        [TestMethod]
        public void ShouldHasStartLoggerWhenThrowException()
        {
            var service = provider.GetService<ILoggingTestService>();
            Assert.ThrowsException<NotImplementedException>(() => { service.Wrong(); });
            var loggerEntry = loggerStore.First();
            Assert.AreEqual(LogLevel.Information, loggerEntry.LogLevel);
            Assert.IsNull(loggerEntry.Exception);
            Assert.AreEqual(typeof(InvokeLogAttribute).FullName, loggerEntry.CategoryName);
            Assert.AreEqual("Start invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Wrong\".", loggerEntry.Message);
        }

        [TestMethod]
        public void ShouldHasExceptionLoggerWhenThrowException()
        {
            var service = provider.GetService<ILoggingTestService>();
            Assert.ThrowsException<NotImplementedException>(() => { service.Wrong(); });
            var loggerEntry = loggerStore.Last();
            Assert.AreEqual(LogLevel.Error, loggerEntry.LogLevel);
            Assert.IsNotNull(loggerEntry.Exception);
            Assert.AreEqual(typeof(InvokeLogAttribute).FullName, loggerEntry.CategoryName);
            Assert.AreEqual("Error occurred when invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Wrong\".", loggerEntry.Message);
        }
    }

    [Service]
    public class LoggingTestService : ILoggingTestService
    {
        public void Hello()
        {
            Console.WriteLine("Hello");
        }

        public void Wrong()
        {
            throw new System.NotImplementedException();
        }
    }

    [InvokeLog]
    public interface ILoggingTestService
    {
        void Hello();

        void Wrong();
    }

}
