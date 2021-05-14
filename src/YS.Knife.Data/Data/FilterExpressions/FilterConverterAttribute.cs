using System;

namespace YS.Knife.Data.FilterExpressions
{
    internal class FilterConverterAttribute : Attribute
    {
        public FilterType FilterType { get; }
        public FilterConverterAttribute(FilterType filterType)
        {
            FilterType = filterType;
        }
    }
}
