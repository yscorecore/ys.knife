using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface ICreate<T>
    {
        Task<ResultData<T>> Create();
    }
}
