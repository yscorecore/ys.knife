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
                Offset = match.Groups["offset"].Success ? int.Parse(match.Groups["offset"].Value) : 0,
                Limit = int.Parse(match.Groups["limit"].Value)
            };
        }
    }

    public class LimitIntoTypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string str)
            {
                return LimitInfo.Parse(str);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is LimitInfo limit && destinationType == typeof(string))
            {
                return limit.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}