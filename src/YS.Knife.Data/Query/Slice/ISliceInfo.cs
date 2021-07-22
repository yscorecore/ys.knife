using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Query
{
    public interface ISliceInfo
    {
        string Start { get; }

        int Limit { get;}
    }
}
