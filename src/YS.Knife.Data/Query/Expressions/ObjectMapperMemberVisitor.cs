using System;
using System.Linq.Expressions;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.Query.Expressions
{
    class ObjectMapperMemberVisitor : IMemberVisitor
    {
        private readonly IObjectMapper _objectMapper;

        public ObjectMapperMemberVisitor(IObjectMapper objectMapper)
        {
            this._objectMapper = objectMapper;
        }
        public Type CurrentType => _objectMapper.SourceType;

        public IFilterMemberInfo GetSubMemberInfo(string memberName)
        {
            var fieldExpression = _objectMapper.GetFieldExpression(memberName, StringComparison.InvariantCultureIgnoreCase);
            if (fieldExpression != null)
            {
                return new MapperMemberInfo(fieldExpression);
            }
            return default;

        }
        class MapperMemberInfo : IFilterMemberInfo
        {
            private readonly IMapperExpression mapperExpression;

            public MapperMemberInfo(IMapperExpression mapperExpression)
            {
                this.mapperExpression = mapperExpression;
            }

            public Type ExpressionValueType => mapperExpression.SourceValueType;

            public LambdaExpression SelectExpression => mapperExpression.SourceExpression;

            public IMemberVisitor SubProvider
            {
                get
                {
                    if (mapperExpression.SubMapper != null)
                    {
                        return new ObjectMapperMemberVisitor(mapperExpression.SubMapper);
                    }
                    else
                    {
                        return IMemberVisitor.GetObjectVisitor(ExpressionValueType);
                    }

                }
            }
        }
    }
}
