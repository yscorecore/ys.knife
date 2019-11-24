using System;
using System.Runtime.Serialization;

namespace System.Timers
{
    /// <summary>
    /// 表示时间的基类
    /// </summary>
    [Serializable]
    public abstract class YSTime
    {
        public DateTime DateTime { get; protected set; }

        #region 重写
        public override bool Equals(object obj)
        {
            if(object.ReferenceEquals(this,obj)) return true;
            if(obj is YSTime)
            {
                if(this.GetType() == obj.GetType())
                {
                    return this.DateTime == (obj as YSTime).DateTime;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return this.DateTime.GetHashCode();
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return this.DateTime.ToString();
        }
        public virtual string ToString(string format)
        {
            return this.DateTime.ToString(format);
        }
        public virtual string ToString(string format,IFormatProvider provider)
        {
            return this.DateTime.ToString(format,provider);
        }
        #endregion

        #region
        public YSTime NextTime()
        {
            return NextTime(1);
        }
        public YSTime NextTime(int step)
        {
            return GetNextTime(this.DateTime,step);
        }
        public YSTime PreviousTime()
        {
            return NextTime(-1);
        }
        public YSTime PreviousTime(int step)
        {
            return GetNextTime(this.DateTime,-step);
        }
        #endregion

        protected abstract YSTime GetNextTime(DateTime dt,int step);
    }
}