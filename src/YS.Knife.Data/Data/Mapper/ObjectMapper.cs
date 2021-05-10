using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mapper
{
    public class ObjectMapper:List<PropMapper>
    {
        public ObjectMapper(Type fromType,Type toType)
        {
            
        }
        internal ObjectMapper(Type fromType,Type toType,string[] basicProps)
        {
            
        }
        public ObjectMapper AddProperty(string fromName, string toName)
        {
            return null;
        }

        // 只过滤指定字段，如果为空，则查询全部
        public ObjectMapper Filter(string[] fields)
        {
            if (fields != null && fields.Length > 0)
            {
                return this;
            }
            return this;
        }
    }

    public class ObjectMapper<TFrom, TTo>
    {
        public Expression<Func<TFrom, TTo>> ToExpression()
        {
            return null;
        }
    }
}
