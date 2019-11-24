using System.Data.Entity;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface IDeleteById<EntityType,TId>
    {
        Task<ResultInfo> DeleteById(TId id);
    }
}
