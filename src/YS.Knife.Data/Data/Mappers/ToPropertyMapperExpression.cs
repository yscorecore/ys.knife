using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public class ToPropertyMapperExpression : IMapperExpression
    {
        private readonly LambdaExpression sourceExpression;

        public ToPropertyMapperExpression(LambdaExpression sourceExpression,  Type sourceValueType)
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            this.sourceExpression = sourceExpression;
            SourceValueType = sourceValueType;
        }


        public Type SourceValueType { get; }

        public LambdaExpression GetLambdaExpression()
        {
            return this.sourceExpression;
        }
    }

}
