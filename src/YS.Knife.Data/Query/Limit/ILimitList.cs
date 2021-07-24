using System.Collections.Generic;

namespace YS.Knife.Data.Query
{
    public interface ILimitList
    {
        int Offset { get; }
        int Limit { get; }
        bool HasNext { get; }
    }
    public interface ILimitList<T> : ILimitList
    {
        List<T> Items { get; }
    }
}
