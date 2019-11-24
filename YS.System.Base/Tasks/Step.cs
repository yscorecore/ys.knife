using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Tasks
{
    /// <summary>
    /// 表示步骤的基类
    /// </summary>
    [Serializable]
    public abstract class Step
    {
        public string StepName { get; set; }
        public string StepDescription { get; set; }
        public abstract StepResult Run(TaskContext tackContext);
    }
}
