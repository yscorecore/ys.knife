using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YS.Knife.Hosting;
using YS.Knife.Stages;

namespace YS.Knife
{
    [TestClass]
    public class StageServiceTest : KnifeHost
    {
        public StageServiceTest() : base(new string[] { "Knife:Stage=temp" })
        {
        }

        [Inject]
        IStageService tempStageService;

        [TestMethod]
        public void ShouldRunTempStageServiceWhenRunStageTempWithAnyEvnironment()
        {
            tempStageService = Mock.Of<IStageService>(p => p.StageName == "temp" && p.EnvironmentName == "*");
            this.Run();
            Mock.Get(tempStageService).Verify(p => p.Run(default), Times.Once);
        }
        [TestMethod]
        public void ShouldNeverRunTempStageServiceWhenRunStageTempWithNoEnvironment()
        {
            tempStageService = Mock.Of<IStageService>(p => p.StageName == "temp");
            var appLiftTime = this.GetRequiredService<IHostApplicationLifetime>();
            Task.Delay(500).ContinueWith(_ => { appLiftTime.StopApplication(); });
            this.Run();
            Mock.Get(tempStageService).Verify(p => p.Run(default), Times.Never);
        }
    }
}
