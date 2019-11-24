using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    public class KeyValueInfo:System.Data.Entity.BaseEntity
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
