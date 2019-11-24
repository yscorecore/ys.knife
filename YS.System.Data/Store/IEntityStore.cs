using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Store
{
    public interface IEntityStore<T>: ISave
    {
        void Add(T entity);
        void Delete(T entity);
        IQueryable<T> Query(Expression<Func<T, bool>> conditions);
        void Update(T entity);
        void Update(T entity, params string[] fields);
        T FindByKey(params object[] keyValues);
    }

    public interface ISave
    {
        int SaveChanges();
        Func<int> GetSaveChangesMethod();
    }


    public interface ISaveAsync
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default(CancellationToken));
        Func<CancellationToken, Task<int>> GetSaveChangesAsyncMethod();
    }
   
}