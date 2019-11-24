using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface IFindItem<EntityType>
    {
        Task<EntityType> FindItem(SearchCondition condition);
    }
}
