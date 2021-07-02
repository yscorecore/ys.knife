using System;

namespace YS.Knife.Data.FilterExpressions
{
    internal class FilterConverterAttribute : Attribute
    {
        public Operator FilterType { get; }
        public FilterConverterAttribute(Operator filterType)
        {
            FilterType = filterType;
        }
    }
}
