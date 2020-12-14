using System;
using System.Linq;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YS.Knife.Data;

namespace YS.Knife.Mongo
{
    public abstract class MongoContext
    {
        public MongoContext(IMongoDatabase database)
        {
            _ = database ?? throw new ArgumentNullException(nameof(database));
            this.Client = database.Client;
            this.Database = database;
            this.InitMongoCollectionProperties();
        }
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }

        public IClientSessionHandle Transaction { get; internal set; }
        private LocalCache<Type, object> collectionCache = new LocalCache<Type, object>();

        public IMongoCollection<T> GetCollection<T>()
        {
            return collectionCache.Get(typeof(T), (type) =>
             {
                 string collectionName = MongoCollectionNameAttribute.GetCollectionName(type);
                 return Database.GetCollection<T>(collectionName);
             }) as IMongoCollection<T>;
        }

        private void InitMongoCollectionProperties()
        {
            var propertyInfos = this.GetType().GetProperties()
                  .Where(IsMongoCollectionProperty)
                  .ToList();
            var method = typeof(MongoContext).GetMethod(nameof(GetCollection));
            foreach (var prop in propertyInfos)
            {
                var entityType = prop.PropertyType.GetGenericArguments().First();
                var mongoCollection = method.MakeGenericMethod(entityType).Invoke(this, null);
                prop.SetValue(this, mongoCollection);
            }

        }
        private bool IsMongoCollectionProperty(PropertyInfo prop)
        {
            return prop.CanRead && prop.CanWrite &&
                prop.PropertyType.IsGenericType &&
                prop.PropertyType.GetGenericTypeDefinition() == typeof(IMongoCollection<>);
        }
    }
}

