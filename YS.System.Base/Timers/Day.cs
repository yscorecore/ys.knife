using System;
using System.Collections.Generic;
using System.Text;

namespace System.Timers
{
    [Serializable]
    public class Day:YSTime
    {
        #region 构造函数
        public Day()
        {
            this.DateTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
        }
        public Day(int year,int month,int day)
        {
            this.DateTime = new DateTime(year,month,day);
        }
        public Day(DateTime dateTime)
        {
            this.DateTime = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day);
        }
        #endregion

        public bool IsFirstDayInMonth
        {
            get 
            {
                return this.DateTime.Day == 1;
            }
        }
        public bool IsLastDayInMonth
        {
            get {
                return this.DateTime.AddDays(1).Day == 1;
            }
        }
        public override string ToString()
        {
            return this.ToString("yyyyMMdd");
        }
        protected override YSTime GetNextTime(DateTime dt,int step)
        {
            return new Day(this.DateTime.AddDays(step));
        }
    }
}
