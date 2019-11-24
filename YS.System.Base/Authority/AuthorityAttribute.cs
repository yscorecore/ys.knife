using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Authority
{
    public abstract class AuthorityAttribute : Attribute
    {
        public AuthorityAttribute(string name)
        {
            this.Name = name;
        }

        public virtual string Code { get; protected set; }
        public abstract FunctionLevel Level { get; }
        public string Name { get; private set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public int Sequence { get; set; }
    }
}
