using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YS.Knife.Data;
namespace YS.Knife.Mongo
{
    public abstract class MongoContext
    {
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }

        public IMongoCollection<T> GetCollection<T>()
        {
            return Database.GetCollection<T>(typeof(T).Name);
        }

    }

    [MongoContext()]
    public class BookContext:MongoContext
    {
        public IMongoCollection<User> Users{get; set;}
        public IMongoCollection<Store> Store{get; set;}
    }
    public class User
    {

    }
     public class Store
    {

    }
}