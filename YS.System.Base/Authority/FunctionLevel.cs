using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Authority
{
    /// <summary>
    /// 表示功能点的等级
    /// </summary>
    public enum FunctionLevel
    {
        Application = 0,//应用
        Module = 1,//模块
        Function = 2,//功能点
        OperationGroup = 3,//操作组
        Operation = 4//操作
    }
}
