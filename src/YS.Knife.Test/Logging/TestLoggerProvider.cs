using Microsoft.Extensions.Logging;

namespace YS.Knife.Test.Logging
{
    public sealed class TestLoggerProvider : ILoggerProvider
    {
        public TestLoggerProvider(TestLoggerStore loggerStore)
        {
            this.loggerStore = loggerStore;
        }
        private readonly TestLoggerStore loggerStore;
        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(loggerStore, categoryName);
        }

        public void Dispose()
        {
        }
    }
}
