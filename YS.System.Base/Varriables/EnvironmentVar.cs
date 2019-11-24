using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
    public sealed class EnvironmentVar:DynamicVar
    {
        public EnvironmentVar(string varName,string propName)
            : base(varName,GetHandleByName(propName))
        {
        }
        public EnvironmentVar(string propName)
            : this(propName,propName)
        {
        }
        private static ValueGetHandle GetHandleByName(string propName)
        {
            return new ValueGetHandle(
                () => { return typeof(Environment).GetProperty(propName).GetValue(null,null); });
        }
    }

}
