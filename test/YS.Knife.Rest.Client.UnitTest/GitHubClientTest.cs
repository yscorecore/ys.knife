using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using YS.Knife.Hosting;
using YS.Knife.Rest.Client.UnitTest.Clients;

namespace YS.Knife.Rest.Client.UnitTest
{

    // 
    public class GitHubClientTest : KnifeHost
    {

        [Fact]
        public async Task ShouldGetGithubIssuesWhenUseClientType()
        {
            var client = this.GetService<GitHubClient>();
            var issues = await client.GetIssues();
            issues.Should().NotBeNull();
            issues.Should().NotBeEmpty();
        }
        [Fact]
        public async Task ShouldGetGithubIssuesWhenUseInterface()
        {
            var client = this.GetService<IGitHubService>();
            var issues = await client.GetIssues();
            issues.Should().NotBeNull();
            issues.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldGetGithubInfoWhenUseInterface()
        {
            var client = this.GetService<IGitHubService>();
            var infos = await client.GetInfo();
            infos.Should().NotBeNull();
            infos.Should().NotBeEmpty();
        }
    }
}
