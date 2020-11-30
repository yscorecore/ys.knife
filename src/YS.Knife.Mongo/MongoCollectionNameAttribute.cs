using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YS.Knife.Mongo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MongoCollectionNameAttribute : Attribute
    {
        public MongoCollectionNameAttribute(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName) || !Regex.IsMatch(collectionName, @"^\w+$"))
            {
                throw new ArgumentException($"Invalid mongo collection name '{collectionName}'.");
            }
            this.CollectionName = collectionName;
        }
        public string CollectionName { get; set; }

        public static string GetCollectionName(Type entityType)
        {
            _ = entityType ?? throw new ArgumentNullException(nameof(entityType));
            var attr = GetCustomAttribute(entityType, typeof(MongoCollectionNameAttribute)) as MongoCollectionNameAttribute;
#pragma warning disable CA1308 // 将字符串规范化为大写
            return attr != null ? attr.CollectionName : entityType.Name.ToLowerInvariant();
#pragma warning restore CA1308 // 将字符串规范化为大写
        }
    }
}
