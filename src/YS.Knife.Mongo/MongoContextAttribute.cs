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
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MongoContextAttribute : YS.Knife.KnifeAttribute
    {
        public MongoContextAttribute() : base(null)
        {

        }
        public string DataBaseName { get; set; }

        public string ConnectionStringKey { get; set; }

        public override void RegisterService(IServiceCollection services, IRegisteContext context, Type declareType)
        {

            string connectionStringKey = string.IsNullOrEmpty(this.ConnectionStringKey)
                ? declareType.Name
                : this.ConnectionStringKey;
            string connectionString = context.Configuration.GetConnectionString(connectionStringKey);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException($"Can not find connection string by key \"{connectionStringKey}\".");
            }
            //services.AddSingleton<IMongoContext>(s => new MongoClient(connectionString));

            throw new NotImplementedException();
        }
       
    }
}