using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace YS.Knife.Mongo
{
    [Service(Lifetime = ServiceLifetime.Singleton)]
    internal class DefaultMongoClientFactory : IMongoClientFactory
    {
        private readonly IConfiguration configuration;
        private readonly LocalCache<string, IMongoClient> localCache = new LocalCache<string, IMongoClient>();
        public DefaultMongoClientFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IMongoClient Create(string connectionStringKey)
        {
            return localCache.Get(connectionStringKey, (nm) =>
             {
                 string connectionString = configuration.GetConnectionString(nm);
                 if (string.IsNullOrEmpty(connectionString))
                 {
                     throw new ApplicationException($"Can not find connection string by name \"{nm}\".");
                 }
                 MongoUrlBuilder builder = new MongoUrlBuilder(connectionString);
                 builder.RetryWrites = false;
                 return new MongoClient(builder.ToMongoUrl());
             });
        }
    }
}
