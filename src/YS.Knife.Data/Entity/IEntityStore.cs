using System;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife
{
    public interface IEntityStore<T> : IEntityReadStore<T>, IEntityWriteStore<T>
    {
    }

    public interface IEntityReadStore<T>
    {
        IQueryable<T> Query(Expression<Func<T, bool>> conditions);
        T FindByKey(params object[] keyValues);
    }
    public interface IEntityWriteStore<in T>
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity, params string[] fields);
    }

}
