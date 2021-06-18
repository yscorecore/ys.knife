using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    interface IMapperExpression
    {
        bool IsCollection { get; }
        Type SourceValueType { get; }
        Type TargetValueType { get; }
        LambdaExpression GetBindExpression();
        IObjectMapper SubMapper { get; }
        
        LambdaExpression SourceExpression { get; }
    }
    interface IObjectMapper
    {
        IMapperExpression GetFieldExpression(string targetField,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase);

        LambdaExpression BuildExpression();
        Delegate BuildConvertFunc();
    }
    abstract class MapperExpression<TSourceValue, TTargetValue>:IMapperExpression
    {
        public MapperExpression(LambdaExpression sourceExpression)
        {
            this.SourceExpression = sourceExpression;
        }
        public abstract bool IsCollection { get; }
        public Type SourceValueType => typeof(TSourceValue);
        public Type TargetValueType => typeof(TTargetValue);
        public abstract LambdaExpression GetBindExpression();
        public virtual LambdaExpression SourceExpression { get; }
        public virtual IObjectMapper SubMapper { get; set; }
    }
}
