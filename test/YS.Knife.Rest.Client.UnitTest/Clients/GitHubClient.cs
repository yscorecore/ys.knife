using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace YS.Knife.Rest.Client.UnitTest.Clients
{
    class GitHubClient
    {
        //[TestMethod]
        //public async Task ShouldSuccessWhenGetGithubAndSetHeaderFromRestInfo()
        //{
        //    var factory = this.GetService<IHttpClientFactory>();
        //    var restInfo = new RestInfo
        //    {
        //        BaseAddress = "https://api.github.com/",
        //        DefaultHeaders = new Dictionary<string, string>
        //        {
        //            ["Accept"] = "application/vnd.github.v3+json",
        //            ["User-Agent"] = "RestClient-Test",
        //        },
        //    };
        //    var client = new RestClient(restInfo, factory.CreateClient());
        //    var issues = await client.Get<IEnumerable<GitHubIssue>>(
        //        "/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc");
        //    Assert.IsNotNull(issues);
        //    Assert.IsTrue(issues.Count() > 0);
        //}
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
