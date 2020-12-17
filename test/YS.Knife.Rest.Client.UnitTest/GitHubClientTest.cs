using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Hosting;
using YS.Knife.Rest.Client.UnitTest.Clients;

namespace YS.Knife.Rest.Client.UnitTest
{

    // [TestClass]
    public class GitHubClientTest : KnifeHost
    {

        [TestMethod]
        public async Task ShouldGetGithubIssuesWhenUseClientType()
        {
            var client = this.GetService<GitHubClient>();
            var issues = await client.GetIssues();
            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Any());
        }
        [TestMethod]
        public async Task ShouldGetGithubIssuesWhenUseInterface()
        {
            var client = this.GetService<IGitHubService>();
            var issues = await client.GetIssues();
            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Any());
        }

        [TestMethod]
        public async Task ShouldGetGithubInfoWhenUseInterface()
        {
            var client = this.GetService<IGitHubService>();
            var infos = await client.GetInfo();
            Assert.IsNotNull(infos);
            Assert.IsTrue(infos.Count> 0);
        }
    }
}
