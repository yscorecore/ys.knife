using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
    [Serializable]
    public class VarriableCollection:KeyedList<Varriable,string>
    {
        public void Add(string name,object value)
        {
            this.Add (new KeyValuedVar (name,value ));
        }
        public void Add(string name,ValueGetHandle valueHandle)
        {
            this.Add(new DynamicVar(name,valueHandle));
        }
        public override string GetItemKey(Varriable item)
        {
            return item.Name;
        }

    }
}
