using System;
using System.Collections.Concurrent;
using YS.Knife.Data.Query;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.Query.Expressions
{
    public interface IMemberVisitor
    {
        static readonly ConcurrentDictionary<Type, IMemberVisitor> ObjectMemberProviderCache =
            new ConcurrentDictionary<Type, IMemberVisitor>();
        public Type CurrentType { get; }
        public IFilterMemberInfo GetSubMemberInfo(string memberName);

        public static IMemberVisitor GetObjectVisitor(Type type)
        {
            return ObjectMemberProviderCache.GetOrAdd(type, (ty) =>
            {
                var objectProviderType = typeof(ObjectMemberVisitor<>).MakeGenericType(ty);
                return Activator.CreateInstance(objectProviderType) as IMemberVisitor;
            });
        }

        public static IMemberVisitor GetMapperVisitor(IObjectMapper objectMapper)
        {
            return new ObjectMapperMemberVisitor(objectMapper);
        }
    }
}
