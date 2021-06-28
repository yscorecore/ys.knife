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
        Type SourceType { get; }
        Type TargetType { get; }
        IMapperExpression GetFieldExpression(string targetField,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase);

        LambdaExpression BuildExpression();
        Delegate BuildConvertFunc();
    }

    abstract class MapperExpression : IMapperExpression
    {
        public MapperExpression(LambdaExpression sourceExpression,Type sourceValueType,Type targetValueType)
        {
            this.SourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            this.SourceValueType = sourceValueType ?? throw new ArgumentNullException(nameof(sourceValueType));
            this.TargetValueType = targetValueType ?? throw new ArgumentNullException(nameof(targetValueType));
        }
        public abstract bool IsCollection { get; }
        public Type SourceValueType { get; }
        public Type TargetValueType { get; }
        public abstract LambdaExpression GetBindExpression();
        public LambdaExpression SourceExpression { get; }
        public IObjectMapper SubMapper { get; set; }
    }

    abstract class MapperExpression<TSourceValue, TTargetValue>:MapperExpression
    {
        public MapperExpression(LambdaExpression sourceExpression):base(sourceExpression,typeof(TSourceValue),typeof(TTargetValue))
        {
        }
       
    }
}
