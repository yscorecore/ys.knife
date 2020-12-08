using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YS.Knife.Data;
namespace YS.Knife.Mongo
{
    public class MongoEntityStore<TEntity, TContext> : IEntityStore<TEntity>
        where TContext : MongoContext
        where TEntity : class
    {
        static readonly FilterDefinitionBuilder<TEntity> Filter = Builders<TEntity>.Filter;
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
            Store.InsertOne(entity);
        }

        public void Delete(TEntity entity)
        {
            var idMap = GetIdValueMap(entity);
            var filter = Filter.And(idMap.Select(kv => Filter.Eq(kv.Key, kv.Value)));
            Store.DeleteOne(filter);
        }

        public TEntity FindByKey(params object[] keyValues)
        {
            var idMap = typeof(TEntity).GetEntityKeyProps().Zip(keyValues, (p, v) => new KeyValuePair<string, object>(p.Name, v));
            var filter = Filter.And(idMap.Select(kv => Filter.Eq(kv.Key, kv.Value)));
            return Store.Find(filter).FirstOrDefault();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> conditions)
        {
            return conditions != null ? Store.AsQueryable().Where(conditions) : Store.AsQueryable();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity, params string[] fields)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<KeyValuePair<string, object>> GetIdValueMap(TEntity entity)
        {
            return typeof(TEntity).GetEntityKeyProps().Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(entity)));
        }
    }
}
