using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Expressions
{
    public interface ISliceData
    {
        bool HasNext { get; }
    }
    public interface ISliceData<TItem,TKey> : ISliceData
    { 
        List<TItem> Items { get; }

        string Next { get; }
    }
}
