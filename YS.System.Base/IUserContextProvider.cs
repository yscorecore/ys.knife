using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public interface IUserContextProvider<T>
         where T : class
    {

        T UserContext { get; }

        //void DirtyContext();

    }
}
