using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace YS.Knife.Data
{
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public class SelectInfo
    {
        public List<SelectItem> Items { get; set; }

        public override string ToString()
        {
            if (Items == null) return string.Empty;
            return string.Join(',', Items.Where(p => p != null).Select(p => p.ToString()));
        }
    }
}
