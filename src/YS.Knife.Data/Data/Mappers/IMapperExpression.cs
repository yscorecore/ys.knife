using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public interface IMapperExpression
    {
        LambdaExpression GetLambdaExpression();
    }
}
