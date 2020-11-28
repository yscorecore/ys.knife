using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

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

    public static class LimitDataExtensions
    {
        public static IListSource AsListSource<T>(this ILimitData<T> limtData)
        {
            return new LimitDataListSource<T>(limtData);
        }

        
    }
    public class LimitDataListSource<T> : IListSource
    {
        private ILimitData<T> limitData;
        public LimitDataListSource(ILimitData<T> limitData)
        {
            _ = limitData ?? throw new System.ArgumentNullException(nameof(limitData));
            this.limitData = limitData;
        }
        public bool ContainsListCollection => true;

        public IList GetList()
        {
            return limitData.ListData;
        }
    }
}
