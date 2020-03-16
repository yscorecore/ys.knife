using System;

namespace YS.Knife.Hosting
{
    [OptionsClass("Knife")]
    public class HostOptions
    {
        public string Stage { get; set; } = "";

        public string[] Plugins { get; set; } = new[] { "*.dll" };
    }
}
