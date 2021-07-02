namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(Operator.StartsWith)]
    internal class StartsWithExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.StartsWith);
    }
}
