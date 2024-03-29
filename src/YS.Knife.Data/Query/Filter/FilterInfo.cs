﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query
{
    [TypeConverter(typeof(FilterInfoTypeConverter))]
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public class FilterInfo
    {
        internal const string Operator_And = "and";
        internal const string Operator_Or = "or";
        private static readonly Dictionary<Operator, string> OperatorTypeCodeDictionary =
            typeof(Operator).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(
                p => (Operator)p.GetValue(null),
                p => p.GetCustomAttributes<OperatorCodeAttribute>().Select(p => p.Code).First());

        public ValueInfo Left { get; set; }
        public ValueInfo Right { get; set; }
        public Operator Operator { get; set; }
        public CombinSymbol OpType { get; set; }
        public List<FilterInfo> Items { get; set; }

        public FilterInfo AndAlso(FilterInfo other)
        {
            if (other == null)
            {
                return this;
            }

            if (OpType == CombinSymbol.AndItems)
            {
                if (other.OpType == CombinSymbol.AndItems)
                {
                    Items.AddRange(other.Items ?? Enumerable.Empty<FilterInfo>());
                    return this;
                }
                else
                {
                    Items.Add(other);
                    return this;
                }
            }
            else
            {
                return new FilterInfo() { OpType = CombinSymbol.AndItems, Items = new List<FilterInfo>() { this, other } };
            }
        }
        public FilterInfo AndAlso(string fieldPaths, Operator filterOperator, object value)
        {
            return this.AndAlso(FilterInfo.CreateItem(fieldPaths, filterOperator, value));
        }
        public FilterInfo OrElse(FilterInfo other)
        {
            if (other == null)
            {
                return this;
            }

            if (OpType == CombinSymbol.OrItems)
            {
                if (other.OpType == CombinSymbol.OrItems)
                {
                    Items.AddRange(other.Items);
                }
                else
                {
                    Items.Add(other);
                }

                return this;
            }
            else
            {
                return new FilterInfo() { OpType = CombinSymbol.OrItems, Items = new List<FilterInfo>() { this, other } };
            }
        }

        public FilterInfo OrElse(string fieldPaths, Operator filterOperator, object value)
        {
            return this.OrElse(FilterInfo.CreateItem(fieldPaths, filterOperator, value));
        }
        public override string ToString()
        {
            switch (OpType)
            {
                case CombinSymbol.AndItems:
                    return string.Join($" {Operator_And} ",
                        Items.TrimNotNull().Select(p => $"({p})"));
                case CombinSymbol.OrItems:
                    return string.Join($" {Operator_Or} ", Items.TrimNotNull().Select(p => $"({p})"));
                default:
                    return
                        $"{ValueToString(Left)} {FilterTypeToString(Operator)} {ValueToString(Right)}";
            }
            string ValueToString(ValueInfo p0)
            {
                return p0?.ToString() ?? "null";
            }
            string FilterTypeToString(Operator filterType)
            {
                return OperatorTypeCodeDictionary[filterType];
            }
        }

        public static FilterInfo Parse(string filterExpression) => Parse(filterExpression, CultureInfo.CurrentCulture);

        public static FilterInfo Parse(string filterExpression, CultureInfo cultureInfo)
        {
            return new QueryExpressionParser(cultureInfo).ParseFilter(filterExpression);
        }
        public static FilterInfo CreateItem(string fieldPaths, Operator filterType, object value)
        {
            return new FilterInfo()
            {
                OpType = CombinSymbol.SingleItem,
                Operator = filterType,
                Left = ValueInfo.Parse(fieldPaths),
                Right = new ValueInfo { IsConstant = true, ConstantValue = value }
            };
        }

        public static FilterInfo CreateOr(params FilterInfo[] items)
        {
            return new FilterInfo { Items = items.TrimNotNull().ToList(), OpType = CombinSymbol.OrItems };
        }

        public static FilterInfo CreateAnd(params FilterInfo[] items)
        {
            return new FilterInfo { Items = items.TrimNotNull().ToList(), OpType = CombinSymbol.AndItems };
        }


        public FilterInfo Not()
        {
            if (this.OpType == CombinSymbol.SingleItem)
            {
                this.Operator = ~(this).Operator;
                return this;
            }
            else if (this.OpType == CombinSymbol.AndItems)
            {
                // AndCondition current = this as AndCondition;
                FilterInfo oc = new FilterInfo() { OpType = CombinSymbol.OrItems, Items = new List<FilterInfo>() };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }

                return oc;
            }
            else
            {
                FilterInfo oc = new FilterInfo() { OpType = CombinSymbol.AndItems, Items = new List<FilterInfo>() };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }

                return oc;
            }
        }

    }

    public class FilterInfoTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string ? FilterInfo.Parse(value as string, culture) : base.ConvertFrom(context, culture, value);
        }
    }
}

namespace YS.Knife.Data
{
    [DebuggerDisplay("{ToString()}")]
    public class ValueInfo
    {
        public object ConstantValue { get; set; }
        public bool IsConstant { get; set; }
        public List<ValuePath> NavigatePaths { get; set; }
        public override string ToString()
        {
            if (IsConstant)
            {
                return ValueToString(ConstantValue);
            }
            else
            {
                var names = (NavigatePaths ?? Enumerable.Empty<ValuePath>()).Where(p => p != null).Select(p => p.ToString());
                return string.Join(".", names);
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
            string Repr(string str)
            {
                return $"\"{Regex.Escape(str).Replace("\"", "\\\"")}\"";
            }
        }

        public static ValueInfo Parse(string valueExpression) => Parse(valueExpression, CultureInfo.CurrentCulture);

        public static ValueInfo Parse(string valueExpression, CultureInfo cultureInfo)
        {
            return new QueryExpressionParser(cultureInfo).ParseValue(valueExpression);
        }

        public static ValueInfo FromConstantValue(object value)
        {
            return new ValueInfo
            {
                IsConstant = true,
                ConstantValue = value
            };
        }
        public static ValueInfo FromPaths(List<ValuePath> navigatePaths)
        {
            return new ValueInfo
            {
                IsConstant = false,
                NavigatePaths = navigatePaths ?? new List<ValuePath>()
            };
        }
    }



    public class ValuePath
    {
        public string Name { get; set; }
        public bool IsFunction { get; set; }
        public object[] FunctionArgs { get; set; }
        public override string ToString()
        {
            if (IsFunction)
            {
                var args = new List<string>();
                if (FunctionArgs != null)
                {
                    args.AddRange(FunctionArgs.Where(p => p != null).Select(p => p?.ToString()));
                }
                return $"{Name}({string.Join(", ", args)})";
            }
            else
            {
                return $"{Name}";
            }


        }
    }




}
