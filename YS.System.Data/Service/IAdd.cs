

using System.Threading.Tasks;

namespace System.Data.Service
{

    public interface IAdd<T>
    {
        Task<ResultData<T>> Add(T entity);
    }
    
}
