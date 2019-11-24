using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    [Serializable]
    public struct Range<T> : IEquatable<Range<T>>
        where T : IComparable<T>, IEquatable<T>
    {
        #region 构造函数
        //public Range()
        //{
        //}
        public Range(T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentException("最大值应当大于最小值");
            }
            this._minimum = min;
            this._maximum = max;
            this._minIntervalKind = IntervalKind.Closed;
            this._maxIntervalKind = IntervalKind.Closed;
        }
        public Range(T min, IntervalKind minkind, T max, IntervalKind maxkind)
            : this(min, max)
        {
            this._minIntervalKind = minkind;
            this._maxIntervalKind = maxkind;
        }
        #endregion
        private T _minimum;
        private T _maximum;
        private IntervalKind _minIntervalKind ;
        private IntervalKind _maxIntervalKind ;

        public IntervalKind MaxIntervalKind
        {
            get { return _maxIntervalKind; }
            set { _maxIntervalKind = value; }
        }
        public T Minimum
        {
            get { return this._minimum; }
            set { this._minimum = value; }
        }
        public T Maximum
        {
            get { return this._maximum; }
            set { this._maximum = value; }
        }


        public IntervalKind MinIntervalKind
        {
            get { return this._minIntervalKind; }
            set { this._minIntervalKind = value; }
        }
        public override string ToString()
        {
            string lowch = this.MinIntervalKind == IntervalKind.Opend ? "(" : "[";
            string highch = this.MaxIntervalKind == IntervalKind.Opend ? ")" : "]";
            return string.Format("{0}{1},{2}{3}", lowch, Minimum, Maximum, highch);
        }

        public  bool ContainsValue(T value)
        {
            int tempflag = value.CompareTo(this.Minimum);
            if (tempflag < 0 || (tempflag == 0 && this.MinIntervalKind == IntervalKind.Opend)) { return false; }
            int tempflag2 = value.CompareTo(this.Maximum);
            if (tempflag2 > 0 || (tempflag2 == 0 && this.MaxIntervalKind == IntervalKind.Opend)) { return false; }
            return true;
        }
        public  bool ContainsRange(Range<T> range)
        {
            if (range == null) throw new ArgumentNullException("range");
            bool flag1;
            int mincomres = this.Minimum.CompareTo(range.Minimum);
            if (mincomres < 0) { flag1 = true; }
            else if (mincomres > 0) { flag1 = false; }
            else { flag1 = !(this.MinIntervalKind == IntervalKind.Opend && range.MinIntervalKind == IntervalKind.Closed); }
            if (flag1 == false) return false;

            bool flag2;
            int mamcomres = this.Maximum.CompareTo(range.Maximum);
            if (mamcomres > 0) { flag2 = true; }
            else if (mamcomres < 0) { flag2 = false; }
            else { flag2 = !(this.MaxIntervalKind == IntervalKind.Opend && range.MaxIntervalKind == IntervalKind.Closed); }
            return flag2;
        }
        public  bool IntersectWith(Range<T> range)
        {
            return HasIntersect(this, range);
        }
        public  Range<T> Intersect(Range<T> range)
        {
            throw new NotImplementedException("TODO");
           // return null;

        }
        #region 静态方法
        public static Range<T> Empty = new Range<T>();
        public static bool operator ==(Range<T> leftRange, Range<T> rightRange)
        {
            if (object.ReferenceEquals(leftRange, rightRange))
            {
                return true;
            }
            else if (object.Equals(leftRange, null) && object.Equals(rightRange, null))
            {
                return true;
            }
            else if ((!object.Equals(leftRange, null)) && (!object.Equals(rightRange, null)))
            {
                return object.Equals(leftRange._maximum,rightRange._maximum) &&
                   object.Equals(leftRange._minimum, rightRange._minimum) &&
                    leftRange._maxIntervalKind == rightRange._maxIntervalKind &&
                    leftRange._minIntervalKind == rightRange._minIntervalKind;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(Range<T> leftRange, Range<T> rightRange)
        {
            return !(leftRange == rightRange);
        }
        public override bool Equals(object obj)
        {
            if (obj is Range<T>)
            {
                return this == (Range<T>)obj;
            }
            else
            {
                return false;
            }
        }
        public bool Equals(Range<T> other)
        {
            return this == other;
        }
        public override int GetHashCode()
        {
            int res = 0;
            if (this._maximum != null) res ^= this._maximum.GetHashCode();
            if (this._minimum != null) res ^= this._minimum.GetHashCode();
            return res ^ this._maxIntervalKind.GetHashCode() ^ this._minIntervalKind.GetHashCode();
        }
        #endregion


        #region 静态方法2
        /// <summary>
        /// 获取一个值，该值反应了给定的两个范围是否有交集
        /// </summary>
        /// <param name="rangel">范围1</param>
        /// <param name="range2">范围2</param>
        /// <returns></returns>
        public static bool HasIntersect(Range<T> rangel, Range<T> range2)
        {
            if (rangel != null && range2 != null)
            {
                throw new NotImplementedException("TODO");

                //TODO....
                return false;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }


    public enum IntervalKind
    {
        /// <summary>
        /// 开区间
        /// </summary>
        Opend = 0,
        /// <summary>
        /// 闭区间
        /// </summary>
        Closed = 1,

    }

    public interface IRange<T>
        where T:IComparer<T>,IEquatable<T>
    {
         T StartValue { get; set; }
         T EndValue { get; set; }


    }
}
