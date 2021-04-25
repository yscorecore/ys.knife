using System;
using System.Linq;

namespace YS.Knife.Data.Translaters
{
    public class FilterTranslater
    {
        public FilterInfo Translate<TFrom, TTo>(FilterInfo filterInfo)
        {
            return Translate(typeof(TFrom), typeof(TTo), filterInfo);
        }
        public FilterInfo Translate(Type from, Type to, FilterInfo filterInfo)
        {
            return filterInfo;
        }
    }
}
