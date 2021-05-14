using System;
using System.Linq;

namespace YS.Knife.Data.Mapper
{
    public static class Mapper
    {
        private static LocalCache<string, ObjectMapper> basicMapperCache = new LocalCache<string, ObjectMapper>();
        
        
        public static ObjectMapper CreateBasic<TFrom, TTo>()
        {
            return CreateBasic(typeof(TFrom), typeof(TTo));
        }

        
        public static ObjectMapper CreateBasic(Type fromType,Type toType)
        {
            Should.BeTrue(Type.GetTypeCode(fromType) == TypeCode.Object, () => new ArgumentException("From type should be an object type."));
            Should.BeTrue(Type.GetTypeCode(toType) == TypeCode.Object, () => new ArgumentException("To type should be an object type."));
            string key = $"{fromType.AssemblyQualifiedName}_{toType.AssemblyQualifiedName}";
            
            ObjectMapper objectMapper = new ObjectMapper(fromType, toType);
            var fromTypeMaps = fromType.GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);
            var toTypeMaps = toType.GetProperties().Where(p=>p.CanWrite).ToDictionary(p => p.Name, p => p);
        
            
            foreach (var fromProp in fromType.GetProperties())
            {
                if (toTypeMaps.TryGetValue(fromProp.Name, out var toProp))
                {

                    if (toProp.PropertyType != fromProp.PropertyType)
                    {
                        continue;
                    }
                    // add basicPropertyMapper
                   
                }
            }
            return objectMapper;
        }

    }
}
