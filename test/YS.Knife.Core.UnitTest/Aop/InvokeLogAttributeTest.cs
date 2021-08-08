using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using YS.Knife.Testing.Logging;

namespace YS.Knife.Aop.UnitTest
{

    public class InvokeLogAttributeTest
    {

        private IServiceProvider provider;

        public InvokeLogAttributeTest()
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

        [Fact]
        public void ShouldHasStartLoggerWhenInvokeMethod()
        {
            var service = provider.GetService<ILoggingTestService>();
            service.Hello();
            loggerStore.First()
                  .Should().BeEquivalentTo(new TestLoggerEntry
                  {
                      LogLevel = LogLevel.Information,
                      Message = "Start invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Hello\".",
                      CategoryName = typeof(InvokeLogAttribute).FullName,
                      Exception = null

                  });

        }

        [Fact]
        public void ShouldHasEndLoggerWhenInvokeMethod()
        {
            var service = provider.GetService<ILoggingTestService>();
            service.Hello();
            loggerStore.Last()
                  .Should().BeEquivalentTo(new TestLoggerEntry
                  {
                      LogLevel = LogLevel.Information,
                      Message = "End invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Hello\".",
                      CategoryName = typeof(InvokeLogAttribute).FullName,
                      Exception = null

                  });

        }

        [Fact]
        public void ShouldHasStartLoggerWhenThrowException()
        {
            var service = provider.GetService<ILoggingTestService>();
            var action = new Action(() => { service.Wrong(); });
            action.Should().Throw<NotImplementedException>();
            var loggerEntry = loggerStore.First();
            loggerEntry.LogLevel.Should().Be(LogLevel.Information);
            loggerEntry.Exception.Should().BeNull();
            loggerEntry.CategoryName.Should().Be(typeof(InvokeLogAttribute).FullName);
            loggerEntry.Message.Should().Be("Start invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Wrong\".");
        }

        [Fact]
        public void ShouldHasExceptionLoggerWhenThrowException()
        {
            var service = provider.GetService<ILoggingTestService>();
            var action = new Action(() => { service.Wrong(); });
            action.Should().Throw<NotImplementedException>();
            var loggerEntry = loggerStore.Last();
            loggerEntry.LogLevel.Should().Be(LogLevel.Error);
            loggerEntry.Exception.Should().BeOfType<NotImplementedException>();
            loggerEntry.CategoryName.Should().Be(typeof(InvokeLogAttribute).FullName);
            loggerEntry.Message.Should().Be("Error occurred when invoke method \"YS.Knife.Aop.UnitTest.LoggingTestService.Wrong\".");
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
