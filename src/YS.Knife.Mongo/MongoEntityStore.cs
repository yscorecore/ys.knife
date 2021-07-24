using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YS.Knife.Data;

namespace YS.Knife.Mongo
{
    public class MongoEntityStore<TEntity, TContext> : IEntityStore<TEntity>, IMongoContextConsumer
        where TContext : MongoContext
        where TEntity : class
    {
        static readonly FilterDefinitionBuilder<TEntity> FilterBuilder = Builders<TEntity>.Filter;
        static readonly UpdateDefinitionBuilder<TEntity> UpdateBuilder = Builders<TEntity>.Update;
        public MongoEntityStore(TContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            this.Context = context;
            this.Collection = context.GetCollection<TEntity>();
        }

        public TContext Context { get; }

        public IMongoCollection<TEntity> Collection { get; }

        MongoContext IMongoContextConsumer.MongoContext => this.Context;

        public void Add(TEntity entity)
        {
            if (Context.Session != null)
            {
                Collection.InsertOne(Context.Session, entity);
            }
            else
            {
                Collection.InsertOne(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            var idMap = GetIdValueMap(entity);
            var filter = FilterBuilder.And(idMap.Select(kv => FilterBuilder.Eq(kv.Key, kv.Value)));

            if (Context.Session != null)
            {
                Collection.DeleteOne(Context.Session, filter);
            }
            else
            {
                Collection.DeleteOne(filter);
            }
        }

        public TEntity FindByKey(params object[] keyValues)
        {
            var idMap = typeof(TEntity).GetEntityKeyProps().Zip(keyValues, (p, v) => new KeyValuePair<string, object>(p.Name, v));
            var filter = FilterBuilder.And(idMap.Select(kv => FilterBuilder.Eq(kv.Key, kv.Value)));
            if (Context.Session != null)
            {
                return Collection.Find(Context.Session, filter).FirstOrDefault();
            }
            else
            {
                return Collection.Find(filter).FirstOrDefault();
            }

        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> conditions)
        {
            var querable = Context.Session != null ? Collection.AsQueryable(Context.Session) : Collection.AsQueryable();
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
                    Collection.UpdateOne(Context.Session, filter, allUpdates, new UpdateOptions());
                }
                else
                {
                    Collection.UpdateOne(filter, allUpdates, new UpdateOptions());
                }
            }
        }

        private IEnumerable<KeyValuePair<string, object>> GetIdValueMap(TEntity entity)
        {
            return typeof(TEntity).GetEntityKeyProps().Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(entity)));
        }



    }

}
