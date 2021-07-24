using System.Collections.Generic;

namespace YS.Knife.Data.Query
{
    public interface ISliceData
    {
        bool HasNext { get; }
    }
    public interface ISliceData<TItem, TCursor> : ISliceData
    {
        List<TItem> Items { get; }

        string Next { get; }
    }
}
