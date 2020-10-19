using System.Net.Http;
using System.Threading.Tasks;
using YS.Knife.Rest.Client.Handlers;

namespace YS.Knife.Rest.Client.UnitTest.Clients
{
    [RestClient("https://1e863a9d-ad99-446d-985b-153f28bf6c1c.mock.pstmn.io", typeof(UnwrapCodeMessageHandler))]
    public class MockClient : RestClient
    {
        public MockClient(RestInfo<MockClient> restInfo, HttpClient httpClient) : base(restInfo, httpClient)
        {

        }
        public Task<int> GetValue()
        {
            return this.Get<int>("success");
        }
        public Task<Person> GetObject()
        {
            return this.Get<Person>("object");
        }
    }
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
