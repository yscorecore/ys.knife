using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System.Data
{
    public interface IPage : IListSource
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }

    }
    public interface ILimit : IListSource
    {
        int Offset { get; }
        int Limit { get; }
        int TotalCount { get; }
        bool HasNext { get; }
    }

    public static class PageExtentions
    {
        //例如第1-5页
        public static int GetStartRangePageIndex(this IPage pageinfo, int rangeSize)
        {
            return pageinfo.PageIndex / rangeSize * rangeSize;
        }
        public static int GetEndRangePageIndex(this IPage pageinfo, int rangeSize)
        {
            return GetStartRangePageIndex(pageinfo, rangeSize) + rangeSize - 1;
        }
    }
    public interface IPageData<TData> : IPage
    {
        List<TData> ListData { get; }
    }

    public interface ILimitData<TData> : ILimit
    {
        List<TData> ListData { get; }
    }
    [Serializable]
    public class PageData<TData> : IPageData<TData>
    {
        public static PageData<TData> Empty
        {
            get
            {
                return new PageData<TData>(new List<TData>(), 0, int.MaxValue, 0);
            }
        }
        public PageData()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PageData(IQueryable<TData> source, int pageIndex, int pageSize)
        {
            if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize");
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;

            this.TotalCount = source.Count();

           
            this.lst.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PageData(IList<TData> source, int pageIndex, int pageSize)
        {
            if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize");

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.TotalCount = source.Count();
            this.lst.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="listData">listData</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count</param>
        public PageData(IEnumerable<TData> listData, int pageIndex, int pageSize, int totalCount)
        {
            if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize");
          
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.TotalCount = totalCount;
            this.lst.AddRange(listData);
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages
        {
            get 
            {
                if (this.PageSize == 0) return 0;
                var totalcount = this.TotalCount;
                var pagesize = this.PageSize;
                var totalPages = totalcount / pagesize;
                if (totalcount % pagesize > 0)
                    totalPages++;
                return totalPages;
            
            }
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
        private List<TData> lst = new List<TData>();
        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<TData> ListData
        {
            get
            {
                return lst;
            }
        }
    

    

       

        public bool ContainsListCollection
        {
            get { return true ; }
        }

        public IList GetList()
        {
            return this.lst;
        }
    }
    [Serializable]
    public class LimitData<TData> : ILimitData<TData>
    {
        public LimitData()
        {
        }
        public LimitData(IQueryable<TData> source, int offset, int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = source.Count();
            this.lst.AddRange(source.Skip(offset).Take(limit).ToList());
        }
        public LimitData(IList<TData> source, int offset, int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = source.Count();
            this.lst.AddRange(source.Skip(offset).Take(limit).ToList());
        }
        public LimitData(IEnumerable<TData> listData, int offset, int limit,int totalCount)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.lst.AddRange(ListData);
        }
        public bool ContainsListCollection
        {
            get
            {
                return true;
            }
        }

        public bool HasNext
        {
            get
            {
                return this.TotalCount > this.Offset + this.Limit;
            }
        }

        public int Limit { get; set; }

        private readonly List<TData> lst = new List<TData>();
        public List<TData> ListData
        {
            get
            {
                return this.lst;
            }
        }

        public int Offset { get; set; }
       

        public int TotalCount { get; set; }
        

        public IList GetList()
        {
            return this.lst;
        }
    }
}

