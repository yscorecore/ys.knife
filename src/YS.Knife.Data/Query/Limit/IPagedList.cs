namespace YS.Knife.Data.Query
{
    public interface IPagedList : ILimitList
    {
        int TotalCount { get; }
        bool CanToPage => this.Limit > 0 && this.Offset % this.Limit == 0;
        int PageSize => this.Limit;
        int PageIndex => this.Limit > 0 ? this.Offset / this.Limit + 1 : 1;
    }

    public interface IPagedList<TData> : IPagedList, ILimitList<TData>
    {

    }
}
