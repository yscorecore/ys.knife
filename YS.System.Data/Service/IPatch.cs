using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface IPatch<T>
    {
        Task<ResultData<T>> Patch(T entity, params string[] fields);
    }
}
