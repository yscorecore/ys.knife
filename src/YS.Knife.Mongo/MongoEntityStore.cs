using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YS.Knife.Data;
namespace YS.Knife.Mongo
{
    public class MongoEntityStore<TEntity, TContext> : IEntityStore<TEntity>,IEntityStoreTransactionProvider
        where TContext : MongoContext
        where TEntity : class
    {
        static readonly FilterDefinitionBuilder<TEntity> FilterBuilder = Builders<TEntity>.Filter;
        static readonly UpdateDefinitionBuilder<TEntity> UpdateBuilder = Builders<TEntity>.Update;
        public MongoEntityStore(TContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            this.Context = context;
            this.Store = context.GetCollection<TEntity>();
        }

        public TContext Context { get; }

        public IMongoCollection<TEntity> Store { get; }

        public void Add(TEntity entity)
        {
            if (Context.Session != null)
            {
                Store.InsertOne(Context.Session, entity);
            }
            else
            {
                Store.InsertOne(entity);
            }

        }

        public void Delete(TEntity entity)
        {
            var idMap = GetIdValueMap(entity);
            var filter = FilterBuilder.And(idMap.Select(kv => FilterBuilder.Eq(kv.Key, kv.Value)));

            if (Context.Session != null)
            {
                Store.DeleteOne(Context.Session, filter);
            }
            else
            {
                Store.DeleteOne(filter);
            }
        }

        public TEntity FindByKey(params object[] keyValues)
        {
            var idMap = typeof(TEntity).GetEntityKeyProps().Zip(keyValues, (p, v) => new KeyValuePair<string, object>(p.Name, v));
            var filter = FilterBuilder.And(idMap.Select(kv => FilterBuilder.Eq(kv.Key, kv.Value)));
            if (Context.Session != null)
            {
                return Store.Find(Context.Session, filter).FirstOrDefault();
            }
            else
            {
                return Store.Find(filter).FirstOrDefault();
            }

        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> conditions)
        {
            var querable = Context.Session != null ? Store.AsQueryable(Context.Session) : Store.AsQueryable();
            return conditions != null ? querable.Where(conditions) : querable;


        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity, params string[] fields)
        {

            var idMap = GetIdValueMap(entity);
            var updateFields = (fields ?? Array.Empty<string>()).Except(idMap.Select(p => p.Key)).ToList();
            if (updateFields.Count > 0)
            {
                var filter = FilterBuilder.And(idMap.Select(kv => FilterBuilder.Eq(kv.Key, kv.Value)));
                var updates = updateFields
                    .Select(p => UpdateBuilder.Set(p, typeof(TEntity).GetProperty(p).GetValue(entity))).ToList();
                var allUpdates = UpdateBuilder.Combine(updates);
                if (Context.Session != null)
                {
                    Store.UpdateOne(Context.Session, filter, allUpdates, new UpdateOptions());
                }
                else
                {
                    Store.UpdateOne(filter, allUpdates, new UpdateOptions());
                }
            }
        }

        private IEnumerable<KeyValuePair<string, object>> GetIdValueMap(TEntity entity)
        {
            return typeof(TEntity).GetEntityKeyProps().Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(entity)));
        }

        public ITransactionManagement GetTransactionManagement()
        {
            return new MongoTransactionManagement(Context);
        }

        class MongoTransactionManagement:ITransactionManagement
        {
            private readonly MongoContext _mongoContext;

            public MongoTransactionManagement(MongoContext mongoContext)
            {
                _mongoContext = mongoContext;
            }
            public bool StartTransaction()
            {
                if (_mongoContext.Session == null)
                {
                    _mongoContext.Session = _mongoContext.Client.StartSession();
                    _mongoContext.Session.StartTransaction();
                    return true;
                }
                return false;
            }

            public void CommitTransaction()
            {
                _mongoContext.Session?.CommitTransaction();
            }

            public void RollbackTransaction()
            {
                _mongoContext.Session?.AbortTransaction();
            }

            public void ResetTransaction()
            {
                _mongoContext.Session = null;
            }

            public override bool Equals(object obj)
            {
                if (obj is MongoTransactionManagement mongoTransactionManagement)
                {
                    return this._mongoContext == mongoTransactionManagement._mongoContext;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this._mongoContext.GetHashCode();
            }
        }
    }
    
}
