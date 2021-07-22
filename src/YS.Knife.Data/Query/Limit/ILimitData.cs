﻿using System.Collections.Generic;

namespace YS.Knife.Data
{
    public interface ILimitData
    {
        int Offset { get; }
        int Limit { get; }
        int TotalCount { get; }
        bool HasNext { get; }
        bool CanToPage => this.Limit > 0 && this.Offset % this.Limit == 0;
        int PageSize => this.Limit;
        int PageIndex => this.Limit > 0 ? this.Offset / this.Limit + 1 : 1;
    }

    public interface ILimitData<TData> : ILimitData
    {
        List<TData> ListData { get; }
    }
}
