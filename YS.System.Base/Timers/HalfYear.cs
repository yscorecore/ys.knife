using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace System.Timers
{
    public enum HalfYearKind
    {
        /// <summary>
        /// 上半年
        /// </summary>
        FirstHalfYear,
        /// <summary>
        /// 下半年
        /// </summary>
        SecondHalfYear,
    }
    [Serializable]
    public class HalfYear:YSTime
    {
        public HalfYearKind HalfYearKind
        {
            get
            {
                if(this.DateTime.Month > 6)
                {
                    return HalfYearKind.SecondHalfYear;
                }
                else
                {
                    return HalfYearKind.FirstHalfYear;
                }
            }
        }

        #region 构造函数
        public HalfYear(int year,HalfYearKind halfYearKind)
        {
            if(!Enum.IsDefined(typeof(HalfYearKind),halfYearKind))
            {
                throw new InvalidEnumArgumentException();
            }
            else if(halfYearKind == HalfYearKind.FirstHalfYear)
            {
                this.DateTime = new DateTime(year,1,1);
            }
            else
            {
                this.DateTime = new DateTime(year,7,1);
            }
        }
        public HalfYear(DateTime datetime)
        {
            if(datetime.Month > 6)
            {
                this.DateTime = new DateTime(datetime.Year,7,1);
            }
            else
            {
                this.DateTime = new DateTime(datetime.Year,1,1);
            }
        }
        #endregion

        protected override YSTime GetNextTime(DateTime dt,int step)
        {
            return new HalfYear(dt.AddMonths(6 * step));
        }
        public override string ToString()
        {
            return base.ToString("yyyyMM");
        }
    }
}
