using System.Collections.Generic;

namespace YS.Knife
{
    [Options]
    public class KnifeOptions
    {
        public string Stage { get; set; } = "";
        public string[] PluginPaths { get; set; } = new string[] { "." };
        public Ingores Ignores { get; set; }
    }
    public class Ingores
    {
        public List<string> Assemblies { get; set; }
        public List<string> Types { get; set; }
    }

}
