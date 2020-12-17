using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
    public interface IEntityWriteStore<T>
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity, params string[] fields);
    }

}
