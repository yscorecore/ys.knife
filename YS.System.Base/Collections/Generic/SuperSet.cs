using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace System.Collections.Generic
{
    public class SuperSet<T>
        where T : IComparable<T>, IEquatable<T>
    {

        private readonly List<T> items = new List<T>();
        private readonly List<Range<T>> ranges = new List<Range<T>>();

        public ReadOnlyCollection<T> Items
        {
            get { return items.AsReadOnly(); }
        }
        public void AddItem(T item)
        {
            if (!this.items.Contains(item))
            {
                this.items.Add(item);
            }
        }
        public bool RemoveItem(T item)
        {
            return this.items.Remove(item);
        }
        public bool RemoveRange(Range<T> range)
        {
            return this.ranges.Remove(range);
        }
        public void AddRange(Range<T> range)
        {
            if (!this.ranges.Contains(range))
            {
                this.ranges.Add(range);
            }
        }

        public ReadOnlyCollection<Range<T>> Ranges
        {
            get { return ranges.AsReadOnly(); }
        }

        public bool Contains(T item)
        {
            foreach (var it in this.items)
            {
                if (item.CompareTo(it) == 0)
                {
                    return true;
                }
            }
            foreach (var range in this.ranges)
            {
                if (range.ContainsValue(item))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual FlagData<T> ConvertItemFromString(string content)
        {
            try
            {
                return new FlagData<T>(true, (T)Convert.ChangeType(content, typeof(T)));
            }
            catch
            {
                return new FlagData<T>(false);
            }
        }

        public virtual void ParseFromString(string desc)
        {
            string Sp = ",";
            string Ep = "-";
            string[] items = desc.Split(new string[] { Sp }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                if (item.Contains(Ep))
                {
                    string[] arr = item.Split(new string[] { Ep }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2)
                    {
                        var v1 = ConvertItemFromString(arr[0]);
                        var v2 = ConvertItemFromString(arr[1]);
                        if (v1 && v2)
                        {
                            Range<T> ran = new Range<T>();
                            ran.Minimum = v1.Item;
                            ran.Maximum = v2.Item;
                            this.ranges.Add(ran);
                        }
                    }
                }
                else
                {
                    var v = ConvertItemFromString(item);
                    if(v) this.items.Add(v.Item);
                }
            }
        }

        //public virtual bool ContainsString(string itemstr)
        //{
        //    var v = this.ConvertItemFromString(itemstr);
        //    if (v)
        //    {
        //        return this.Contains(v.Item);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
