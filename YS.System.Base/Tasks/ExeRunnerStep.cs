using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System.Tasks
{
    /// <summary>
    /// 表示执行exe文件的步骤
    /// </summary>
    [Serializable]
    public class ExeRunnerStep : Step
    {
        public ExeRunnerStep()
        {
            //this.StartWindowStyle = ProcessWindowStyle.Normal;
        }
        public string FilePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string StartArguments { get; set; }
        public ProcessWindowStyle StartWindowStyle { get; set; }
        //public bool Block { get; set; }
        public override StepResult Run(TaskContext tackContext)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = FilePath;
                info.Arguments = tackContext.TextTranslate.ExpandEnvironmentVariables(StartArguments) as string;
                info.WorkingDirectory = tackContext.TextTranslate.ExpandEnvironmentVariables(WorkingDirectory) as string;
                info.WindowStyle = StartWindowStyle;
                var p = Process.Start(info);
                p.WaitForExit();
                return new StepResult();
            }
            catch (Exception ex)
            {
                return new StepResult() { Exception = ex };
            }
        }
    }
}
