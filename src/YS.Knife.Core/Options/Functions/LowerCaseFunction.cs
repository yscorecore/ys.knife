namespace YS.Knife.Options.Functions
{

    [OptionsPostFunction]
    [DictionaryKey("lower")]
    public class LowerCaseFunction : IOptionsPostFunction
    {
        public string Invoke(string context)
        {
            return context?.ToLowerInvariant();
        }
    }
}
