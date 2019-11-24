using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Tasks
{
    /// <summary>
    /// 表示任务
    /// </summary>
    [Serializable]
    public sealed class Task : List<Step>
    {
        public event EventHandler<TaskEventArgs> BeforeTaskExec;
        public event EventHandler<TaskEventArgs> AfterTaskExec;
        public event EventHandler<ExceptionEventArgs> UnhandledExceptionOccured;
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public void Exec(TaskContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            TaskEventArgs arg = new TaskEventArgs(context);
            this.OnBeforeTaskExec(arg);
            //开始执行任务
            foreach (Step step in this)
            {
                if (!context.Canceled)
                {
                    try
                    {
                        //开始执行步骤
                        StepResult v = step.Run(context);
                        //接受执行步骤
                        context.AddStepResult(step, v);
                    }
                    catch (Exception ex)//没有处理的step异常
                    {
                        this.OnUnhandledExceptionOccured(new ExceptionEventArgs(ex));
                    }
                }
            }
            //完成执行任务
            this.OnAfterTaskExec(arg);
        }
        private void OnBeforeTaskExec(TaskEventArgs e)
        {
            if (this.BeforeTaskExec != null)
            {
                this.BeforeTaskExec(this, e);
            }
        }
        private void OnAfterTaskExec(TaskEventArgs e)
        {
            if (this.AfterTaskExec != null)
            {
                this.AfterTaskExec(this, e);
            }
        }
        private void OnUnhandledExceptionOccured(ExceptionEventArgs e)
        {
            if (this.UnhandledExceptionOccured != null)
            {
                this.UnhandledExceptionOccured(this, e);
            }
        }
    }
}
