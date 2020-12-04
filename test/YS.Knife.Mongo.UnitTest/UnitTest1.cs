using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


namespace YS.Knife.Mongo.UnitTest
{
    [TestClass]
    public class UnitTest1
    {


        [TestMethod]
        public void TestMethod2()
        {
            var _database = new MyMongoDatabase("mongodb://localhost:27017");
            _database.Values.InsertOne(new Value { Val = "abcd", Num = 3 });
            var all = _database.Values.AsQueryable().ToList();
            var first = _database.Values.AsQueryable().Where(p => p.Val.EndsWith("d") && p.Num != 3).First();

        }

    }

    public class MyMongoDatabase
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        public MyMongoDatabase(string connectionString)
        {
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase("api");
            Values = _database.GetCollection<Value>("values");
        }

        public IMongoCollection<Value> Values { get; set; }
    }

    public class Value
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Val { get; set; }

        public int Num { get; set; }
    }
}
