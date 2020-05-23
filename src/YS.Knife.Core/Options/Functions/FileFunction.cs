using System.IO;
namespace YS.Knife.Options.Functions
{

    [OptionsPostFunction]
    [DictionaryKey("file")]
    public class FileFunction : IOptionsPostFunction
    {
        public string Invoke(string context)
        {
            return File.ReadAllText(context);
        }
    }
}
