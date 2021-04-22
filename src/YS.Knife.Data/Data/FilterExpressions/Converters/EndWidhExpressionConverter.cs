namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(FilterType.EndsWith)]
    internal class EndWidhExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.EndsWith);
    }
}
