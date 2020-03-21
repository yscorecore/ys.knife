using System.Collections.Generic;

namespace YS.Knife
{
    [OptionsClass]
    public class KnifeOptions
    {
        public string Stage { get; set; } = "";
        public List<string> Plugins { get; set; } = new List<string> { "*.dll" };
        public Ingores Ignores { get; set; }
    }
    public class Ingores
    {
        public List<string> Assemblies { get; set; }
        public List<string> Types { get; set; }
    }
}
