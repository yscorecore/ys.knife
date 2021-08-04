using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class AutoConstructorIgnoreAttribute : Attribute
    {
    }
}
