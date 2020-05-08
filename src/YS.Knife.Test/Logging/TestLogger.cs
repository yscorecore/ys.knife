using System;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Test.Logging
{
    public class TestLogger : ILogger
    {
        public TestLogger(TestLoggerStore loggerStore, string categoryName)
        {
            this.categoryName = categoryName;
            this.loggerStore = loggerStore;
        }
        private readonly TestLoggerStore loggerStore;
        private readonly string categoryName;
        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDispose();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.loggerStore.Add(new TestLoggerEntry
            {
                CategoryName = categoryName,
                LogLevel = logLevel,
                EventId = eventId,
                Exception = exception,
                Message = Convert.ToString(state, CultureInfo.InvariantCulture)
            });
        }

        class NoopDispose : IDisposable
        {
            void IDisposable.Dispose() { }
        }
    }
}
