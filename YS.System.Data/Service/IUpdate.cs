
using System.Threading.Tasks;

namespace System.Data.Service
{

    public interface IUpdate<T>
    {
        Task<ResultData<T>> Update(T entity);
    }
}
