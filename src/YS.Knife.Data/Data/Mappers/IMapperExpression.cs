using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    interface IMapperExpression
    {
        bool IsCollection { get; }
        Type SourceValueType { get; }
        Type TargetValueType { get; }
        LambdaExpression GetLambdaExpression();
        IObjectMapper SubMapper { get; }
    }
    interface IObjectMapper
    {
        IMapperExpression GetFieldExpression(string targetField,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase);

        LambdaExpression BuildExpression();
    }
    abstract class MapperExpression<TSourceValue, TTargetValue>:IMapperExpression
    {
        public abstract bool IsCollection { get; }
        public Type SourceValueType => typeof(TSourceValue);
        public Type TargetValueType => typeof(TTargetValue);
        public abstract LambdaExpression GetLambdaExpression();
        public virtual IObjectMapper SubMapper { get; set; }
    }
}
