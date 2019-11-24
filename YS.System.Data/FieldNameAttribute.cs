using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FieldNameAttribute : Attribute
    {
        public FieldNameAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }
        public string FieldName { get; set; }

    }
}
