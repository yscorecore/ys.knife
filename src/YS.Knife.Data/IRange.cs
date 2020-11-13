using System.Collections.Generic;
using System.ComponentModel;

namespace YS.Knife.Data
{
    public interface IRange<out T>:IListSource
    {
        int Limit { get; }
        int TotalCount { get; }
        T NextKey { get; }
    }

    public interface IRangeData<out TKey, TItem>:IRange<TKey>
    {
        List<TItem> Items { get; }
    }
}
