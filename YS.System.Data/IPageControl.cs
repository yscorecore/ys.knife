using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    public interface IPageControl
    {
        event EventHandler<PageDataEventArgs> PageDataNeeded;
        IPage CurrentPage { get; set; }

        event EventHandler CurrentPageChanged;
        void BringToPage(int pageIndex);
        bool Visible { get; set; }
    }
}
