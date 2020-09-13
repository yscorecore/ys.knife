using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YS.Knife.Rest.Client.UnitTest.Clients
{
    [RequestHeader("Accept", "application/vnd.github.v3+json")]
    [RequestHeader("User-Agent", "RestClient-Test")]
    [RestClient("https://api.github.com/")]
    public class GitHubClient : RestClient, IGitHubService
    {

        public GitHubClient(RestInfo<GitHubClient> restInfo, HttpClient httpClient) : base(restInfo, httpClient)
        {

        }

        public Task<IEnumerable<GitHubIssue>> GetIssues()
        {
            return this.Get<IEnumerable<GitHubIssue>>("/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc");
        }

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

    public interface IGitHubService
    {
        Task<IEnumerable<GitHubIssue>> GetIssues();
    }
}
