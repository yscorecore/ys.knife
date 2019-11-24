using System;
using System.Collections.Generic;
using System.Text;

namespace System.Record
{

    /// <summary>
    /// 表示 操作 的基类
    /// </summary>
    public abstract class OperationBase
    {
        /// <summary>
        /// 设置到操作的新值。
        /// </summary>
      public abstract  void SetNewValue ();
        /// <summary>
        /// 设置到操作的旧值。
        /// </summary>
      public abstract void SetOldValue ();
      /// <summary>
      /// 获取或设置操作的描述。
      /// </summary>
      public string Description { get; set; }    
    }
}
