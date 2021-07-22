using System.Collections;
using System.ComponentModel;

namespace YS.Knife.Data
{
    public class LimitDataListSource<T> : IListSource
    {
        private readonly ILimitData<T> _limitData;
        public LimitDataListSource(ILimitData<T> limitData)
        {
            _ = limitData ?? throw new System.ArgumentNullException(nameof(limitData));
            this._limitData = limitData;
        }
        public bool ContainsListCollection => true;

        public IList GetList()
        {
            return _limitData.ListData;
        }
    }
}
