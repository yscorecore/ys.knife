using System.IO;
namespace YS.Knife.Options.Functions
{

    [OptionsPostFunction]
    [DictionaryKey("upper")]
    public class UpperCaseFunction : IOptionsPostFunction
    {
        public string Invoke(string context)
        {
            return context?.ToUpperInvariant();
        }
    }
}
