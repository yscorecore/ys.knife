using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YS.Knife.Data
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

        public static LimitInfo Parse(string limitStr)
        {
            _ = limitStr ?? throw new ArgumentNullException(nameof(limitStr));
            var match = Regex.Match(limitStr, @"^(\s*(?<offset>\d+)\s*,)?\s*(?<limit>\d+)\s*$");
            if (!match.Success)
            {
                throw new FormatException($"Invalid format for {nameof(LimitInfo)}.");
            }
            return new LimitInfo
            {
                Offset = match.Groups["offset"].Success ? Convert.ToInt32(match.Groups["offset"].Value, CultureInfo.InvariantCulture) : 0,
                Limit = Convert.ToInt32(match.Groups["limit"].Value, CultureInfo.InvariantCulture)
            };
        }

        public static LimitInfo FromPageInfo(int pageIndex, int pageSize)
        {
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
