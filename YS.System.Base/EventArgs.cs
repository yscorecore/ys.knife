using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    [Serializable]
    public  class EventArgs<T>:EventArgs
    {
        public EventArgs(T argument)
        {
            this.Argument = argument;
        }
        public T Argument { get; private set; }
    }
}
