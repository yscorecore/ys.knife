using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Varriables;

namespace System.Tasks
{
    [Serializable]
    public class TaskContext
    {
        SuperVarExplain textTranslate = new SuperVarExplain();

        public SuperVarExplain TextTranslate
        {
            get { return textTranslate; }
        }
        Dictionary<Step, StepResult> dic = new Dictionary<Step, StepResult>();
        /// <summary>
        /// 获取或设置一个值，该值表示是否执行后面的<see cref="System.Task.Step"/>
        /// </summary>
        public bool Canceled { get; set; }
        public void AddStepResult(Step step, StepResult result)
        {
            // if (!dic.ContainsKey(step))
            {
                dic.Add(step, result);
            }
        }
        public void AddVarriable(string name, object value)
        {
            this.textTranslate.Varriables.Add(name, value);

        }
        public IEnumerable<KeyValuePair<Step, StepResult>> GetStepResults()
        {
            return this.dic;

        }
    }
}
