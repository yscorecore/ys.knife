using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    [Obsolete()]
    public interface ITypeService
    {
        Type GetTypeByName(string typeName);
    }
}
