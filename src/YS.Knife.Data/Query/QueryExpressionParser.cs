using System;
using System.Collections.Generic;
using System.Globalization;

namespace YS.Knife.Data.Query
{
    internal partial class QueryExpressionParser
    {
        public CultureInfo CurrentCulture { get; }
        public QueryExpressionParser(CultureInfo cultureInfo)
        {
            this.CurrentCulture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        }
        public FilterInfo ParseFilter(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            var context = new ParseContext(text, this.CurrentCulture);
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
            var context = new ParseContext(text, this.CurrentCulture);
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
            var context = new ParseContext(text, this.CurrentCulture);
            context.SkipWhiteSpace();
            var paths = context.ParsePropertyPaths();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return paths;
        }
        public SelectInfo ParseSelectInfo(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text, this.CurrentCulture);
            SelectInfo selectInfo = context.ParseSelectInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return selectInfo;
        }
        public LimitInfo ParseLimitInfo(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text, this.CurrentCulture);
            LimitInfo limitInfo = context.ParseLimitInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return limitInfo;
        }
    }

}
