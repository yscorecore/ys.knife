using System;
using System.Collections.Generic;
using System.Text;

namespace System.Timers
{
    [Serializable]
    public class Year:YSTime
    {
        #region 构造函数
        public Year()
        {
            this.DateTime = new DateTime(DateTime.Now.Year,1,1);
        }
        public Year(int year)
        {
            this.DateTime = new DateTime(year,1,1);
        }
        public Year(DateTime dateTime)
        {
            this.DateTime = new DateTime(dateTime.Year,1,1);
        }
        #endregion

        public int YearValue
        {
            get { return this.DateTime.Year; }
        }
        protected override YSTime GetNextTime(DateTime dateTime,int step)
        {
            return new Year(dateTime.AddYears(step));
        }
        public override string ToString()
        {
            return base.ToString("yyyy");
        }
        public static Year Parse(string str)
        {
            return new Year(int.Parse(str));
        }
        public static Year TryParse(string str)
        {
            int y;
            bool res = int.TryParse(str,out y);
            if(res)
            {
                if(y < DateTime.MinValue.Year || y > DateTime.MaxValue.Year)
                {
                    return null;
                }
                else
                {
                    return new Year(y);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
