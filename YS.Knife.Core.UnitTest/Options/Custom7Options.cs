using System.ComponentModel.DataAnnotations;

namespace YS.Knife.Options
{
    [OptionsClass()]
    public class Custom7Options
    {
        [Url]
        public string Value { get; set; }
    }
}
