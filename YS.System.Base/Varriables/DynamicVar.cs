using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Varriables
{
    public class DynamicVar:Varriable
    {
        #region 构造函数
        public DynamicVar()
        {

        }
        public DynamicVar(string name)
            : this(name,null)
        {

        }
        public DynamicVar(string name,ValueGetHandle valuehandle)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            this.Name = name;
            this.Handle = valuehandle;
        }
        #endregion

        public virtual ValueGetHandle Handle { get; set; }

        public override object GetValue()
        {
            if(this.Handle != null)
            {
                return this.Handle.Invoke();
            }
            else
            {
                return null;
            }
        }

    }
}
