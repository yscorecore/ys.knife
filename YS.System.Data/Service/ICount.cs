using System.Threading.Tasks;

namespace System.Data.Service
{

    public interface ICount<T>
    {
        Task<int> Count(SearchCondition conditions);
    }
}
