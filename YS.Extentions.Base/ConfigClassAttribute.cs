using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple =false)]
    public sealed class ConfigClassAttribute : Attribute
    {

        readonly string configKey;

        // This is a positional argument
        public ConfigClassAttribute(string configKey)
        {
            this.configKey = configKey;

        }

        public string ConfigKey
        {
            get
            {
                return this.configKey;
            }
        }
    }
}
