using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Rest.Client.UnitTest
{
    [TestClass]
    public class RestClientTest : YS.Knife.Hosting.KnifeHost
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var factory = this.GetService<IHttpClientFactory>();
            var client = new RestClient("https://api.github.com/", factory.CreateClient());
            var issues = await client.Get<IEnumerable<GitHubIssue>>("/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc",
                new ApiArgument("Accept", ArgumentSource.FromHeader, "application/vnd.github.v3+json"),
                new ApiArgument("User-Agent", ArgumentSource.FromHeader, "HttpClientFactory-Sample")
            );
            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count() > 0);
        }


        /// <summary>
        /// A partial representation of an issue object from the GitHub API
        /// </summary>
        public class GitHubIssue
        {
            [JsonPropertyName("html_url")]
            public string Url { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("created_at")]
            public DateTime Created { get; set; }
        }
    }
}
