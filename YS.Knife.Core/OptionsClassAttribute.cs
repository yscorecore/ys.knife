using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple =false)]
    public sealed class OptionsClassAttribute : Attribute
    {
        public OptionsClassAttribute()
        {

        }
        // This is a positional argument
        public OptionsClassAttribute(string configKey)
        {
            this.ConfigKey = configKey;

        }

        public string ConfigKey
        {
            get;
            set;
        }
    }
}
