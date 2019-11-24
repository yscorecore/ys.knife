using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    [Serializable]
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public ExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
        }
    }
}
