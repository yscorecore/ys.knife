using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using YS.Knife.Data.Expressions;

namespace YS.Knife.Data.Expressions
{
    [Serializable]
    [TypeConverter(typeof(LimitIntoTypeConverter))]
    public class LimitInfo
    {
        public LimitInfo()
        {
        }
        public LimitInfo(int offset, int limit)
        {
            this.Offset = offset;
            this.Limit = limit;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public override string ToString()
        {
            return $"{Offset},{Limit}";
        }
        public static LimitInfo Parse(string limitStr) => Parse(limitStr, CultureInfo.CurrentCulture);
        public static LimitInfo Parse(string limitStr, CultureInfo currentCulture)
        {
            return new FilterInfoParser2(currentCulture).ParseLimitInfo(limitStr);
        }

        public static LimitInfo FromPageInfo(int pageIndex, int pageSize)
        {
            if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));
            return new LimitInfo
            {
                Offset = (pageIndex - 1) * pageSize,
                Limit = pageSize
            };
        }
    }

    public class LimitIntoTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return (value is string limttInfo) ? LimitInfo.Parse(limttInfo) : base.ConvertFrom(context, culture, value);
        }
    }
}
