using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    public interface ISearchItemControl
    {
        SearchCondition GetCondition();
        void ResetValue();
    }
    public interface ISearchControl: ISearchItemControl
    {
        event EventHandler DoSearch;
        bool Visible { get; set; }
        void Search();
    }
}
