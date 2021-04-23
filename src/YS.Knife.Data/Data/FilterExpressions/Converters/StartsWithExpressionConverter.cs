namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(FilterType.StartsWith)]
    internal class StartsWithExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.StartsWith);
    }
}
