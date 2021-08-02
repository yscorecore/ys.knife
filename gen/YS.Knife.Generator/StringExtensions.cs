namespace YS.Knife
{
    internal static class StringExtensions
    {
        public static string ToCamelCase(this string name)
        {
            name = name.TrimStart('_');
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
        }
    }
}
