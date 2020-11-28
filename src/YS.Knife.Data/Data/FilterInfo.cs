using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Data
{
    [Serializable]
    public class FilterInfo
    {
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

        public List<FilterInfo> Items { get; set; } = new List<FilterInfo>();

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
                FilterInfo oc = new FilterInfo() { OpType = OpType.OrItems };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }
                return oc;
            }
            else
            {

                FilterInfo oc = new FilterInfo() { OpType = OpType.AndItems };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }
                return oc;
            }
        }
    }




    public enum OpType
    {
        AndItems,
        OrItems,
        SingleItem,
    }


}
