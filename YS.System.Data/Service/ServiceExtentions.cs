using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public static class ServiceExtentions
    {
        public static Task<int> CountAll<EntityType>(this ICount<EntityType> service)
        {
            return service.Count(null);
        }

        public static Task<EntityType> FindByName<EntityType>(this IFindItem<EntityType> service, string name)
            where EntityType : IName
        {
            return service.FindItem(Exp<EntityType>.CreateSearch(p => p.Name == name));
        }

        public static Task<ResultInfo> Delete<EntityType,ID>(this IDeleteById<EntityType, ID> service,EntityType entity)
             where EntityType : IId<ID>
        {
            return service.DeleteById(entity.Id);
        }
    }
}
