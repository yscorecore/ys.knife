using System;

namespace Knife
{
    [OptionsClass("App")]
    public class WebAppOptions : AppOptions
    {
        public string[] MvcParts { get; set; } = new[] { "*.Api" };
    }
}
