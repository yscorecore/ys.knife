using System;
using System.Collections.Generic;
using System.Text;

namespace System.Timers
{
    /// <summary>
    /// 表示节气的类
    /// </summary>
    public class SolarTerm:IFestival
    {
        static string[] lunarHoliDayName ={ 
            "小寒", "大寒", "立春", "雨水","惊蛰", "春分", "清明", "谷雨","立夏", "小满", "芒种", "夏至", 
            "小暑", "大暑", "立秋", "处暑","白露", "秋分", "寒露", "霜降","立冬", "小雪", "大雪", "冬至"};
        private readonly DateTime solarTermDate;
        private readonly string name;
        private SolarTerm (DateTime dateTime, string name) {
            this.solarTermDate = dateTime;
            this.name = name;
        }
        /// <summary>
        /// 获取或设置节气的描述信息。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得指定月份的节气。
        /// </summary>
        /// <param name="year">公历年份</param>
        /// <param name="month">公历月份</param>
        /// <returns>返回此月份的两个节气</returns>
        public static SolarTerm[] GetSolarTerm (int year, int month) {
            SolarTerm[] solarTerm = new SolarTerm[2];
            for (int n = month * 2 - 1; n <= month * 2; n++) {
                double dd = Term(year, n, true);
                double sd1 = AntiDayDifference(year, Math.Floor(dd));
                double sm1 = Math.Floor(sd1 / 100);
                int h = (int)Math.Floor((double)Tail(dd) * 24);
                int min = (int)Math.Floor((double)(Tail(dd) * 24 - h) * 60);
                int mmonth = (int)Math.Ceiling((double)n / 2);
                int day = (int)sd1 % 100;
                solarTerm[n - month * 2 + 1] = new SolarTerm(new DateTime(year, month, day, h, min, 0), lunarHoliDayName[n - 1]);
            }
            return solarTerm;
        }
        /// <summary>
        /// 取得指定年份是所有节气
        /// </summary>
        /// <param name="year">公历年份</param>
        /// <returns>返回对应年份的24节气</returns>
        public static SolarTerm[] GetSolarTerm (int year) {
            SolarTerm[] yearTerms = new SolarTerm[24];
            for (int i = 1; i <= 12; i++) {
                SolarTerm[] monthTerm = GetSolarTerm(year, i);
                yearTerms[(i - 1) * 2] = monthTerm[0];
                yearTerms[i * 2 - 1] = monthTerm[1];
            }
            return yearTerms;
        }
        /// <summary>
        /// 取得指定日期之间的所有节气
        /// </summary>
        public static SolarTerm[] GetSolarTerm (int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay) {
            DateTime dstart = new DateTime(startYear, startMonth, startDay);
            DateTime dend = new DateTime(endYear, endMonth, endDay, 23, 59, 59);
            if (dstart > dend) throw new ArgumentException("起始日期不能在结束日期的后面");
            List<SolarTerm> lst = new List<SolarTerm>();
            SolarTerm[] tempSolars;
            if (startYear == endYear) { //同年
                if (startMonth == endMonth) {//同年同月
                    tempSolars = GetSolarTerm(startYear, startMonth);
                    if (tempSolars[0].DateTime < dend && tempSolars[0].DateTime > dstart) lst.Add(tempSolars[0]);
                    if (tempSolars[1].DateTime < dend && tempSolars[1].DateTime > dstart) lst.Add(tempSolars[1]);
                }
                else {//同年不同月
                    tempSolars = GetSolarTerm(startYear, startMonth);
                    if (tempSolars[0].DateTime > dstart) lst.Add(tempSolars[0]);
                    if (tempSolars[1].DateTime > dstart) lst.Add(tempSolars[1]);
                    for (int i = startMonth + 1; i <= endMonth - 1; i++) {
                        lst.AddRange(GetSolarTerm(startYear, i));
                    }
                    tempSolars = GetSolarTerm(startYear, endMonth);
                    if (tempSolars[0].DateTime < dend) lst.Add(tempSolars[0]);
                    if (tempSolars[1].DateTime < dend) lst.Add(tempSolars[1]);
                }

            }
            else//不同年
            {
                lst.AddRange(GetSolarTerm(startYear, startMonth, startDay, startYear, 12, 31));
                for (int i = startYear + 1; i <= endYear - 1; i++) {
                    lst.AddRange(GetSolarTerm(i));
                }
                lst.AddRange(GetSolarTerm(endYear, 1, 1, endYear, endMonth, endDay));
            }
            return lst.ToArray();
        }
        #region 辅助函数
        /// <summary>
        ///返回y年第n个节气（如小寒为1）的日差天数值（pd取值真假，分别表示平气和定气）
        /// </summary>
        private static double Term (int y, int n, bool pd) {
            double juD = y * (365.2423112 - 6.4e-14 * (y - 100) * (y - 100) - 3.047e-8 * (y - 100)) + 15.218427 * n + 1721050.71301;//儒略日
            double tht = 3e-4 * y - 0.372781384 - 0.2617913325 * n;//角度
            double yrD = (1.945 * Math.Sin(tht) - 0.01206 * Math.Sin(2 * tht)) * (1.048994 - 2.583e-5 * y);//年差实均数
            double shuoD = -18e-4 * Math.Sin(2.313908653 * y - 0.439822951 - 3.0443 * n);//朔差实均数
            double vs = (pd) ? (juD + yrD + shuoD - EquivalentStandardDay(y, 1, 0) - 1721425) : (juD - EquivalentStandardDay(y, 1, 0) - 1721425);
            return vs;
        }
        /// <summary>
        /// 返回等效标准天数（y年m月d日相应历种的1年1月1日的等效(即对Gregorian历与Julian历是统一的)天数）
        /// </summary>
        private static double EquivalentStandardDay (int y, int m, int d) {
            double v = (y - 1) * 365 + Math.Floor((double)((y - 1) / 4)) + DayDifference(y, m, d) - 2;  //Julian的等效标准天数
            if (y > 1582)
                v += -Math.Floor((double)((y - 1) / 100)) + Math.Floor((double)((y - 1) / 400)) + 2;  //Gregorian的等效标准天数
            return v;
        }
        /// <summary>
        /// 返回阳历y年m月d日的日差天数（在y年年内所走过的天数，如2000年3月1日为61）
        /// </summary>
        private static int DayDifference (int y, int m, int d) {
            int ifG = IfGregorian(y, m, d, 1);
            int[] monL = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if (ifG == 1)
                if ((y % 100 != 0 && y % 4 == 0) || (y % 400 == 0))
                    monL[2] += 1;
                else
                    if (y % 4 == 0)
                        monL[2] += 1;
            int v = 0;
            for (int i = 0; i <= m - 1; i++) {
                v += monL[i];
            }
            v += d;
            if (y == 1582) {
                if (ifG == 1)
                    v -= 10;
                if (ifG == -1)
                    v = 0;  //infinity 
            }
            return v;
        }
        /// <summary>
        /// 判断y年m月(1,2,..,12,下同)d日是Gregorian历还是Julian历（opt=1,2,3分别表示标准日历,Gregorge历和Julian历）,是则返回1，是Julian历则返回0，若是Gregorge历所删去的那10天则返回-1
        /// </summary>
        private static int IfGregorian (int y, int m, int d, int opt) {
            if (opt == 1) {
                if (y > 1582 || (y == 1582 && m > 10) || (y == 1582 && m == 10 && d > 14))
                    return (1);  //Gregorian
                else
                    if (y == 1582 && m == 10 && d >= 5 && d <= 14)
                        return (-1);  //空
                    else
                        return (0);  //Julian
            }

            if (opt == 2)
                return (1);  //Gregorian
            if (opt == 3)
                return (0);  //Julian
            return (-1);
        }
        /// <summary>
        /// 返回阳历y年日差天数为x时所对应的月日数（如y=2000，x=274时，返回1001(表示10月1日，即返回100*m+d)）
        /// </summary>
        private static double AntiDayDifference (int y, double x) {
            int m = 1;
            for (int j = 1; j <= 12; j++) {
                int mL = DayDifference(y, j + 1, 1) - DayDifference(y, j, 1);
                if (x <= mL || j == 12) {
                    m = j;
                    break;
                }
                else
                    x -= mL;
            }
            return 100 * m + x;
        }
        /// <summary>
        /// 返回x的小数尾数，若x为负值，则是1-小数尾数
        /// </summary>
        private static double Tail (double x) {
            return x - Math.Floor(x);
        }
        /// <summary>
        /// 角度函数
        /// </summary>
        private static double Angle (double x, double t, double c1, double t0, double t2, double t3) {
            return Tail(c1 * x) * 2 * Math.PI + t0 - t2 * t * t - t3 * t * t * t;
        }
        /// <summary>
        /// 广义求余
        /// </summary>
        private static double rem (double x, double w) {
            return Tail((x / w)) * w;
        }
        #endregion
        /// <summary>
        /// 获取节气的时间
        /// </summary>
        public DateTime DateTime {
            get {
                return solarTermDate;
            }
        }
        /// <summary>
        /// 获取节气的名称
        /// </summary>
        public string Name {
            get {
                return name;
            }
        }
    }
}
