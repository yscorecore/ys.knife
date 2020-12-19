using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YS.Knife;
using YS.Knife.Stages;

namespace Microsoft.Extensions.Hosting
{
    public static class StageRunner
    {
        public static void RunStage(this IHost host, string name, CancellationToken cancellation = default)
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            using (var scope = host.Services.CreateScope())
            {
                var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
                var handlersTypes = GetMatchedStageTypes(name, environment.EnvironmentName);
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(typeof(StageRunner));
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    ["StageName"] = name
                }))
                {
                    logger.LogInformation($"There are {handlersTypes.Count} handlers in {name} stage.");
                    for (int i = 0; i < handlersTypes.Count; i++)
                    {
                        if (cancellation.IsCancellationRequested)
                        {
                            break;
                        }
                        var index = i + 1;
                        var handlerType = handlersTypes[i];
                        logger.LogInformation($"[{index:d2}] Starting exec handler {handlerType.FullName}.");
                        var handler = CreateHandler(handlerType, scope.ServiceProvider, logger);
                        handler.Run(cancellation).Wait(cancellation);
                        logger.LogInformation($"[{index:d2}] Finished exec handler {handlerType.FullName}.");
                    }
                }

            }
        }

        private static List<Type> GetMatchedStageTypes(string name, string currentEnvironment)
        {
            var query = from type in AppDomain.CurrentDomain
                    .FindInstanceTypesByAttributeAndBaseType<StageAttribute, IStageService>()
                        let stageAttr = GetMatchedStageAttribute(type, name, currentEnvironment)
                        where stageAttr != null
                        orderby stageAttr.Priority descending
                        select type;
            return query.ToList();

        }

        private static IStageService CreateHandler(Type handlerType, IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, handlerType) as IStageService;
            }
            catch (Exception e)
            {
                logger.LogError($"Create stage handler '{handlerType.FullName}' failure: {e.Message}.");
                throw;
            }
        }
        private static StageAttribute GetMatchedStageAttribute(Type type, string stageName, string currentEnvironment)
        {
            return type
                .GetCustomAttributes(typeof(StageAttribute), true)
                .OfType<StageAttribute>()
                .FirstOrDefault(p => p.IsMatch(stageName, currentEnvironment));
        }


    }
}
