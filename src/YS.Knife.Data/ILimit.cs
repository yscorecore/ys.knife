using System.Collections.Generic;
using System.ComponentModel;

namespace YS.Knife.Data
{
    public interface ILimit : IListSource
    {
        int Offset { get; }
        int Limit { get; }
        int TotalCount { get; }
        bool HasNext { get; }
    }
    public interface ILimitData<TData> : ILimit
    {
        List<TData> ListData { get; }
    }
}
