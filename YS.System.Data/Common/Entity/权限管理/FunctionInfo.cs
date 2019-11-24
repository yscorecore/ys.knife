using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace System.Data.Common
{
   public class FunctionInfo<TKey> : BaseNamedEntity<TKey>,ISequence, ISelfTree<TKey>
    {
        public TKey ApplicationId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public TKey ParentId { get; set; }

        public FunctionLevel Level { get; set; }

        public string Icon { get; set; }

        public int Sequence { get; set; }
    }
    public class FunctionInfo : FunctionInfo<Guid>
    {

    }
}
