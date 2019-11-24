using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface IListLimit<EntityType>
    {
        Task<LimitData<EntityType>> ListLimit(SearchCondition condition, OrderCondition orderItems, int offset,int limit);
    }
}
