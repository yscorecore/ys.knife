using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Mvc
{
    public interface IController
    {
        void Handler();
    }
    public interface IController<T> : IController
    {
        T View { get; }
    }
}
