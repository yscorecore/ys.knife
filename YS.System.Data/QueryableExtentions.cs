using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace System.Data
{
    public static class QueryableExtentions
    {
        public static PageData<T> ToPageData<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            
            return new PageData<T>(query, pageIndex, pageSize);
        }
    }
}
