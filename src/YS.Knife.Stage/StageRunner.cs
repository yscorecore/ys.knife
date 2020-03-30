using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YS.Knife.Stage;

namespace Microsoft.Extensions.Hosting
{
    public static class StageRunner
    {
        public static void RunStage(this IHost host, string name, CancellationToken cancellation=default)
        {
            using (var scope = host.Services.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IStageService>>().Where(p => string.Equals(name, p.StageName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(typeof(StageRunner));
                logger.LogInformation($"There are {handlers.Count} handlers in {name} stage.");
                for (int i = 0; i < handlers.Count; i++)
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        break;
                    }
                    var index = i + 1;
                    var handler = handlers[i];
                    logger.LogInformation($"[{index:d2}] Start exec handler {handler.GetType().Name}.");
                    handler.Run(cancellation).Wait();
                }
            }
        }
    }
}