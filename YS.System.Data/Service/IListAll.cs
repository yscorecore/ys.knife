using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface IListAll<EntityType>
    {
        Task<List<EntityType>> ListAll(SearchCondition condition, OrderCondition orderitems);
    }
}
