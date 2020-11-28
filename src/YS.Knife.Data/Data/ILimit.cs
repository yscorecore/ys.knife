using System.Collections.Generic;

namespace YS.Knife.Data
{
    public interface ILimit 
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
