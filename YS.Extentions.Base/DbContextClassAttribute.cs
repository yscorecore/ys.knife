using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DbContextClassAttribute:Attribute
    {
        public string ConnectStringKey { get; set; }

        public string ConfigKey { get; set; }
    }
}
