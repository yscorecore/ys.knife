using System;

namespace YS.Knife.Hosting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]

    public class InjectConfigurationAttribute : System.Attribute
    {
        public InjectConfigurationAttribute(string configurationKey)
        {
            this.ConfigurationKey = configurationKey;
        }
        public string ConfigurationKey { get; set; }
    }
}
