using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public interface IMapperExpression
    {
        public Type SourceValueType { get; }
        LambdaExpression GetLambdaExpression();
    }
}
