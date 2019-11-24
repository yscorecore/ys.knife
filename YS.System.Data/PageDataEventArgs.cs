using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    [Serializable]
    public class PageDataEventArgs : EventArgs
    {
        public PageDataEventArgs(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public IPage ResultPage { get; set; }
    }
}
