using System;
using System.Linq;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YS.Knife.Data;
using YS.Knife.Data.Transactions;

namespace YS.Knife.Mongo
{
    public abstract class MongoContext : ITransactionManagerProvider
    {
        public MongoContext(IMongoDatabase database)
        {
            _ = database ?? throw new ArgumentNullException(nameof(database));
            this.Client = database.Client;
            this.Database = database;
            this.transactionManagement = new Lazy<ITransactionManagement>(() => new MongoTransactionManagement(this), true);
            this.InitMongoCollectionProperties();
        }
        private Lazy<ITransactionManagement> transactionManagement;
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }

        public IClientSessionHandle Session { get; internal set; }
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
        public ITransactionManagement GetTransactionManagement()
        {
            return transactionManagement.Value;
        }

        class MongoTransactionManagement : ITransactionManagement
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
        }
    }
}

