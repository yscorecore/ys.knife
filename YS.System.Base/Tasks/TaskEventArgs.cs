using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Tasks
{
    [Serializable]
    public class TaskEventArgs : EventArgs
    {
        public TaskContext Context { get; private set; }
        public TaskEventArgs(TaskContext context)
        {
            this.Context = context;
        }
    }
}
