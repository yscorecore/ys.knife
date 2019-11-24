using System;
using System.Collections.Generic;
using System.Text;

namespace System.Timers
{
    [Serializable]
    public class Month:YSTime
    {
        #region 构造函数
        public Month()
        {

        }
        public Month(int year,int month)
        {
            this.DateTime = new DateTime(year,month,1);
        }
        public Month(DateTime dateTime)
        {
            this.DateTime = new DateTime(dateTime.Year,dateTime.Month,1);
        }
        #endregion
        public override string ToString()
        {
            return this.ToString("YYYYMM");
        }

        protected override YSTime GetNextTime(DateTime dt,int step)
        {
            return new Month(dt.AddMonths(step));
        }
        public bool IsFirstMonthInYear
        {
            get { return this.DateTime.Month == 1; }
        }
        public bool IsLastMonthInYear
        {
            get { return this.DateTime.Month == 12; }
        }
        private Year year;
        public Year Year
        {
            get
            {
                if(year == null)
                {
                    year = new Year(this.DateTime.Year);
                }
                return year;
            }
        }

        public static Month Current
        {
            get { return new Month(DateTime.Now); }
        }
    }
}
