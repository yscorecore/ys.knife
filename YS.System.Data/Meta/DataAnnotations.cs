using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 表示数据的单位
    /// </summary>
    public class UnitAttribute:Attribute
    {
        public UnitAttribute(string unit)
        {
            this.Unit = unit;
        }
        public string Unit { get; set; }
    }
}
