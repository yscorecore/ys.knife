using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public class PropertyMapperExpression : IMapperExpression
    {
        private readonly LambdaExpression sourceExpression;

        public PropertyMapperExpression(LambdaExpression sourceExpression)
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            this.sourceExpression = sourceExpression;
        }
        public LambdaExpression GetLambdaExpression()
        {
            return this.sourceExpression;
        }
    }
}
