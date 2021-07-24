using System.Collections.Generic;

namespace YS.Knife.Data
{
    public interface IPagedData:ILimitList
    {
        int TotalCount { get; }
        bool CanToPage => this.Limit > 0 && this.Offset % this.Limit == 0;
        int PageSize => this.Limit;
        int PageIndex => this.Limit > 0 ? this.Offset / this.Limit + 1 : 1;
    }

    public interface IPagedData<TData> : IPagedData, ILimitList<TData>
    {

    }
}
