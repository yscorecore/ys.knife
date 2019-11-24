using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
    public  interface IVarriable
    {
        string Name { get; }
        object GetValue();
    }
}
