using System;

namespace YS.Knife.Hosting.Web
{
    [OptionsClass("Knife")]
    public class KnifeWebOptions : KnifeOptions
    {
        public string[] MvcParts { get; set; } = new[] { "*.Api" };
    }
}
