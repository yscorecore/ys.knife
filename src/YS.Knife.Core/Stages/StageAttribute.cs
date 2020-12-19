using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Stages
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class StageAttribute : Attribute
    {
        public StageAttribute(string stageName)
        {
            this.StageName = stageName;
        }

        /// <summary>
        /// Stage的名称，忽略大小写匹配
        /// </summary>
        public string StageName { get; set; }

        /// <summary>
        /// 匹配的环境名称，如果为"*"则表示匹配所有环境，否则忽略大小写匹配
        /// </summary>
        public string Environment { get; set; } = "*";

        /// <summary>
        /// 优先级，优先级大的先执行
        /// </summary>

        public int Priority { get; set; } = 10000;

        public bool IsMatch(string stageName, string environmentName)
        {
            return IsMatchStage(stageName) && IsMatchEnv(environmentName);
        }

        private bool IsMatchEnv(string environmentName)
        {
            return Environment == "*" ||
                   string.Equals(Environment, environmentName, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsMatchStage(string stageName)
        {
            return string.Equals(StageName, stageName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
