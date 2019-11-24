using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface IListPage<EntityType>
    {
        Task<PageData<EntityType>> ListPage(SearchCondition condition, OrderCondition orderitems, int pageIndex,int pageSize);
    }
}
