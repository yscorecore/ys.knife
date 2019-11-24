using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
    [Serializable]
    public abstract class Varriable:IVarriable
    {
        /// <summary>
        /// 获取或设置变量的名称
        /// </summary>
        public virtual string Name { get;  set; }
        /// <summary>
        /// 获取变量的值
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}={1}",this.Name , Convert.ToString(this.GetValue()));
        }


    }

}
