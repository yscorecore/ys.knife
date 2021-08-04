using System;
// ReSharper disable once CheckNamespace
namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class AutowiredAttribute : Attribute
    {
    }
}
