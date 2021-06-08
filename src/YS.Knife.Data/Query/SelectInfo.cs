using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace YS.Knife.Data
{
    [Serializable]
    [TypeConverter(typeof(SelectInfoConverter))]
    public class SelectInfo
    {
        public SelectInfo()
        {
        }
        public SelectInfo(params string[] include)
        {
            this.Include = include;
        }
        public string[] Include { get; set; }

        public override string ToString()
        {
            var fields = Include ?? Array.Empty<string>();
            return $"[{string.Join(',', fields)}]";
        }
        public static SelectInfo Parse(string line)
        {
            var fields = (line ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries);
            return new SelectInfo(fields.Select(p=>p.Trim()).ToArray());
        }
    }

    
    public class SelectInfoConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (value is string selectInfo) ? SelectInfo.Parse(selectInfo) : base.ConvertFrom(context, culture, value);
        }
    }
}
