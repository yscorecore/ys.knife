namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(FilterType.Contains)]
    internal class ContainsExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.Contains);
    }
}
