using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace System.Timers
{
    /// <summary>
    /// 表示中国传统的农历日期
    /// </summary>
    public class LunarDate2
    {
        private static ChineseLunisolarCalendar chineseLunisolar = new ChineseLunisolarCalendar();

        public const string LeapString = "闰";
        // 十天干
        private static string[] tiangan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        //十二地支
        private static string[] dizhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        // 十二生肖
        private static string[] shengxiao = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

        private static string[] months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "腊" };
        //农历日
        private static string[] days = { "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十", 

                                         "十一", "十二","十三", "十四" ,"十五", "十六","十七", "十八" ,"十九", "二十",
    
                                         "廿一", "廿二","廿三", "廿四" ,"廿五", "廿六","廿七", "廿八" ,"廿九", "三十"};
        private DateTime dateTime;
        /// <summary>
        /// 初始化<see cref="LunarDate2"/>的新实例。
        /// </summary>
        /// <param name="dateTime">公元纪年法时间</param>
        public LunarDate2 (DateTime dateTime) {
            this.dateTime = dateTime;
        }

        public static LunarDate2 FromDateTime (DateTime dateTime) {
            return new LunarDate2(dateTime);
        }
        /// <summary>
        /// 获取指定日期对应的生肖信息
        /// </summary>
        /// <param name="dateTime">指定的日期</param>
        /// <returns>返回对应的生肖信息</returns>
        public static string GetZodiac (DateTime dateTime) {
            int index = chineseLunisolar.GetTerrestrialBranch(chineseLunisolar.GetSexagenaryYear(dateTime)) - 1;
            return shengxiao[index];
        }
        /// <summary>
        /// 获取指定日期对应的生肖全称
        /// </summary>
        /// <param name="dateTime">指定的日期</param>
        /// <returns>返回对应的生肖信息</returns>
        public static string GetFullZodiac (DateTime dateTime) {
            int index = chineseLunisolar.GetTerrestrialBranch(chineseLunisolar.GetSexagenaryYear(dateTime)) - 1;
            return dizhi[index] + shengxiao[index];
        }
        /// <summary>
        /// 获取指定日期所在年的天干信息
        /// </summary>
        /// <param name="dateTime">指定的日期</param>
        /// <returns>返回对应的天干</returns>
        public static string GetCelestialStem (DateTime dateTime) {
            int index = chineseLunisolar.GetCelestialStem(chineseLunisolar.GetSexagenaryYear(dateTime)) - 1;
            return tiangan[index];
        }
        /// <summary>
        /// 获取指定日期所在年的地支信息
        /// </summary>
        /// <param name="dateTime">指定的日期</param>
        /// <returns>返回对应的地支</returns>
        public static string GetTerrestrialBranch (DateTime dateTime) {
            int index = chineseLunisolar.GetTerrestrialBranch(chineseLunisolar.GetSexagenaryYear(dateTime)) - 1;
            return dizhi[index];
        }
        /// <summary>
        /// 获取指定日期的农历年
        /// </summary>
        /// <param name="dateTime">指定的日期</param>
        /// <returns>返回农历年名称</returns>
        public static string GetLunisolarYear (DateTime dateTime) {
            int year = chineseLunisolar.GetSexagenaryYear(dateTime);
            int index_tianGan = chineseLunisolar.GetCelestialStem(year) - 1;
            int index_dizhi = chineseLunisolar.GetTerrestrialBranch(year) - 1;
            return tiangan[index_tianGan] + dizhi[index_dizhi];
        }
        /// <summary>
        /// 获取指定日期对应的农历月
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetLunisolarMonth (DateTime dateTime) {
           int year=  chineseLunisolar.GetYear(dateTime);//2010年2月13日，返回2009,2010年2月14日，返回2010，因为2010年2月14日是春节。
           int month = chineseLunisolar.GetMonth(dateTime);
           if (chineseLunisolar.IsLeapYear(year)) {//闰年
               int leapmonth = chineseLunisolar.GetLeapMonth(year);
               if (month < leapmonth) {
                   return months[month - 1];
               }
               else if (month == leapmonth ) {//闰月
                   return LeapString + months[month - 2];
               }
               else {
                   return months[month - 2];
               }
           }
           else {//平年 
               return months[month - 1];
           }
        }
        /// <summary>
        /// 获取指定日期对应的农历日
        /// </summary>
        /// <param name="dateTime">指定的日期</param>
        /// <returns></returns>
        public static string GetLunisolarDay (DateTime dateTime) {
            int dayindex = chineseLunisolar.GetDayOfMonth(dateTime) - 1;
            return days[dayindex];
        }

     
    }
}
