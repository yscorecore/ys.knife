using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YS.Knife.Data.Expressions.Functions;

namespace YS.Knife.Data
{
    internal partial class FilterInfoParser2
    {
        public CultureInfo CurrentCulture { get; }
        public FilterInfoParser2(CultureInfo cultureInfo)
        {
            this.CurrentCulture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        }
        public FilterInfo2 ParseFilter(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            var context = new ParseContext(text,this.CurrentCulture);
            var filterInfo = context.ParseFilterInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return filterInfo;
        }
        public OrderInfo ParseOrder(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text,this.CurrentCulture);
            OrderInfo orderInfo = context.ParseOrderInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return orderInfo;
        }
        public List<ValuePath> ParsePropertyPaths(string text)
        {
            var context = new ParseContext(text,this.CurrentCulture);
            context.SkipWhiteSpace();
            var paths = context.ParsePropertyPaths();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return paths;
        }


    }

}
