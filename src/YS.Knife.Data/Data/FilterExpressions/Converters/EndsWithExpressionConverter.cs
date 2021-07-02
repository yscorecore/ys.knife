namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(Operator.EndsWith)]
    internal class EndsWithExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.EndsWith);
    }
}
