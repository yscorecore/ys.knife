using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Timers
{
    public class DateTimeEx
    {
        private int _Year;
        public int Year
        {
            get { return this._Year; }
            set { this._Year = value; }
        }

        #region Month
        private int _MonthOfYear;
        public int MonthOfYear
        {
            get { return this._MonthOfYear; }
            set { this._MonthOfYear = value; }
        }
        private int _LastMonthOfYear;
        public int LastMonthOfYear
        {
            get { return this._LastMonthOfYear; }
            set { this._LastMonthOfYear = value; }
        }
        #endregion

        #region Week
        private int _WeekdayOfMonth;
        /// <summary>
        /// 表示所在的星期(日，一，二，三，四，五，六)在当前月份出现的次序
        /// </summary>
        public int WeekdayOfMonth
        {
            get { return this._WeekdayOfMonth; }
            set { this._WeekdayOfMonth = value; }
        }

        private int _WeekdayOfYear;
        /// <summary>
        /// 表示所在的星期(日，一，二，三，四，五，六)在当前年份出现的次序
        /// </summary>
        public int WeekdayOfYear
        {
            get { return this._WeekdayOfYear; }
            set { this._WeekdayOfYear = value; }
        }
        private int _LastWeekdayOfMonth;
        /// <summary>
        /// 表示所在的星期(日，一，二，三，四，五，六)在当前月份倒数出现的次序
        /// </summary>
        public int LastWeekdayOfMonth
        {
            get { return this._LastWeekdayOfMonth; }
            set { this._LastWeekdayOfMonth = value; }
        }
        private int _LastWeekdayOfYear;
        /// <summary>
        /// 表示所在的星期(日，一，二，三，四，五，六)在当前月份倒数出现的次序
        /// </summary>
        public int LastWeekdayOfYear
        {
            get { return this._LastWeekdayOfYear; }
            set { this._LastWeekdayOfYear = value; }
        }
        #endregion

        #region Day
        private int _DayOfWeek;
        public int DayOfWeek
        {
            get { return this._DayOfWeek; }
            set { this._DayOfWeek = value; }
        }
        private int _DayOfMonth;
        public int DayOfMonth
        {
            get { return this._DayOfMonth; }
            set { this._DayOfMonth = value; }
        }
        private int _DayOfYear;
        public int DayOfYear
        {
            get { return this._DayOfYear; }
            set { this._DayOfYear = value; }
        }
        private int _LastDayOfWeek;
        public int LastDayOfWeek
        {
            get { return this._LastDayOfWeek; }
            set { this._LastDayOfWeek = value; }
        }
        private int _LastDayOfMonth;
        public int LastDayOfMonth
        {
            get { return this._LastDayOfMonth; }
            set { this._LastDayOfMonth = value; }
        }
        private int _LastDayOfYear;
        public int LastDayOfYear
        {
            get { return this._LastDayOfYear; }
            set { this._LastDayOfYear = value; }
        }
        #endregion

        #region Hour
        private int _HourOfDay;
        public int HourOfDay
        {
            get { return this._HourOfDay; }
            set { this._HourOfDay = value; }
        }
        private int _HourOfWeek;
        public int HourOfWeek
        {
            get { return this._HourOfWeek; }
            set { this._HourOfWeek = value; }
        }
        private int _HourOfMonth;
        public int HourOfMonth
        {
            get { return this._HourOfMonth; }
            set { this._HourOfMonth = value; }
        }
        private int _HourOfYear;
        public int HourOfYear
        {
            get { return this._HourOfYear; }
            set { this._HourOfYear = value; }
        }
        private int _LastHourOfDay;
        public int LastHourOfDay
        {
            get { return this._LastHourOfDay; }
            set { this._LastHourOfDay = value; }
        }
        private int _LastHourOfWeek;
        public int LastHourOfWeek
        {
            get { return this._LastHourOfWeek; }
            set { this._LastHourOfWeek = value; }
        }
        private int _LastHourOfMonth;
        public int LastHourOfMonth
        {
            get { return this._LastHourOfMonth; }
            set { this._LastHourOfMonth = value; }
        }
        private int _LastHourOfYear;
        public int LastHourOfYear
        {
            get { return this._LastHourOfYear; }
            set { this._LastHourOfYear = value; }
        }
        #endregion

        #region Minute
        private int _MinuteOfHour;
        public int MinuteOfHour
        {
            get { return this._MinuteOfHour; }
            set { this._MinuteOfHour = value; }
        }
        private int _MinuteOfDay;
        public int MinuteOfDay
        {
            get { return this._MinuteOfDay; }
            set { this._MinuteOfDay = value; }
        }
        private int _MinuteOfWeek;
        public int MinuteOfWeek
        {
            get { return this._MinuteOfWeek; }
            set { this._MinuteOfWeek = value; }
        }
        private int _MinuteOfMonth;
        public int MinuteOfMonth
        {
            get { return this._MinuteOfMonth; }
            set { this._MinuteOfMonth = value; }
        }
        private int _MinuteOfYear;
        public int MinuteOfYear
        {
            get { return this._MinuteOfYear; }
            set { this._MinuteOfYear = value; }
        }
        private int _LastMinuteOfHour;
        public int LastMinuteOfHour
        {
            get { return this._LastMinuteOfHour; }
            set { this._LastMinuteOfHour = value; }
        }
        private int _LastMinuteOfDay;
        public int LastMinuteOfDay
        {
            get { return this._LastMinuteOfDay; }
            set { this._LastMinuteOfDay = value; }
        }
        private int _LastMinuteOfWeek;
        public int LastMinuteOfWeek
        {
            get { return this._LastMinuteOfWeek; }
            set { this._LastMinuteOfWeek = value; }
        }
        private int _LastMinuteOfMonth;
        public int LastMinuteOfMonth
        {
            get { return this._LastMinuteOfMonth; }
            set { this._LastMinuteOfMonth = value; }
        }
        private int _LastMinuteOfYear;
        public int LastMinuteOfYear
        {
            get { return this._LastMinuteOfYear; }
            set { this._LastMinuteOfYear = value; }
        }
        #endregion

        #region Second
        private int _SecondOfMinute;
        public int SecondOfMinute
        {
            get { return this._SecondOfMinute; }
            set { this._SecondOfMinute = value; }
        }
        private int _SecondOfHour;
        public int SecondOfHour
        {
            get { return this._SecondOfHour; }
            set { this._SecondOfHour = value; }
        }
        private int _SecondOfDay;
        public int SecondOfDay
        {
            get { return this._SecondOfDay; }
            set { this._SecondOfDay = value; }
        }
        private int _SecondOfWeek;
        public int SecondOfWeek
        {
            get { return this._SecondOfWeek; }
            set { this._SecondOfWeek = value; }
        }
        private int _SecondOfMonth;
        public int SecondOfMonth
        {
            get { return this._SecondOfMonth; }
            set { this._SecondOfMonth = value; }
        }
        private int _SecondOfYear;
        public int SecondOfYear
        {
            get { return this._SecondOfYear; }
            set { this._SecondOfYear = value; }
        }
        private int _LastSecondOfMinute;
        public int LastSecondOfMinute
        {
            get { return this._LastSecondOfMinute; }
            set { this._LastSecondOfMinute = value; }
        }
        private int _LastSecondOfHour;
        public int LastSecondOfHour
        {
            get { return this._LastSecondOfHour; }
            set { this._LastSecondOfHour = value; }
        }
        private int _LastSecondOfDay;
        public int LastSecondOfDay
        {
            get { return this._LastSecondOfDay; }
            set { this._LastSecondOfDay = value; }
        }
        private int _LastSecondOfWeek;
        public int LastSecondOfWeek
        {
            get { return this._LastSecondOfWeek; }
            set { this._LastSecondOfWeek = value; }
        }
        private int _LastSecondOfMonth;
        public int LastSecondOfMonth
        {
            get { return this._LastSecondOfMonth; }
            set { this._LastSecondOfMonth = value; }
        }
        private int _LastSecondOfYear;
        public int LastSecondOfYear
        {
            get { return this._LastSecondOfYear; }
            set { this._LastSecondOfYear = value; }
        }
        #endregion

        public static DateTimeEx FromDateTime(DateTime dateTime)
        {
            bool leapyear = IsLeapYear(dateTime);
            int totaldays = leapyear ? 366 : 365;
            int monthdays = GetMonthDays(leapyear, dateTime.Month);
            var res = new DateTimeEx();
            int dayofyear = dateTime.DayOfYear;
            res._Year = dateTime.Year;
            DateTime firstYearDay = new DateTime(dateTime.Year, 1, 1);
            DateTime lastYearDay = new DateTime(dateTime.Year, 12, 31);
            DateTime firstMonthDay = new DateTime(dateTime.Year, dateTime.Month, 1);
            DateTime lastMondthDay = new DateTime(dateTime.Year, dateTime.Month, monthdays);
            #region month
            res._MonthOfYear = dateTime.Month;
            res._LastMonthOfYear = 13 - dateTime.Month;
            #endregion

            #region week
            int count = dayofyear / 7;
            #endregion

            #region Day
            res._DayOfMonth = dateTime.Day;
            res._DayOfYear = dayofyear;
            res._DayOfWeek = GetDayOfWeek(dateTime.DayOfWeek);

            res._LastDayOfMonth = monthdays - dateTime.Day;
            res._LastDayOfYear = totaldays - res._DayOfYear + 1;
            res._LastDayOfWeek = 8 - res._DayOfWeek;
            #endregion

            #region hour
            res._HourOfDay = dateTime.Hour;
            res._HourOfWeek = (res._DayOfWeek - 1) * 24 + res._HourOfDay;
            res._HourOfMonth = (res._DayOfMonth - 1) * 24 + res._HourOfDay;
            res._HourOfYear = (res._DayOfYear - 1) * 24 + res._HourOfDay;

            res._LastHourOfDay = 24 - res._HourOfDay;
            res._LastHourOfMonth = (res._LastDayOfMonth - 1) * 24 + res._LastHourOfDay;
            res._LastHourOfWeek = (res._LastDayOfWeek - 1) * 24 + res._LastHourOfDay;
            res._LastHourOfYear = (res._LastDayOfYear - 1) * 24 + res._LastHourOfDay;
            #endregion

            #region minute
            res._MinuteOfHour = dateTime.Minute;
            res._MinuteOfDay = (res._HourOfDay) * 60 + res._MinuteOfHour;
            res._MinuteOfWeek = (res._HourOfWeek) * 60 + res._MinuteOfHour;
            res._MinuteOfMonth = (res._HourOfMonth) * 60 + res._MinuteOfHour;
            res._MinuteOfYear = (res._HourOfYear) * 60 + res._MinuteOfHour;

            res._LastMinuteOfHour = 60 - res._MinuteOfHour;
            res._LastMinuteOfDay = (res._LastHourOfDay - 1) * 60 + res._LastMinuteOfHour;
            res._LastMinuteOfWeek = (res._LastHourOfWeek - 1) * 60 + res._LastMinuteOfHour;
            res._LastMinuteOfMonth = (res._LastHourOfMonth - 1) * 60 + res._LastMinuteOfHour;
            res._LastMinuteOfYear = (res._LastHourOfYear - 1) * 60 + res._LastMinuteOfHour;
            #endregion

            #region second
            res._SecondOfMinute = dateTime.Second;
            res._SecondOfHour = (res._MinuteOfHour) * 60 + res._SecondOfMinute;
            res._SecondOfDay = (res._MinuteOfDay) * 60 + res._SecondOfMinute;
            res._SecondOfWeek = (res._MinuteOfWeek) * 60 + res._SecondOfMinute;
            res._SecondOfMonth = (res._MinuteOfMonth) * 60 + res._SecondOfMinute;
            res._SecondOfYear = (res._MinuteOfYear) * 60 + res._SecondOfMinute;

            res._LastSecondOfMinute = 60 - res._SecondOfMinute;
            res._LastSecondOfHour = (res._LastMinuteOfHour - 1) * 60 + res._LastSecondOfMinute;
            res._LastSecondOfDay = (res._LastMinuteOfDay - 1) * 60 + res._LastSecondOfMinute;
            res._LastSecondOfWeek = (res._LastMinuteOfWeek - 1) * 60 + res._LastSecondOfMinute;
            res._LastSecondOfMonth = (res._LastMinuteOfMonth - 1) * 60 + res._LastSecondOfMinute;
            res._LastSecondOfYear = (res._LastMinuteOfYear - 1) * 60 + res._LastSecondOfMinute;

            #endregion
            return res;
        }

        public static bool IsLeapYear(int year)
        {
            return (year % 400 == 0 && year % 3200 != 0)
               || (year % 4 == 0 && year % 100 != 0)
               || (year % 3200 == 0 && year % 172800 == 0);

        }
        public static bool IsLeapYear(DateTime dateTime)
        {
            return IsLeapYear(dateTime.Year);
        }
        public static int GetDayOfWeek(DayOfWeek wd)
        {
            return (int)wd + 1;
        }
        public static int GetWeekOfYear(DateTime time)
        {
            return 1;
        }
        public static int GetWeekOfMonth(DateTime time)
        {
            return 2;
        }
        public static int GetMonthDays(bool isleapYear, int month)
        {
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    return isleapYear ? 29 : 28;
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                default:
                    return 0;
            }
        }
    }
}
