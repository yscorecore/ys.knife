using System.Data.Entity;

namespace System.Data.Service
{


    public interface ICURDAll<EntityType,T> :
        IDeleteById<EntityType, T>,
        IAdd<EntityType>,
        IUpdate<EntityType>,
        ISelectById<EntityType, T>,
        ICount<EntityType>,
        ICreate<EntityType>,
        IBatch<EntityType>,
        IFindItem<EntityType>,
        IListAll<EntityType>,
        IListPage<EntityType>,
        IListLimit<EntityType>,
        IPatch<EntityType>
    {
        
    }
    public interface ICURDAll<EntityType>: ICURDAll<EntityType, Guid>
    {

    }



}
