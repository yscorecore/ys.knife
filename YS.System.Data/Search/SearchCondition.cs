using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    [Serializable]
    public class SearchCondition
    {
        public SearchCondition()
        {
            this.Items = new List<SearchCondition>();
        }
        public SearchCondition(string fieldName, SearchType searchType, object value):this()
        {
            this.OpType = Data.OpType.SingleItem;
            this.FieldName = fieldName;
            this.SearchType = searchType;
            this.Value = value;
        }
        public SearchCondition(IEnumerable<SearchCondition> items, OpType opType = Data.OpType.AndItems):this()
        {
            this.OpType = opType;
            this.Items.AddRange(items);
        }

        public OpType OpType { get; set; }

        public string FieldName { get; set; }

        public SearchType SearchType { get; set; }

        public object Value { get; set; }

        //  readonly List<SearchCondition> items = new List<SearchCondition>();

        public List<SearchCondition> Items
        {
            get;
            set;
        }

        public static SearchCondition CreateItem(string fieldName, SearchType searchType, object value)
        {
            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = fieldName, SearchType = searchType, Value = value };
        }
        public static SearchCondition CreateOr(params SearchCondition[] items)
        {
            return new SearchCondition(items, OpType.OrItems);
        }
        public static SearchCondition CreateAnd(params SearchCondition[] items)
        {
            return new SearchCondition(items, OpType.AndItems);
        }

        public SearchCondition AndAlso(SearchCondition s)
        {
            if (this.OpType == Data.OpType.AndItems)
            {
                if (s.OpType == Data.OpType.AndItems)
                {
                    this.Items.AddRange(s.Items);
                    return this;
                }
                else
                {
                    this.Items.Add(s);
                    return this;
                }
            }
            else
            {
                return new SearchCondition() { OpType = Data.OpType.AndItems, Items = new List<SearchCondition>() { this, s } };
            }
        }
        public SearchCondition AndAlso(string fieldName, SearchType searchType, object value)
        {
            return AndAlso(new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = fieldName, SearchType = searchType, Value = value });
        }
        public SearchCondition OrElse(SearchCondition s)
        {
            if (this.OpType == Data.OpType.OrItems)
            {
                if (s.OpType == Data.OpType.OrItems)
                {
                    this.Items.AddRange(s.Items);
                }
                else
                {
                    this.Items.Add(s);
                }
                return this;
            }
            else
            {
                return new SearchCondition() { OpType = Data.OpType.OrItems, Items = new List<SearchCondition>() { this, s } };
            }
        }
        public SearchCondition OrElse(string fieldName, SearchType searchType, object value)
        {
            return OrElse(new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = fieldName, SearchType = searchType, Value = value });
        }
        public SearchCondition Not()
        {
            if (this.OpType == Data.OpType.SingleItem)
            {
                this.SearchType = ~(this).SearchType;
                return this;
            }
            else if (this.OpType == Data.OpType.AndItems)
            {
                // AndCondition current = this as AndCondition;
                SearchCondition oc = new SearchCondition() { OpType = OpType.OrItems };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }
                return oc;
            }
            else
            {

                SearchCondition oc = new SearchCondition() { OpType = OpType.AndItems };
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


    //[Serializable]
    //public sealed class AndCondition : SearchCondition
    //{
    //    public AndCondition()
    //    {

    //    }
    //    public AndCondition(IEnumerable<SearchCondition> conditions)
    //    {
    //        this.items.AddRange(conditions);
    //    }
    //    readonly List<SearchCondition> items = new List<SearchCondition>();

    //    public List<SearchCondition> Items
    //    {
    //        get { return items; }
    //    }
    //}
    //[Serializable]
    //public sealed class OrCondition : SearchCondition
    //{
    //    public OrCondition()
    //    {

    //    }
    //    public OrCondition(IEnumerable<SearchCondition> conditions)
    //    {
    //        this.items.AddRange(conditions);
    //    }
    //    readonly List<SearchCondition> items = new List<SearchCondition>();

    //    public List<SearchCondition> Items
    //    {
    //        get { return items; }
    //    }
    //}
}
