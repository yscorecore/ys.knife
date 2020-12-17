using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YS.Knife.Rest.Client.Handlers
{
    public class EnsureStatusCodeMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = await base.SendAsync(request, cancellationToken);
            message.EnsureSuccessStatusCode();
            return message;
        }
    }
}
