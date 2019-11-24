using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Text.RegularExpressions
{
    /// <summary>
    /// 提供一些常用的字符串验证方法
    /// </summary>
    public static class StringEx
    {
        /// <summary>
        /// 是否是合法的ipv4地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIPV4Address(this string input)
        {
            string num = "(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)";
            return Regex.IsMatch(input, ("^" + num + "\\." + num + "\\." + num + "\\." + num + "$"));
        }
        /// <summary>
        /// 是否是合法的手机号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(this string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[1]+[3,5,8]+\d{9}");
        }
        /// <summary>
        /// 是否是合法的邮箱地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(this string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9] {1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\)?]$");
        }
        /// <summary>
        /// 是否是有效的Uri格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsUri(this string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^\w{2,8}://");
        }
        /// <summary>
        /// 是否是合法的邮编地址
        /// </summary>
        /// <param name="str_postalcode"></param>
        /// <returns></returns>
        public static bool IsPostalcode(this string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d{6}$");
        }
        /// <summary>
        /// 是否是正整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsInteger(this string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[0-9]*$");
        }
        /// <summary>
        /// 是否是身份证号（15位）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIdentityCard15(this string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$");
        }
        /// <summary>
        /// 是否是身份证号（18位）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIdentityCard18(this string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}(\d|x|X)$");
        }
        /// <summary>
        /// 是否是正确的QQ号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsQQNO(this string input)
        {
            return Regex.IsMatch(input, @"([1-9][0-9]{4})|([0-9]{6,10})");
        }
    }
}
