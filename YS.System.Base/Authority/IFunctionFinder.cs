using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Authority
{
    public interface IFunctionFinder
    {
        IEnumerable<FunctionInfo> FindFunctions();
    }
}
