using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ConvertToAttribute : Attribute
    {
        public ConvertToAttribute()
        {

        }
        public ConvertToAttribute(string sourceType, string targetType)
        {

            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public string SourceType { get; set; }
        public string TargetType { get; set; }
    }
}
