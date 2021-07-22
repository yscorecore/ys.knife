using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace YS.Knife.Data.Query
{
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    [TypeConverter(typeof(SelectInfoTypeConverter))]
    public class SelectInfo
    {
        public List<SelectItem> Items { get; set; }

        public override string ToString()
        {
            if (Items == null) return string.Empty;
            return string.Join(',', Items.Where(p => p != null).Select(p => p.ToString()));
        }

        public static SelectInfo Parse(string text)
        {
            return Parse(text, CultureInfo.CurrentCulture);
        }
        public static SelectInfo Parse(string text, CultureInfo culture)
        {
            return new QueryExpressionParser(culture).ParseSelectInfo(text);
        }
    }
    public class SelectInfoTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (value is string selectText) ? OrderInfo.Parse(selectText) : base.ConvertFrom(context, culture, value);
        }
    }
}
