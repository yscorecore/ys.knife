using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Moq;
using YS.Knife.Hosting;
using YS.Knife.Stages;
using YS.Knife.Testing;

namespace YS.Knife
{
    
    public class StageServiceTest
    {

        
        public StageServiceTest()
        {
            InvokeHistory.Reset();
        }

        [Fact]
        public void ShouldRunOneStageServiceWhenRunStageTempWithAnyEnvironment()
        {
            using (var host = new KnifeHost(new[] { "Knife:Stage=temp" }))
            {
                host.Run();
            }
            InvokeHistory.Instance.Count.Should().Be(1);
            InvokeHistory.Instance.First().Should().Be(typeof(TempStageInAllEnvironment));

        }
        [Fact]
        public void ShouldRunTwoStageServiceWhenRunStageTempWithUatEnvironment()
        {
            using (Utility.WithDotnetEnv("uat"))
            {
                using (var host = new KnifeHost(new[] { "Knife:Stage=temp" }))
                {
                    host.Run();
                }
                InvokeHistory.Instance.Count.Should().Be(2);
                InvokeHistory.Instance.First().Should().Be(typeof(TempStageInUatEnvironment));
                InvokeHistory.Instance.Last().Should().Be(typeof(TempStageInAllEnvironment));
            }
        }
    }

    class InvokeHistory : List<Type>
    {
        internal static readonly InvokeHistory Instance = new InvokeHistory();
        public static void Invoke(Type type)
        {
            Instance.Add(type);
        }

        public static void Reset()
        {
            Instance.Clear();
        }
    }
    [Stage("temp", Environment = "*", Priority = 100)]
    class TempStageInAllEnvironment : IStageService
    {
        public Task Run(CancellationToken cancellationToken = default)
        {
            InvokeHistory.Invoke(this.GetType());
            return Task.CompletedTask;
        }
    }

    class ChildClassShouldNotRun : TempStageInAllEnvironment
    {

    }
    [Stage("temp", Environment = "Development")]
    class TempStageInDevelopmentEnvironment : IStageService
    {
        public Task Run(CancellationToken cancellationToken = default)
        {
            InvokeHistory.Invoke(this.GetType());
            return Task.CompletedTask;
        }
    }
    [Stage("temp2", Environment = "*")]
    class Temp2StageInDevelopmentEnvironment : IStageService
    {
        public Task Run(CancellationToken cancellationToken = default)
        {
            InvokeHistory.Invoke(this.GetType());
            return Task.CompletedTask;
        }
    }
    [Stage("temp", Environment = "UAT", Priority = 500)]
    class TempStageInUatEnvironment : IStageService
    {
        public Task Run(CancellationToken cancellationToken = default)
        {
            InvokeHistory.Invoke(this.GetType());
            return Task.CompletedTask;
        }
    }
}
