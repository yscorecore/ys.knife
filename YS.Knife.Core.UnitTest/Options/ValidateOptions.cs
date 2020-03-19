using System.ComponentModel.DataAnnotations;

namespace YS.Knife.Options
{
    [OptionsClass()]
    public class ValidateOptions
    {
        [Url]
        public string Value { get; set; }
    }
}
