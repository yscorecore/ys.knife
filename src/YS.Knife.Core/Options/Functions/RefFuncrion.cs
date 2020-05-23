using Microsoft.Extensions.Configuration;
namespace YS.Knife.Options.Functions
{
    [OptionsPostFunction]
    [DictionaryKey("ref")]
    public class RefFunction : IOptionsPostFunction
    {
        public RefFunction(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        private readonly IConfiguration configuration;
        public string Invoke(string context)
        {
            throw new System.NotImplementedException();
        }
    }
}
