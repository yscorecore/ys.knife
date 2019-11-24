using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Authority
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class ApplicationAttribute : AuthorityAttribute
    {
        public ApplicationAttribute(string name, string code) : base(name)
        {
            this.Code = code;
        }
        public override FunctionLevel Level
        {
            get
            {
                return FunctionLevel.Application;
            }
        }
    }
}
