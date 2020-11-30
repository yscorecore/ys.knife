using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace YS.Knife.Mongo
{
    public interface IMongoClientFactory
    {
        IMongoClient Create(string name);
    }
}
