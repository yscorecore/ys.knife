using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace YS.Knife.Data
{
    [TypeConverter(typeof(FilterInfoTypeConverter))]
    [Serializable]
    public class FilterInfo
    {
        internal const string Operator_And = "and";
        internal const string Operator_Or = "or";

        internal static readonly Dictionary<FilterType, string> FilterTypeNameMapper = new Dictionary<FilterType, string>
        {
            [FilterType.Equals] = "==",
            [FilterType.NotEquals] = "!=",
            [FilterType.GreaterThan] = ">",
            [FilterType.LessThanOrEqual] = "<=",
            [FilterType.LessThan] = "<",
            [FilterType.GreaterThanOrEqual] = ">=",
            [FilterType.Between] = "bt",
            [FilterType.NotBetween] = "nbt",
            [FilterType.In] = "in",
            [FilterType.NotIn] = "nin",
            [FilterType.StartsWith] = "sw",
            [FilterType.NotStartsWith] = "nsw",
            [FilterType.EndsWith] = "ew",
            [FilterType.NotEndsWith] = "new",
            [FilterType.Contains] = "ct",
            [FilterType.NotContains] = "nct",
        };


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

        public FunctionInfo Function { get; set; }

        public static FilterInfo CreateItem(string fieldName, FilterType filterType, object value)
        {
            return new FilterInfo() { OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Value = value };
        }

        public static FilterInfo CreateItem(string fieldName, FilterType filterType, params FilterInfo[] items)
        {
            return new FilterInfo() { OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Items = items.ToList() };
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
                    return string.Join($" {Operator_And} ", filterInfo.Items.TrimNotNull().Select(p => $"({ToStringInternal(p)})"));
                case OpType.OrItems:
                    return string.Join($" {Operator_Or} ", filterInfo.Items.Select(p => $"({ToStringInternal(p)})"));
                case OpType.SingleItem:
                default:
                    return $"{FileNameToString(filterInfo)} {FilterTypeToString(filterInfo.FilterType)} {ValueToString(filterInfo.Value)}";
            }
            string FileNameToString(FilterInfo filter)
            {
                if (filter.Function != null)
                {
                    var function = filter.Function;
                    var functionBody = new StringBuilder();
                    if (function.Args != null && function.Args.Any())
                    {
                        functionBody.Append(string.Join(", ", function.Args.Select(p=>ValueToString(p))));
                    }

                    if (function.FieldNames != null && function.FieldNames.Any())
                    {
                        if (functionBody.Length > 0)
                        {
                            functionBody.Append(", ");
                        }
                        functionBody.Append(string.Join(", ", function.FieldNames.Where(p => !string.IsNullOrWhiteSpace(p))));
                    }
                    if (function.SubFilter != null)
                    {
                        if (functionBody.Length > 0)
                        {
                            functionBody.Append(", ");
                        }
                        functionBody.Append(ToStringInternal(function.SubFilter));
                    }

                    return $"{filter.FieldName}.{function.Name}({functionBody})";
                }
                else
                {
                    return filter.FieldName;
                }
            }

            string ValueToString(object value,bool convertCollection = true)
            {
                if (value == null || value == DBNull.Value)
                {
                    return "null";
                }
                else if (value is string str)
                {
                    return Repr(str);
                }
                else if (value is bool)
                {
                    return value.ToString().ToLowerInvariant();
                }
                else if (value is int || value is short || value is long || value is float || value is double || value is decimal
                     || value is uint || value is ushort || value is ulong || value is sbyte || value is byte)
                {
                    return value.ToString();
                }
                else if (convertCollection && value is IEnumerable items)
                {
                    var body =string.Join(',', items.OfType<object>().Select(p => ValueToString(p, false)));
                    return string.Format($"[{body}]");
                }
                else
                {
                    return Repr(value.ToString());
                }
            }
            string FilterTypeToString(FilterType filterType)
            {
                return FilterTypeNameMapper[filterType];
            }
            string Repr(string str)
            {
                return $"\"{Regex.Escape(str).Replace("\"","\\\"")}\"";
            }
        }

        public static FilterInfo Parse(string filterExpression) => Parse(filterExpression, CultureInfo.CurrentCulture);
        
        public static FilterInfo Parse(string filterExpression, CultureInfo cultureInfo)
        {
            return new FilterInfoParser(cultureInfo).Parse(filterExpression);
        }
    }


    public class FunctionInfo
    {
        public string Name { get; set; }
        public FilterInfo SubFilter { get; set; }
        public List<string> FieldNames { get; set; }
        public List<object> Args { get; set; }
    }

    public class FilterInfoTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string base64StringOrFilterExpression)
            {
                if (Base64.IsBase64String(base64StringOrFilterExpression))
                {
                    var bytes = Convert.FromBase64String(base64StringOrFilterExpression);
                    return Json.DeSerialize<FilterInfo>(bytes);
                }
                else 
                {
                    return FilterInfo.Parse(base64StringOrFilterExpression, culture);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

    }




}
