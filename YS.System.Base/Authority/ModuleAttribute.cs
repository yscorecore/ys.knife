using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Authority
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class ModuleAttribute : AuthorityAttribute
    {
        public ModuleAttribute(string name, string code, string parentCode) : this(name, code)
        {
            this.ParentCode = parentCode;
        }
        private ModuleAttribute(string name, string code) : base(name)
        {
            this.Code = code;
        }

        public string ParentCode { get; set; }
        public override FunctionLevel Level
        {
            get
            {
                return FunctionLevel.Application;
            }
        }
    }
}
