using System.Data.Entity;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface ISelectById<EntityType,T>

    {
        Task<EntityType> FindById(T id);
    }
   
}
