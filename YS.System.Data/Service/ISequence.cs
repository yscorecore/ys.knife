using System.Data.Entity;

namespace System.Data.Service
{
    public interface ISequence<T,ID>
        where T :ISequence
    {
        ResultData<int> UpdateSequence(params T[] entities);

        ResultData<int> TrimSequence(params ID[] ids);
       
    }
}
