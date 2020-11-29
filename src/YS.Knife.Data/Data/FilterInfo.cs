using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace YS.Knife.Data
{
    [TypeConverter(typeof(FilterInfoTypeConverter))]
    [Serializable]
    public class FilterInfo
    {
        static Dictionary<FilterType, string> FilterTypeNameMapper = new Dictionary<FilterType, string>
        {
            [Data.FilterType.Equals] = "==",
            [Data.FilterType.NotEquals] = "!=",
            [Data.FilterType.GreaterThan] = ">",
            [Data.FilterType.LessThanOrEqual] = "<=",
            [Data.FilterType.LessThan] = "<",
            [Data.FilterType.GreaterThanOrEqual] = ">",
            [Data.FilterType.Between] = "between",
            [Data.FilterType.NotBetween] = "not between",
            [Data.FilterType.In] = "in",
            [Data.FilterType.NotIn] = "not in",
            [Data.FilterType.StartsWith] = "starts",
            [Data.FilterType.NotStartsWith] = "not starts",
            [Data.FilterType.EndsWith] = "ends",
            [Data.FilterType.NotEndsWith] = "not ends",
            [Data.FilterType.Contains] = "contains",
            [Data.FilterType.NotContains] = "not contains",
        };

        internal static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,

        };
        static FilterInfo()
        {
            JsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }
        public FilterInfo()
        {
        }
        public FilterInfo(string fieldName, FilterType filterType, object value) : this()
        {
            this.OpType = OpType.SingleItem;
            this.FieldName = fieldName;
            this.FilterType = filterType;
            this.Value = value;
        }
        public FilterInfo(IEnumerable<FilterInfo> items, OpType opType = OpType.AndItems) : this()
        {
            this.OpType = opType;
            this.Items = items.ToList();
        }
        public OpType OpType { get; set; }

        public string FieldName { get; set; }

        public FilterType FilterType { get; set; }

        public object Value { get; set; }

        public List<FilterInfo> Items { get; set; }

        public static FilterInfo CreateItem(string fieldName, FilterType filterType, object value)
        {
            return new FilterInfo() { OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Value = value };
        }
        public static FilterInfo CreateOr(params FilterInfo[] items)
        {
            return new FilterInfo(items, OpType.OrItems);
        }
        public static FilterInfo CreateAnd(params FilterInfo[] items)
        {
            return new FilterInfo(items, OpType.AndItems);
        }

        public FilterInfo AndAlso(FilterInfo other)
        {
            if (other == null)
            {
                return this;
            }
            if (this.OpType == OpType.AndItems)
            {
                if (other.OpType == OpType.AndItems)
                {
                    this.Items.AddRange(other.Items);
                    return this;
                }
                else
                {
                    this.Items.Add(other);
                    return this;
                }
            }
            else
            {
                return new FilterInfo() { OpType = OpType.AndItems, Items = new List<FilterInfo>() { this, other } };
            }
        }
        public FilterInfo AndAlso(string fieldName, FilterType filterType, object value)
        {
            return AndAlso(new FilterInfo() { OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Value = value });
        }
        public FilterInfo OrElse(FilterInfo other)
        {
            if (other == null)
            {
                return this;
            }
            if (this.OpType == OpType.OrItems)
            {
                if (other.OpType == OpType.OrItems)
                {
                    this.Items.AddRange(other.Items);
                }
                else
                {
                    this.Items.Add(other);
                }
                return this;
            }
            else
            {
                return new FilterInfo() { OpType = OpType.OrItems, Items = new List<FilterInfo>() { this, other } };
            }
        }
        public FilterInfo OrElse(string fieldName, FilterType filterType, object value)
        {
            return OrElse(new FilterInfo() { OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Value = value });
        }
        public FilterInfo Not()
        {
            if (this.OpType == OpType.SingleItem)
            {
                this.FilterType = ~(this).FilterType;
                return this;
            }
            else if (this.OpType == OpType.AndItems)
            {
                // AndCondition current = this as AndCondition;
                FilterInfo oc = new FilterInfo() { OpType = OpType.OrItems, Items = new List<FilterInfo>() };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }
                return oc;
            }
            else
            {

                FilterInfo oc = new FilterInfo() { OpType = OpType.AndItems, Items = new List<FilterInfo>() };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }
                return oc;
            }
        }

        public override string ToString()
        {
            return ToStringInternal(this);
        }

        private string ToStringInternal(FilterInfo filterInfo)
        {
            switch (filterInfo.OpType)
            {
                case OpType.AndItems:
                    return string.Join(" and ", filterInfo.Items.Select(p => $"({ToStringInternal(p)})"));
                case OpType.OrItems:
                    return string.Join(" or ", filterInfo.Items.Select(p => $"({ToStringInternal(p)})"));
                case OpType.SingleItem:
                default:
                    return $"{filterInfo.FieldName} {FilterTypeToString(filterInfo.FilterType)} {ValueToString(filterInfo.Value)}";
            }
        }
        private string FilterTypeToString(FilterType? filterType)
        {
            if (filterType.HasValue && FilterTypeNameMapper.TryGetValue(filterType.Value, out var res))
            {
                return res;
            }
            throw new InvalidOperationException();
        }
        private string ValueToString(object value)
        {
            return JsonSerializer.Serialize(value);
        }
    }


    public class FilterInfoTypeConverter : TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string base64String)
            {
                var bytes = Convert.FromBase64String(base64String);
                ReadOnlySpan<byte> byteSpan = new ReadOnlySpan<byte>(bytes);
                return JsonSerializer.Deserialize(byteSpan, typeof(FilterInfo), FilterInfo.JsonOptions);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is FilterInfo)
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, FilterInfo.JsonOptions);
                return Convert.ToBase64String(bytes);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

    }

    public enum OpType
    {
        AndItems,
        OrItems,
        SingleItem,
    }


}
