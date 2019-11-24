using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common
{
    public class AreaInfo
    {
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string ParentCode
        {
            get; set;
        }
    }
}
