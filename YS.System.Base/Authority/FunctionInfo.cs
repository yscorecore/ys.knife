using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Authority
{
    [Serializable]
    public class FunctionInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string ParentId { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public FunctionLevel Level { get; set; }
        public int Sequence { get; set; }
    }
}
