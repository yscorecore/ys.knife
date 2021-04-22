namespace YS.Knife.Data.FilterExpressions.Converters
{
    [FilterConverter(FilterType.StartsWith)]
    internal class StartWithExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.StartsWith);
    }
}
