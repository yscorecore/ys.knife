using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Rest.Client
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequestHeaderAttribute : Attribute
    {
        public string Key { get; }
        public string Value { get; }
        public RequestHeaderAttribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
