using MongoDB.Driver;

namespace YS.Knife.Mongo
{
    public interface IMongoClientFactory
    {
        IMongoClient Create(string connectionStringKey);
    }
}
