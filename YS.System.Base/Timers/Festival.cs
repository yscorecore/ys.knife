using System;
using System.Collections.Generic;
using System.Text;

namespace System.Timers
{
    /// <summary>
    /// 节日
    /// </summary>
    public interface IFestival
    {
        string Name{get;}
        string Description { get; set; }
        DateTime DateTime { get; }
    }
    public class SolarFestival
    {

        //public override bool IsFestival (DateTime dt) {
        //    return false;
        //}
        
    }
}
