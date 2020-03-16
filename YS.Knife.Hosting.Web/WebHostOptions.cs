using System;

namespace YS.Knife.Hosting.Web
{
    [OptionsClass("Knife")]
    public class WebHostOptions:HostOptions
    {
        public string[] MvcParts { get; set; } = new[] { "*.Api" };
    }
}
