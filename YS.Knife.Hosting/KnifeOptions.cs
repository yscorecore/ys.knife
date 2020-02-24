using System;

namespace Knife.Hosting
{
    [OptionsClass("")]
    public class HostOptions
    {
        public string Stage { get; set; } = "";
    }
}
