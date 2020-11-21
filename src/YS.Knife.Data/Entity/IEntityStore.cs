using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace YS.Knife
{
    public interface IEntityStore<T>: IEntityReadStore<T>,IEntityWriteStore<T>
    {
    }

    public interface IEntityReadStore<T>
    {
        IQueryable<T> Query(Expression<Func<T, bool>> conditions);
        T FindByKey(params object[] keyValues);
    }
    public interface IEntityWriteStore<T>:ISave,ISaveAsync, ITransactionStep
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        void Update(T entity, params string[] fields);
    }


    public interface ISave
    {

        int SaveChanges();
    }


    public interface ISaveAsync
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default(CancellationToken));
    }


    public interface ITransactionContext : IDisposable
    {
        List<ITransaction> Transactions { get; }
    }
    public interface ITransaction
    {
        void Commit();
        void Rollback();
    }



    public interface ITransactionStep
    {
        void UseTransaction(ITransactionContext transactionContext);
    }

    public class DatabaseTransaction : ITransaction
    {


        public DatabaseTransaction(DbTransaction dbTransaction)
        {
            this.DbTransaction = dbTransaction;
        }

        public DbTransaction DbTransaction { get; set; }

        public void Commit()
        {
            DbTransaction.Commit();
        }

        public void Rollback()
        {
            DbTransaction.Rollback();
        }
    }
}
