using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Authority
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OperationAttribute : AuthorityAttribute
    {
        public OperationAttribute(string name) : base(name)
        {

        }

        public string GroupCode { get; set; }

        public override FunctionLevel Level
        {
            get
            {
                return FunctionLevel.Operation;
            }
        }
        public virtual void InitFunctionCode(Type type, MethodInfo method)
        {
            //var assemblyName = type.Assembly.GetName().FullName.Split(new char[] { ',' })[0];
            //this.Code = string.Format("{0},{1}/{2}", type.FullName, assemblyName.Trim(),method.Name);
            this.Code = string.Format("{0}/{1}", type.GetFullNameWithAssembly(), method.Name);
        }

    }
}
