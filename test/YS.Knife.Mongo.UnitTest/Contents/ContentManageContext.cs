using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace YS.Knife.Mongo.UnitTest.Contents
{
    [MongoContext("cms")]
    public class ContentManageContext : MongoContext
    {
        public ContentManageContext(IMongoDatabase mongoDataBase) : base(mongoDataBase)
        {

        }

        public IMongoCollection<Topic> Topic { get; set; }
    }
    [MongoCollectionName("topics")]
    public class Topic
    {
        [BsonId()]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        public string Summary { get; set; }

        public DateTimeOffset CreateTime { get; set; }

        public TopicStatistic Statistics { get; set; }
    }

    public class TopicStatistic
    {

        public int Liked { get; set; }
        public int UnLiked { get; set; }
        public int Viewed { get; set; }



    }
}
