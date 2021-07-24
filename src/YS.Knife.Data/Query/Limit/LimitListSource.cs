using System.Collections;
using System.ComponentModel;

namespace YS.Knife.Data.Query
{
    public class LimitListSource<T> : IListSource
    {
        private readonly ILimitList<T> limitList;
        public LimitListSource(ILimitList<T> limitList)
        {
            _ = limitList ?? throw new System.ArgumentNullException(nameof(limitList));
            this.limitList = limitList;
        }
        public bool ContainsListCollection => true;

        public IList GetList()
        {
            return limitList.Items;
        }
    }
}
