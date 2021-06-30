using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace YS.Knife.Data
{
    [TypeConverter(typeof(FilterInfoTypeConverter))]
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public class FilterInfo
    {
        internal const string Operator_And = "and";
        internal const string Operator_Or = "or";

        internal static readonly Dictionary<FilterType, string> FilterTypeNameMapper =
            new Dictionary<FilterType, string>
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
            return new FilterInfo()
            {
                OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Value = value
            };
        }

        public static FilterInfo CreateItem(string fieldName, FilterType filterType, params FilterInfo[] items)
        {
            return new FilterInfo()
            {
                OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Items = items.ToList()
            };
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
                    this.Items.AddRange(other.Items ?? Enumerable.Empty<FilterInfo>());
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
                return new FilterInfo() {OpType = OpType.AndItems, Items = new List<FilterInfo>() {this, other}};
            }
        }

        public FilterInfo AndAlso(string fieldName, FilterType filterType, object value)
        {
            return AndAlso(new FilterInfo()
            {
                OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Value = value
            });
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
                return new FilterInfo() {OpType = OpType.OrItems, Items = new List<FilterInfo>() {this, other}};
            }
        }

        public FilterInfo OrElse(string fieldName, FilterType filterType, object value)
        {
            return OrElse(new FilterInfo()
            {
                OpType = OpType.SingleItem, FieldName = fieldName, FilterType = filterType, Value = value
            });
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
                FilterInfo oc = new FilterInfo() {OpType = OpType.OrItems, Items = new List<FilterInfo>()};
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }

                return oc;
            }
            else
            {
                FilterInfo oc = new FilterInfo() {OpType = OpType.AndItems, Items = new List<FilterInfo>()};
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
                    return string.Join($" {Operator_And} ",
                        filterInfo.Items.TrimNotNull().Select(p => $"({ToStringInternal(p)})"));
                case OpType.OrItems:
                    return string.Join($" {Operator_Or} ", filterInfo.Items.Select(p => $"({ToStringInternal(p)})"));
                case OpType.SingleItem:
                default:
                    return
                        $"{FileNameToString(filterInfo)} {FilterTypeToString(filterInfo.FilterType)} {ValueToString(filterInfo.Value)}";
            }

            string FileNameToString(FilterInfo filter)
            {
                if (filter.Function != null)
                {
                    var function = filter.Function;
                    var functionBody = new StringBuilder();
                    if (function.Args != null && function.Args.Any())
                    {
                        functionBody.Append(string.Join(", ", function.Args.Select(p => ValueToString(p))));
                    }

                    if (function.FieldNames != null && function.FieldNames.Any())
                    {
                        if (functionBody.Length > 0)
                        {
                            functionBody.Append(", ");
                        }

                        functionBody.Append(string.Join(", ",
                            function.FieldNames.Where(p => !string.IsNullOrWhiteSpace(p))));
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

            string ValueToString(object value, bool convertCollection = true)
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
                else if (value is int || value is short || value is long || value is float || value is double ||
                         value is decimal
                         || value is uint || value is ushort || value is ulong || value is sbyte || value is byte)
                {
                    return value.ToString();
                }
                else if (convertCollection && value is IEnumerable items)
                {
                    var body = string.Join(',', items.OfType<object>().Select(p => ValueToString(p, false)));
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
                return $"\"{Regex.Escape(str).Replace("\"", "\\\"")}\"";
            }
        }

        public static FilterInfo Parse(string filterExpression) => Parse(filterExpression, CultureInfo.CurrentCulture);

        public static FilterInfo Parse(string filterExpression, CultureInfo cultureInfo)
        {
            return new FilterInfoParser(cultureInfo).Parse(filterExpression);
        }
    }
    public class FilterInfo2
    {
        internal const string Operator_And = "and";
        internal const string Operator_Or = "or";
        internal static readonly Dictionary<FilterType, string> FilterTypeNameMapper =
            new Dictionary<FilterType, string>
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

        public ValueInfo Left { get; set; }
        public ValueInfo Right { get; set; }
        public FilterType FilterType { get; set; }
        public OpType OpType { get; set; }
        public List<FilterInfo2> Items { get; set; }
        
        public FilterInfo2 AndAlso(FilterInfo2 other)
        {
            if (other == null)
            {
                return this;
            }

            if (this.OpType == OpType.AndItems)
            {
                if (other.OpType == OpType.AndItems)
                {
                    this.Items.AddRange(other.Items ?? Enumerable.Empty<FilterInfo2>());
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
                return new FilterInfo2() {OpType = OpType.AndItems, Items = new List<FilterInfo2>() {this, other}};
            }
        }
        public FilterInfo2 OrElse(FilterInfo2 other)
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
                return new FilterInfo2() {OpType = OpType.OrItems, Items = new List<FilterInfo2>() {this, other}};
            }
        }
        public override string ToString()
        {
            switch (OpType)
            {
                case OpType.AndItems:
                    return string.Join($" {Operator_And} ",
                        Items.TrimNotNull().Select(p => $"({p})"));
                case OpType.OrItems:
                    return string.Join($" {Operator_Or} ", Items.TrimNotNull().Select(p => $"({p})"));
                default:
                    return
                        $"{ValueToString(Left)} {FilterTypeToString(FilterType)} {ValueToString(Right)}";
            }
            string ValueToString(ValueInfo p0)
            {
                return p0?.ToString() ?? "null";
            }
            string FilterTypeToString(FilterType filterType)
            {
                return FilterTypeNameMapper[filterType];
            }
        }


    }

    public class ValueInfo
    {
        public object Value { get; set; }
        public bool IsValue { get; set; }
        public List<NameInfo> Segments { get; set; }
        public override string ToString()
        {
            if (IsValue)
            {
                return ValueToString(Value);
            }
            else
            {
                var names = (Segments ?? Enumerable.Empty<NameInfo>()).Where(p => p != null).Select(p => p.ToString());
                return string.Join(".", names);
            }

            return base.ToString();
            
            string ValueToString(object value, bool convertCollection = true)
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
                else if (value is int || value is short || value is long || value is float || value is double ||
                         value is decimal
                         || value is uint || value is ushort || value is ulong || value is sbyte || value is byte)
                {
                    return value.ToString();
                }
                else if (convertCollection && value is IEnumerable items)
                {
                    var body = string.Join(',', items.OfType<object>().Select(p => ValueToString(p, false)));
                    return string.Format($"[{body}]");
                }
                else
                {
                    return Repr(value.ToString());
                }
            }
            string Repr(string str)
            {
                return $"\"{Regex.Escape(str).Replace("\"", "\\\"")}\"";
            }
        }
     
    }

   

    public class NameInfo
    {
        public string Name { get; set; }
        public bool IsFunction { get; set; }
        
        public FieldRequiredKind RequiredKind { get; set; }
        public List<ValueInfo> FunctionArgs { get; set; }
        public FilterInfo FunctionFilter { get; set; }
        public override string ToString()
        {
            if (IsFunction)
            {
                List<string> args = new List<string>();
                if (FunctionArgs != null)
                {
                    args.AddRange(FunctionArgs.Where(p => p != null).Select(p => p.ToString()));
                }
                if (FunctionFilter != null)
                {
                    args.Add(FunctionFilter.ToString());
                }

                return $"{Name}({string.Join(", ",args)}){RequiredKindToString(RequiredKind)}";
            }
            else
            {
                return $"{Name}{RequiredKindToString(RequiredKind)}";
            }

            string RequiredKindToString(FieldRequiredKind kind)
            {
                switch (kind)
                {
                    case  FieldRequiredKind.Must: return "!";
                    case FieldRequiredKind.Optional: return "?";
                    default: return string.Empty;
                }
            }
        }
    }
    public enum FieldRequiredKind
    {
        None,
        Must,
        Optional,
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
