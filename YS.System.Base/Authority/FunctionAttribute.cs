using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Authority
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FunctionAttribute : AuthorityAttribute
    {
        public FunctionAttribute(string name, string parentCode) : base(name)
        {
            this.ParentCode = parentCode;
        }
        public override FunctionLevel Level
        {
            get
            {
                return FunctionLevel.Function;
            }
        }
        public string ParentCode { get; set; }
        public virtual void InitFunctionCode(Type type)
        {
            //var assemblyName = type.Assembly.GetName().FullName.Split(new char[] { ',' })[0];
            //this.Code = string.Format("{0},{1}", type.FullName, assemblyName.Trim());
            this.Code = type.GetFullNameWithAssembly();
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ArgFunctionAttribute : FunctionAttribute
    {
        public ArgFunctionAttribute(string name, string parentCode) : base(name, parentCode)
        {

        }
        public string MethodName { get; set; }
        public string Arguments { get; set; }

        public override void InitFunctionCode(Type type)
        {
            if (string.IsNullOrEmpty(MethodName))
            {
                base.InitFunctionCode(type);
            }
            else
            {
                this.Code = string.Format("{0}/{1}({2})", type.GetFullNameWithAssembly(), MethodName, Arguments);
            }
        }
    }
}
