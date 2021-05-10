using System;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.Mapper
{
    public class PropMapper
    {
        public Type FromType { get; }
        public PropertyInfo FromProperty { get; }
        public string FromName { get;  }
               
        public Type ToType { get; }
        public Expression ToExpression { get; }
        public string ToPath { get; set; }
    }

    public class NestedObjectMapper
    {
    }

    public class CollectionMapper
    {
        
    }
}
