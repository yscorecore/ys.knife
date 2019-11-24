using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
   public  class KeyValuedVar:Varriable
    {
       public KeyValuedVar()
       {
       }
       public KeyValuedVar(string key,object value)
       {
           this.Name = key;
           this.Value = value;
       }
        public object Value { get; set; }
        public override object GetValue()
        {
            return this.Value;
        }
    }
}
