namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(FilterType.EndsWith)]
    internal class EndsWithExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.EndsWith);
    }
}
