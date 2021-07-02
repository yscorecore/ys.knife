namespace YS.Knife.Data.FilterExpressions.Converters
{

    [FilterConverter(Operator.Contains)]
    internal class ContainsExpressionConverter : StringExpressionConverter
    {
        protected override string MethodName => nameof(string.Contains);
    }
}
