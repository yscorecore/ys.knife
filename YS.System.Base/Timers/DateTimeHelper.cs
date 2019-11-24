using System;
using System.Collections.Generic;
using System.Text;

namespace System.Timers
{
    /// <summary>
    /// 提供一些时间日期的辅助函数
    /// </summary>
    public static class DateTimeUtility
    {
        #region 字段
        private static float[] atomBound = { 1.20F, 2.20F, 3.21F, 4.21F, 5.21F, 6.22F, 7.23F, 8.23F, 9.23F, 10.23F, 11.21F, 12.22F, 13.20F };
       // private static string[] atoms = { "水瓶座", "双鱼座", "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "魔羯座" };
        #endregion
        /// <summary>
        /// 将UTC时间转换为UNIX时间戳。
        /// </summary>
        /// <param name="utcTime">需要转换的UTC时间</param>
        /// <returns>返回对应的UNIX时间戳</returns>
        public static Int32 UtcTimeToMark (DateTime utcTime) {
            return (Int32)(utcTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }
        /// <summary>
        /// 将本地时间转换为UNIX时间戳。
        /// </summary>
        /// <param name="localTime">需要转换的本地时间。</param>
        /// <returns>返回对应的UNIX时间戳</returns>
        public static Int32 LocalTimeToMark (DateTime localTime) {
            return UtcTimeToMark(localTime.ToUniversalTime());
        }
        /// <summary>
        /// 取得当前时刻对应的时间戳。
        /// </summary>
        /// <returns>返回当前时刻的UNIX时间戳</returns>
        public static Int32 GetTimeMark () {
            return UtcTimeToMark(DateTime.UtcNow);
        }
        /// <summary>
        /// 将UNIX时间戳转换成UTC时间。
        /// </summary>
        /// <param name="timeMark">需要转换的UNIX时间戳</param>
        /// <returns>返回对应的UTC时间。</returns>
        public static DateTime MarkToUtcTime (Int32 timeMark) {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(timeMark);
        }
        /// <summary>
        /// 将UNIX时间戳转换成本地时间。
        /// </summary>
        /// <param name="timeMark">需要转换的UNIX时间戳</param>
        /// <returns>返回对应的本地时间。</returns>
        public static DateTime MarkToLocalTime (Int32 timeMark) {
            return MarkToUtcTime(timeMark).ToLocalTime();
        }
        /// <summary>
        /// 根据生日获取对应的星座信息
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GetAtomFromBirthday (DateTime birthday) {
            float birthdayF = 0.00F;
            if (birthday.Month == 1 && birthday.Day < 20) {
                birthdayF = float.Parse(string.Format("13.{0}", birthday.Day));
            }
            else {
                birthdayF = float.Parse(string.Format("{0}.{1}", birthday.Month, birthday.Day));
            }
            string ret = string.Empty;
            for (int i = 0; i < atomBound.Length - 1; i++) {
                if (atomBound[i] <= birthdayF && atomBound[i + 1] > birthdayF) {
                    ret = ResourceManager.GetString(string.Format("_{0}",i));
                    break;
                }
            }
            return ret;
        }
        private static global::System.Resources.ResourceManager resourceMan;
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if(object.ReferenceEquals(resourceMan,null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("System.Timers.Constellation",typeof(DateTimeUtility).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
    }
}
