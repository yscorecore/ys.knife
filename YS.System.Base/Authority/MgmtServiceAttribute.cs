using System;
using System.Collections.Generic;
using System.Text;

namespace System.Authority
{
    public class MgmtServiceAttribute : FunctionAttribute
    {
        public Type EntityType { get; set; }

        public bool CanAdd { get; set; } 

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public MgmtServiceAttribute(string name, string parentCode,Type entityType) : base(name, parentCode)
        {
            this.EntityType = entityType;
        }
    }
}
