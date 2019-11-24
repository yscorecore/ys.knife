using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Mvc
{
    public class BaseController<T> : IController<T>, IController
    {
        public BaseController(T view)
        {
            this.View = view;
        }
        public T View
        {
            get; protected set;
        }
        public virtual void Handler()
        {
        }
    }
}
