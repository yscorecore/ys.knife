using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Mongo
{
    internal interface IMongoContextConsumer
    {
        MongoContext MongoContext { get; }
    }
}
