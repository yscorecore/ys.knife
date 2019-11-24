using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    /// <summary>
    /// 表示支持排序的BindingList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingListEx<T> : BindingList<T>
    {
        public BindingListEx()
        {

        }
        public BindingListEx(IList<T> list)
            : base(list)
        {

        }
        private bool isSorted;
        private PropertyDescriptor sortProperty;
        private ListSortDirection sortDirection;

        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortProperty; }
        }

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> items = this.Items as List<T>;

            if (items != null)
            {
                ObjectPropertyCompare<T> pc = new ObjectPropertyCompare<T>(property, direction);
                items.Sort(pc);
                isSorted = true;
            }
            else
            {
                isSorted = false;
            }

            sortProperty = property;
            sortDirection = direction;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        //排序
        public void Sort(PropertyDescriptor property, ListSortDirection direction)
        {
            this.ApplySortCore(property, direction);
        }
        #region 内部类
        class ObjectPropertyCompare<TT> : System.Collections.Generic.IComparer<TT>
        {
            private PropertyDescriptor property;
            private ListSortDirection direction;

            public ObjectPropertyCompare(PropertyDescriptor property, ListSortDirection direction)
            {
                this.property = property;
                this.direction = direction;
            }

            #region IComparer<T>

            /// <summary>
            /// 比较方法
            /// </summary>
            /// <param name="x">相对属性x</param>
            /// <param name="y">相对属性y</param>
            /// <returns></returns>
            public int Compare(TT x, TT y)
            {
                object xValue = property.GetValue(x);
                object yValue = property.GetValue(y);

                int returnValue;

                if (xValue == null)
                {
                    if (yValue == null) return 0;
                    return -1;
                }

                if (xValue is IComparable)
                {
                    returnValue = ((IComparable)xValue).CompareTo(yValue);
                }
                else if (xValue.Equals(yValue))
                {
                    returnValue = 0;
                }
                else
                {
                    returnValue = Convert.ToString(xValue).CompareTo(Convert.ToString(yValue));
                }

                if (direction == ListSortDirection.Ascending)
                {
                    return returnValue;
                }
                else
                {
                    return returnValue * -1;
                }
            }

            public bool Equals(TT xWord, TT yWord)
            {
                return xWord.Equals(yWord);
            }

            public int GetHashCode(TT obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }
        #endregion
    }

    public static class BindingListExtensions
    {
        public static BindingList<T> AsBindingList<T>(this IList<T> list)
        {
            return new BindingList<T>(list);
        }
        public static BindingListEx<T> AsBindingListEx<T>(this IList<T> list)
        {
            return new BindingListEx<T>(list);
        }
    }
}
