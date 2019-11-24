using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    /// <summary>
    /// 表示分页的信息
    /// </summary>
    [Serializable]
    public struct PageInfo
    {
        private int pageSize;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }
        private int pageIndex;

        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }
        public PageInfo(int pageIndex, int pageSize)
        {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
        }
    }
    ///// <summary>
    ///// 表示分页信息2
    ///// </summary>
    //[Serializable]
    //public struct LimitInfo
    //{
    //    /// <summary>
    //    /// 表示要忽略的条数
    //    /// </summary>
    //    public int Offset { get; set; }
    //    /// <summary>
    //    ///表示查询限制的最大数量
    //    /// </summary>
    //    public int Limit { get; set; }

    //    public LimitInfo(int offset,int limit)
    //    {
    //        this.Offset = offset;
    //        this.Limit = limit;
    //    }
    //}
}
