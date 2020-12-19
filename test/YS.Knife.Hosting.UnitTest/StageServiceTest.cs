using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YS.Knife.Hosting;
using YS.Knife.Stages;
using YS.Knife.Testing;

namespace YS.Knife
{
    [TestClass]
    public class StageServiceTest
    {

        [TestInitialize]
        public void Setup()
        {
            InvokeHistory.Reset();
        }

        [TestMethod]
        public void ShouldRunOneStageServiceWhenRunStageTempWithAnyEnvironment()
        {
            using (var host = new KnifeHost(new[] { "Knife:Stage=temp" }))
            {
                host.Run();
            }
            Assert.AreEqual(1, InvokeHistory.Instance.Count);
            Assert.AreEqual(typeof(TempStageInAllEnvironment), InvokeHistory.Instance.First());

        }
        [TestMethod]
        public void ShouldRunTwoStageServiceWhenRunStageTempWithUatEnvironment()
        {
            using (Utility.WithDotnetEnv("uat"))
            {
                using (var host = new KnifeHost(new[] { "Knife:Stage=temp" }))
                {
                    host.Run();
                }
                Assert.AreEqual(2, InvokeHistory.Instance.Count);
                Assert.AreEqual(typeof(TempStageInUatEnvironment), InvokeHistory.Instance.First());
                Assert.AreEqual(typeof(TempStageInAllEnvironment), InvokeHistory.Instance.Last());
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
