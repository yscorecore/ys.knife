using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Tasks
{
    [Serializable]
    public class StepResult
    {
        public Exception Exception { get; set; }
        public object Result { get; set; }
    }
}
