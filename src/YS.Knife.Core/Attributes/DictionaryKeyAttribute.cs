using System;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DictionaryKeyAttribute : Attribute
    {
        public DictionaryKeyAttribute(string key)
        {
            this.Key = key;
        }
        public string Key { get; }
    }
}
