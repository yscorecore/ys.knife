using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security;

namespace System
{
    public static class StringEx
    {

        ///// 转全角的函数(SBC case)
        /////
        /////任意字符串
        /////全角字符串
        /////
        /////全角空格为12288，半角空格为32
        /////其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248


        /// <summary>
        /// 将字符串转换为对应的全角字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(this string input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }
        /// <summary>
        /// 将指定的字符串转换为词首字母大写
        /// </summary>
        /// <param name="input">要转换为词首字母大写的字符串</param>
        /// <returns></returns>
        public static string ToTitleCase(this string input)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }
        /// <summary>
        /// 将指定的字符串转换为词首字母大写
        /// </summary>
        /// <param name="input">要转换为词首字母大写的字符串</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string input, CultureInfo culture)
        {
            culture = culture ?? System.Globalization.CultureInfo.CurrentCulture;
            return culture.TextInfo.ToTitleCase(input);
        }
        // /
        // / 转半角的函数(DBC case)
        // /
        // /任意字符串
        // /半角字符串
        // /
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        /// <summary>
        /// 将字符串转换为对应的半角字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);

        }

        static int Min(int num1, int num2, int num3)
        {
            return Math.Min(Math.Min(num1, num2), num3);
        }
        /// <summary>
        /// 求两个字符串的 Levenshtein 编辑距离
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        /// <returns></returns>
        public static int GetLevenshteinDistance(string str1, string str2)
        {
            #region 空字符串的处理
            if (string.IsNullOrEmpty(str1))
            {
                return string.IsNullOrEmpty(str2) ? 0 : str2.Length;
            }
            if (string.IsNullOrEmpty(str2))
            {
                return str1.Length;
            }
            #endregion

            int len1 = str1.Length;
            int len2 = str2.Length;
            string temp;
            if (len1 > len2)
            {
                temp = str1; str1 = str2; str2 = temp;
                len1 ^= len2; len2 ^= len1; len1 ^= len2;
            }
            int[,] arr = new int[2, len1 + 1];
            for (int i = 0; i < len1 + 1; i++) arr[0, i] = i;//第一行
            for (int i = 0; i < len2; i++)
            {
                if (i % 2 != 0)//偶数行
                {
                    arr[0, 0] = i + 1;
                    for (int j = 1; j < len1 + 1; j++)
                    {
                        arr[0, j] = Min(
                            arr[1, j] + 1,
                            arr[0, j - 1] + 1,
                            arr[1, j - 1] + (str1[j - 1] == str2[i] ? 0 : 1));
                    }
                    //如果大字符串包含小字符串,则返回
                    if (arr[0, 0] - arr[0, len1] == len1)
                        return len2 - len1;
                }
                else//奇数行
                {
                    arr[1, 0] = i + 1;
                    for (int j = 1; j < len1 + 1; j++)
                    {
                        arr[1, j] = Min(
                           arr[0, j] + 1,
                           arr[1, j - 1] + 1,
                           arr[0, j - 1] + (str1[j - 1] == str2[i] ? 0 : 1));
                    }
                    //如果大字符串包含小字符串，则返回
                    if (arr[1, 0] - arr[1, len1] == len1)
                        return len2 - len1;
                }
            }
            return len2 % 2 == 0 ? arr[0, len1] : arr[1, len1];
        }
        /// <summary>
        /// 求两个字符串的相似度
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        /// <returns></returns>
        public static double GetStringSimilarity(string str1, string str2)
        {
            int dis = GetLevenshteinDistance(str1, str2);
            int len1 = str1 == null ? 0 : str1.Length;
            int len2 = str2 == null ? 0 : str2.Length;
            double max = Math.Max(len1, len2);
            if (max == 0) return 1;
            else return (max - dis) / max;
        }

        /// <summary>
        /// 截断字符串到指定的长度，如果字符串的长度不够，则不进行截断处理
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string SubStringEx(this string str, int maxLength)
        {
            if (str == null) throw new ArgumentNullException("str");
            return (str.Length > maxLength) ? str.Substring(0, maxLength) : str;
        }
        /// <summary>
        /// 截断字符串到指定的长度，如果字符串的长度不够，则不进行截断处理
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string SubStringEx(this string str, int startIndex, int maxLength)
        {
            if (str == null) throw new ArgumentNullException("str");
            return (str.Length > maxLength + startIndex) ? str.Substring(startIndex, maxLength) : str.Substring(startIndex);
        }
        public static DateTime AsDateTime(this string str, DateTime defaultValue)
        {
            DateTime res;
            return DateTime.TryParse(str, out res) ? res : defaultValue;
        }
        public static DateTime? AsNullableDateTime(this string str)
        {
            DateTime res;
            if (DateTime.TryParse(str, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static DateTime AsDateTime(this string num, string format, DateTime defaultValue)
        {
            DateTime res;
            if (DateTime.TryParseExact(num, format, null, DateTimeStyles.None, out res))
                return res;
            else
                return defaultValue;
        }
        public static DateTime? AsNullableDateTime(this string num, string format)
        {
            DateTime res;
            if (DateTime.TryParseExact(num, format, null, DateTimeStyles.None, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static Guid AsGuid(this string str, Guid defaultValue)
        {
            Guid res;
            return Guid.TryParse(str, out res) ? res : defaultValue;
        }
        public static Guid? AsNullableGuid(this string str)
        {
            Guid res;
            return Guid.TryParse(str, out res) ? new Guid?(res) : null;
        }
        public static bool AsBoolean(this string text, bool defaultValue = false)
        {
            bool res;
            if (bool.TryParse(text, out res))
            {
                return res;
            }
            else
            {
                return defaultValue;
            }
        }
        public static bool? AsNullableBoolean(this string text)
        {
            bool res = false;
            if (bool.TryParse(text, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static int AsInt32(this string num, int defaultValue = 0)
        {
            int res = 0;
            if (int.TryParse(num, out res))
                return res;
            else
                return defaultValue;
        }
        public static int? AsNullableInt32(this string num)
        {
            int res = 0;
            if (int.TryParse(num, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static long AsInt64(this string num, long defaultValue = 0)
        {
            long res = 0;
            if (long.TryParse(num, out res))
                return res;
            else
                return defaultValue;
        }
        public static long? AsNullableInt64(this string num)
        {
            long res = 0;
            if (long.TryParse(num, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static decimal AsDecimal(this string num, decimal defaultValue = 0)
        {
            decimal res = 0;
            if (decimal.TryParse(num, out res))
                return res;
            else
                return defaultValue;
        }
        public static decimal? AsNullableDecimal(this string num)
        {
            decimal res = 0;
            if (decimal.TryParse(num, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static byte AsByte(this string num, byte defaultValue = 0)
        {
            byte res = 0;
            if (byte.TryParse(num, out res))
                return res;
            else
                return defaultValue;
        }
        public static byte? AsNullableByte(this string num)
        {
            byte res = 0;
            if (byte.TryParse(num, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static float AsSingle(this string num, float defaultValue = 0)
        {
            float res = 0;
            if (float.TryParse(num, out res))
                return res;
            else
                return defaultValue;
        }
        public static float? AsNullableSingle(this string num)
        {
            float res = 0;
            if (float.TryParse(num, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }
        public static double AsDouble(this string num, double defaultValue = 0)
        {
            double res = 0;
            if (double.TryParse(num, out res))
                return res;
            else
                return defaultValue;
        }
        public static double? AsNullableDouble(this string num)
        {
            double res = 0;
            if (double.TryParse(num, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Case insensitive version of String.Replace().
        /// </summary>
        /// <param name="input">String that contains patterns to replace</param>
        /// <param name="oldValue">Pattern to find</param>
        /// <param name="newValue">New pattern to replaces old</param>
        /// <param name="comparisonType">String comparison type</param>
        /// <returns></returns>
        public static string Replace(this string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (input == null)
                return null;

            if (String.IsNullOrEmpty(oldValue))
                return input;

            StringBuilder result = new StringBuilder(Math.Min(4096, input.Length));
            int pos = 0;

            while (true)
            {
                int i = input.IndexOf(oldValue, pos, comparisonType);
                if (i < 0)
                    break;

                result.Append(input, pos, i - pos);
                result.Append(newValue);

                pos = i + oldValue.Length;
            }
            result.Append(input, pos, input.Length - pos);

            return result.ToString();
        }
        public static string ReplaceFirst(this string input, string oldValue, string newValue)
        {
            return ReplaceFirst(input, oldValue, newValue, StringComparison.CurrentCulture);
        }
        public static string ReplaceFirst(this string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue))
                return input;
            int i = input.IndexOf(oldValue, comparisonType);
            if (i < 0)
            {
                return input;
            }
            else
            {
                newValue = newValue ?? string.Empty;
                StringBuilder sb = new StringBuilder(input.Length);
                sb.Append(input, 0, i);
                sb.Append(newValue);
                sb.Append(input.Substring(i + oldValue.Length));
                return sb.ToString();
            }
        }
        public static string ReplaceLast(this string input, string oldValue, string newValue)
        {
            return ReplaceLast(input, oldValue, newValue, StringComparison.CurrentCulture);
        }
        public static string ReplaceLast(this string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue))
                return input;
            int i = input.LastIndexOf(oldValue, comparisonType);
            if (i < 0)
            {
                return input;
            }
            else
            {
                newValue = newValue ?? string.Empty;
                StringBuilder sb = new StringBuilder(input.Length);
                sb.Append(input, 0, i);
                sb.Append(newValue);
                sb.Append(input.Substring(i + oldValue.Length));
                return sb.ToString();
            }
        }
        public static string Replace(this string input, Dictionary<string, string> replaces)
        {
            if (!string.IsNullOrEmpty(input) && replaces != null)
            {
                foreach (var v in replaces)
                {
                    input = input.Replace(v.Key, v.Value);
                }
                return input;
            }
            else
            {
                return input;
            }
        }
        public static string Replace(this string input, Dictionary<string, string> replaces, StringComparison comparisonType)
        {
            if (!string.IsNullOrEmpty(input) && replaces != null)
            {
                foreach (var v in replaces)
                {
                    input = input.Replace(v.Key, v.Value, comparisonType);
                }
                return input;
            }
            else
            {
                return input;
            }
        }
        public static string Replace(this string input, Dictionary<char, char> replaces)
        {
            if (!string.IsNullOrEmpty(input) && replaces != null)
            {
                foreach (var v in replaces)
                {
                    input = input.Replace(v.Key, v.Value);
                }
                return input;
            }
            else
            {
                return input;
            }

        }
        public static string Replace(this string input, IEnumerable<char> oldChars, char newChar)
        {
            if (!string.IsNullOrEmpty(input) && oldChars != null)
            {
                foreach (var ch in oldChars)
                {
                    input = input.Replace(ch, newChar);
                }
                return input;
            }
            else
            {
                return input;
            }
        }
        public static string Replace(this string input, IEnumerable<string> oldStrs, string newStr)
        {
            if (!string.IsNullOrEmpty(input) && oldStrs != null)
            {
                foreach (var ch in oldStrs)
                {
                    input = input.Replace(ch, newStr);
                }
                return input;
            }
            else
            {
                return input;
            }

        }
        public static string Replace(this string input, IEnumerable<string> oldStrs, string newStr, StringComparison comparisonType)
        {
            if (!string.IsNullOrEmpty(input) && oldStrs != null)
            {
                foreach (var ch in oldStrs)
                {
                    input = input.Replace(ch, newStr, comparisonType);
                }
                return input;
            }
            else
            {
                return input;
            }

        }
        public static string Remove(this string input, params char[] chs)
        {
            if (!string.IsNullOrEmpty(input))
            {
                StringBuilder sb = new StringBuilder();
                int startindex = 0, index = -1;
                while (startindex < input.Length)
                {
                    index = input.IndexOfAny(chs, startindex);
                    if (index >= 0)
                    {
                        if (index > startindex)
                        {
                            sb.Append(input.Substring(startindex, index - startindex));
                        }
                        startindex = index + 1;
                    }
                    else
                    {
                        sb.Append(input.Substring(startindex));
                        break;
                    }
                }
                return sb.ToString();
            }
            else
            {
                return input;
            }
        }
        public static string Remove(this string input, IEnumerable<string> strs)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.Replace(strs, string.Empty);
            }
            else
            {
                return input;
            }
        }
        public static string Remove(this string input, IEnumerable<string> strs, StringComparison comparisonType)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.Replace(strs, string.Empty, comparisonType);
            }
            else
            {
                return input;
            }
        }
        public static string RemoveSpace(this string input)
        {
            if (input != null)
            {
                return Remove(input, ' ', '\t');
                // return Regex.Replace(input,@"\s",string.Empty);
            }
            else
            {
                return input;
            }
        }
        public static string RemoveLineBreak(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Remove(input, new char[] { '\r', '\n' });
            }
            else
            {
                return input;
            }
        }
        public static string RemoveLineBreakAndSpace(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Remove(input, new char[] { ' ', '\t', '\r', '\n' });
            }
            else
            {
                return input;
            }
        }
        public static bool Contains(this string input, string value, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                return input.IndexOf(value, comparison) >= 0;
            }
        }
        public static string RemoveBetween(this string input, char startCh, char endCh)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int start = input.IndexOf(startCh);
            int end = input.LastIndexOf(endCh);
            if (start >= 0 && end >= 0 && end >= start)
            {
                string s1 = input.Substring(0, start);
                if (end < input.Length - 1)
                {
                    return s1 + input.Substring(end + 1);
                }
                else
                {
                    return s1;
                }
            }
            else
            {
                return input;
            }

        }
        public static string RemoveBetween(this string input, char[] startChars, char[] endChars)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int start = input.IndexOfAny(startChars);
            int end = input.LastIndexOfAny(endChars);
            if (start >= 0 && end >= 0 && end >= start)
            {
                string s1 = input.Substring(0, start);
                if (end < input.Length - 1)
                {
                    return s1 + input.Substring(end + 1);
                }
                else
                {
                    return s1;
                }
            }
            else
            {
                return input;
            }


        }
        public static string SubStringAfter(this string input, params char[] chs)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int index = input.IndexOfAny(chs);
            if (index >= 0)
            {
                if (index < input.Length - 1)
                {
                    return input.Substring(index);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return input;
            }
        }
        public static string SubStringBefore(this string input, params char[] chs)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int index = input.IndexOfAny(chs);
            if (index >= 0)
            {
                return input.Substring(0, index);
            }
            else
            {
                return input;
            }
        }
        public static int? ParseInt32(this string str, int startindex = 0)
        {
            var mat = GetIntegerMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return int.Parse(mat.Value);
            }
            return null;
        }
        public static uint? ParseUInt32(this string str, int startindex = 0)
        {
            var mat = GetIntegerMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return uint.Parse(mat.Value);
            }
            return null;
        }
        public static long? ParseInt64(this string str, int startindex = 0)
        {
            var mat = GetIntegerMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return Int64.Parse(mat.Value);
            }
            return null;
        }
        public static ulong? ParseUInt64(this string str, int startindex = 0)
        {
            var mat = GetIntegerMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return UInt64.Parse(mat.Value);
            }
            return null;
        }
        public static short? ParseInt16(this string str, int startindex = 0)
        {
            var mat = GetIntegerMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return Int16.Parse(mat.Value);
            }
            return null;
        }
        public static ushort? ParseUInt16(this string str, int startindex = 0)
        {
            var mat = GetIntegerMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return UInt16.Parse(mat.Value);
            }
            return null;
        }
        public static decimal? ParseDecimal(this string str, int startindex = 0)
        {
            var mat = GetDecimalMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return decimal.Parse(mat.Value);
            }
            return null;
        }
        public static double? ParseDouble(this string str, int startindex = 0)
        {
            var mat = GetDecimalMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return double.Parse(mat.Value);
            }
            return null;
        }
        public static float? ParseSingle(this string str, int startindex = 0)
        {
            var mat = GetDecimalMatch(str, startindex, false);
            if (mat != null && mat.Success)
            {
                return float.Parse(mat.Value);
            }
            return null;
        }
        public static int? ParseLastInt32(this string str)
        {
            var mat = GetIntegerMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return int.Parse(mat.Value);
            }
            return null;
        }
        public static uint? ParseLastUInt32(this string str)
        {
            var mat = GetIntegerMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return uint.Parse(mat.Value);
            }
            return null;
        }
        public static long? ParseLastInt64(this string str)
        {
            var mat = GetIntegerMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return Int64.Parse(mat.Value);
            }
            return null;
        }
        public static ulong? ParseLastUInt64(this string str)
        {
            var mat = GetIntegerMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return UInt64.Parse(mat.Value);
            }
            return null;
        }
        public static short? ParseLastInt16(this string str)
        {
            var mat = GetIntegerMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return Int16.Parse(mat.Value);
            }
            return null;
        }
        public static ushort? ParseLastUInt16(this string str)
        {
            var mat = GetIntegerMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return UInt16.Parse(mat.Value);
            }
            return null;
        }
        public static decimal? ParseLastDecimal(this string str)
        {
            var mat = GetDecimalMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return decimal.Parse(mat.Value);
            }
            return null;
        }
        public static double? ParseLastDouble(this string str)
        {
            var mat = GetDecimalMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return double.Parse(mat.Value);
            }
            return null;
        }
        public static float? ParseLastSingle(this string str)
        {
            var mat = GetDecimalMatch(str, 0, true);
            if (mat != null && mat.Success)
            {
                return float.Parse(mat.Value);
            }
            return null;
        }
        /// <summary>
        /// 将一个相连的字符串断开来,例如将 UserName 分成 User Name
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SplitJoinedString(this string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "([a-z]+)([A-Z])", "${1} ${2}");
        }
        private static Match GetIntegerMatch(string str, int start, bool righttoleft)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (righttoleft)
            {
                return Regex.Match(str.Substring(start), @"[+-]{0,1}\d+", RegexOptions.RightToLeft);
            }
            else
            {
                return Regex.Match(str.Substring(start), @"[+-]{0,1}\d+");
            }
        }
        private static Match GetDecimalMatch(string str, int start, bool righttoleft)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (righttoleft)
            {
                return Regex.Match(str.Substring(start), @"[+-]{0,1}\d+(\.\d+){0,1}", RegexOptions.RightToLeft);
            }
            else
            {
                return Regex.Match(str.Substring(start), @"[+-]{0,1}\d+(\.\d+){0,1}");
            }
        }

        /// <summary>
        /// 比较输入字符串与通配符模式的是否匹配，将根据每个字符进行比较
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式，允许使用的通配符：?，*，其中? 代表任意一个字符，* 代表零或多个任意字符</param>
        /// <returns></returns>
        public static bool IsMatchWildcard(this string input, string pattern, StringComparison stringcomparison = StringComparison.CurrentCulture)
        {//http://www.codeproject.com/Articles/1088/Wildcard-string-compare-globbing

            if (input == null) throw new ArgumentNullException("input");
            if (input == null) throw new ArgumentNullException("pattern");
            int inputIndex = 0;
            int patternIndex = 0;

            //无通配符 * 时，比较算法（）
            while (inputIndex < input.Length && patternIndex < pattern.Length && (pattern[patternIndex] != '*'))
            {
                //if ((pattern[patternIndex] != '?') && (input[inputIndex] != pattern[patternIndex]))
                if ((pattern[patternIndex] != '?') && !string.Equals(input.Substring(inputIndex, 1), pattern.Substring(patternIndex, 1), stringcomparison))
                {//如果模式字符不是通配符，且输入字符与模式字符不相等，则可判定整个输入字串与模式不匹配
                    return false;
                }
                patternIndex++;
                inputIndex++;
                if (patternIndex == pattern.Length && inputIndex < input.Length)
                {
                    return false;
                }
                if (inputIndex == input.Length && patternIndex < pattern.Length)
                {
                    return false;
                }
                if (patternIndex == pattern.Length && inputIndex == input.Length)
                {
                    return true;
                }
            }
            //有通配符 * 时，比较算法
            int mp = 0;
            int cp = 0;
            while (inputIndex < input.Length)
            {
                if (patternIndex < pattern.Length && pattern[patternIndex] == '*')
                {
                    if (++patternIndex >= pattern.Length)
                    {
                        return true;
                    }
                    mp = patternIndex;
                    cp = inputIndex + 1;
                }
                //else if (patternIndex < pattern.Length && ((pattern[patternIndex] == input[inputIndex]) || (pattern[patternIndex] == '?')))
                else if (patternIndex < pattern.Length && (string.Equals(input.Substring(inputIndex, 1), pattern.Substring(patternIndex, 1), stringcomparison) || (pattern[patternIndex] == '?')))
                {
                    patternIndex++;
                    inputIndex++;
                }
                else
                {
                    patternIndex = mp;
                    inputIndex = cp++;
                }
            }
            //当输入字符为空且模式为*时
            while (patternIndex < pattern.Length && pattern[patternIndex] == '*')
            {
                patternIndex++;
            }
            return patternIndex >= pattern.Length ? true : false;

        }//end mehtod
        /// <summary>
        /// 判断字符串是否匹配多个模式中的任意一个
        /// </summary>
        /// <param name="input"></param>
        /// <param name="patterns"></param>
        /// <param name="stringcomparison"></param>
        /// <returns></returns>
        public static bool IsMatchWildcardAnyOne(this string input, IEnumerable<string> patterns, StringComparison stringcomparison = StringComparison.CurrentCulture)
        {
            if (patterns == null) return false;
            foreach (var pattern in patterns)
            {
                var res = IsMatchWildcard(input, pattern, stringcomparison);
                if (res) return true;
            }
            return false;
        }

        public static bool IsMatchRegex(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return Regex.IsMatch(input, pattern);
        }
        public static bool IsMatchRegex(this string input, string pattern, RegexOptions regexOptions)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return Regex.IsMatch(input, pattern, regexOptions);
        }

        /// <summary>
        /// 将字符串进行编码(等同于javascript中的escape函数)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Escape(this string s)
        {
            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb1.Append("%u");
                sb1.Append(char.ConvertToUtf32(s, i).ToString("X4"));
            }
            return sb1.ToString();
        }
        /// <summary>
        /// 解码由<see cref=""/>Escape编码过的字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Unescape(this string s)
        {
            return Regex.Replace(s, "%u(?<code>[0-9a-fA-F]{4})", (a) =>
            {
                return char.ConvertFromUtf32(Convert.ToInt32(a.Groups["code"].Value, 16));
            });
        }

        ///   <summary> 
        ///   将指定字符串按指定长度进行截取并加上指定的后缀
        ///   </summary> 
        ///   <param   name= "oldStr "> 需要截断的字符串 </param> 
        ///   <param   name= "maxLength "> 字符串的最大长度 </param> 
        ///   <param   name= "endWith "> 超过长度的后缀 </param> 
        ///   <returns> 如果超过长度，返回截断后的新字符串加上后缀，否则，返回原字符串 </returns> 
        public static string Truncat(this string oldStr, int maxLength, string endWith = "...")
        {
            //判断原字符串是否为空
            if (string.IsNullOrEmpty(oldStr))
                return oldStr;


            //返回字符串的长度必须大于1
            if (maxLength < 1)
                throw new ArgumentOutOfRangeException("maxLength", "返回的字符串长度必须大于[0] ");


            //判断原字符串是否大于最大长度
            if (oldStr.Length > maxLength)
            {
                //截取原字符串
                string strTmp = oldStr.Substring(0, maxLength);


                //判断后缀是否为空
                if (string.IsNullOrEmpty(endWith))
                    return strTmp;
                else
                    return strTmp + endWith;
            }
            return oldStr;
        }
        /// <summary>
        /// 将字符串转换为保密的文本
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static System.Security.SecureString ToSecureString(this string str)
        {
            if (str == null) return null;
            SecureString ss = new SecureString();
            foreach (var v in str)
            {
                ss.AppendChar(v);
            }
            return ss;
        }
        #region 判断Json
        //http://www.cnblogs.com/cyq1162/p/3841766.html
        private static bool IsJsonStart(ref string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                json = json.Trim('\r', '\n', ' ');
                if (json.Length > 1)
                {
                    char s = json[0];
                    char e = json[json.Length - 1];
                    return (s == '{' && e == '}') || (s == '[' && e == ']');
                }
            }
            return false;
        }
        public static bool IsJson(this string json)
        {
            int errIndex;
            return IsJson(json, out errIndex);
        }
        public static bool IsJson(this string json, out int errIndex)
        {
            errIndex = 0;
            if (IsJsonStart(ref json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (SetCharState(c, ref cs) && cs.childrenStart)//设置关键符号状态。
                    {
                        string item = json.Substring(i);
                        int err;
                        int length = GetValueLength(item, true, out err);
                        cs.childrenStart = false;
                        if (err > 0)
                        {
                            errIndex = i + err;
                            return false;
                        }
                        i = i + length - 1;
                    }
                    if (cs.isError)
                    {
                        errIndex = i;
                        return false;
                    }
                }

                return !cs.arrayStart && !cs.jsonStart;
            }
            return false;
        }

        /// <summary>
        /// 获取值的长度（当Json值嵌套以"{"或"["开头时）
        /// </summary>
        private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
        {
            errIndex = 0;
            int len = 0;
            if (!string.IsNullOrEmpty(json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (!SetCharState(c, ref cs))//设置关键符号状态。
                    {
                        if (!cs.jsonStart && !cs.arrayStart)//json结束，又不是数组，则退出。
                        {
                            break;
                        }
                    }
                    else if (cs.childrenStart)//正常字符，值状态下。
                    {
                        int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//递归子值，返回一个长度。。。
                        cs.childrenStart = false;
                        cs.valueStart = 0;
                        //cs.state = 0;
                        i = i + length - 1;
                    }
                    if (breakOnErr && cs.isError)
                    {
                        errIndex = i;
                        return i;
                    }
                    if (!cs.jsonStart && !cs.arrayStart)//记录当前结束位置。
                    {
                        len = i + 1;//长度比索引+1
                        break;
                    }
                }
            }
            return len;
        }
        /// <summary>
        /// 字符状态
        /// </summary>
        private class CharState
        {
            internal bool jsonStart = false;//以 "{"开始了...
            internal bool setDicValue = false;// 可以设置字典值了。
            internal bool escapeChar = false;//以"\"转义符号开始了
            /// <summary>
            /// 数组开始【仅第一开头才算】，值嵌套的以【childrenStart】来标识。
            /// </summary>
            internal bool arrayStart = false;//以"[" 符号开始了
            internal bool childrenStart = false;//子级嵌套开始了。
            /// <summary>
            /// 【0 初始状态，或 遇到“,”逗号】；【1 遇到“：”冒号】
            /// </summary>
            internal int state = 0;

            /// <summary>
            /// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
            /// </summary>
            internal int keyStart = 0;
            /// <summary>
            /// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
            /// </summary>
            internal int valueStart = 0;
            internal bool isError = false;//是否语法错误。

            internal void CheckIsError(char c)//只当成一级处理（因为GetLength会递归到每一个子项处理）
            {
                if (keyStart > 1 || valueStart > 1)
                {
                    return;
                }
                //示例 ["aa",{"bbbb":123,"fff","ddd"}] 
                switch (c)
                {
                    case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        isError = jsonStart && state == 0;//重复开始错误 同时不是值处理。
                        break;
                    case '}':
                        isError = !jsonStart || (keyStart != 0 && state == 0);//重复结束错误 或者 提前结束{"aa"}。正常的有{}
                        break;
                    case '[':
                        isError = arrayStart && state == 0;//重复开始错误
                        break;
                    case ']':
                        isError = !arrayStart || jsonStart;//重复开始错误 或者 Json 未结束
                        break;
                    case '"':
                    case '\'':
                        isError = !(jsonStart || arrayStart); //json 或数组开始。
                        if (!isError)
                        {
                            //重复开始 [""",{"" "}]
                            isError = (state == 0 && keyStart == -1) || (state == 1 && valueStart == -1);
                        }
                        if (!isError && arrayStart && !jsonStart && c == '\'')//['aa',{}]
                        {
                            isError = true;
                        }
                        break;
                    case ':':
                        isError = !jsonStart || state == 1;//重复出现。
                        break;
                    case ',':
                        isError = !(jsonStart || arrayStart); //json 或数组开始。
                        if (!isError)
                        {
                            if (jsonStart)
                            {
                                isError = state == 0 || (state == 1 && valueStart > 1);//重复出现。
                            }
                            else if (arrayStart)//["aa,] [,]  [{},{}]
                            {
                                isError = keyStart == 0 && !setDicValue;
                            }
                        }
                        break;
                    case ' ':
                    case '\r':
                    case '\n'://[ "a",\r\n{} ]
                    case '\0':
                    case '\t':
                        break;
                    default: //值开头。。
                        isError = (!jsonStart && !arrayStart) || (state == 0 && keyStart == -1) || (valueStart == -1 && state == 1);//
                        break;
                }
                //if (isError)
                //{

                //}
            }
        }
        /// <summary>
        /// 设置字符状态(返回true则为关键词，返回false则当为普通字符处理）
        /// </summary>
        private static bool SetCharState(char c, ref CharState cs)
        {
            cs.CheckIsError(c);
            switch (c)
            {
                case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                    #region 大括号
                    if (cs.keyStart <= 0 && cs.valueStart <= 0)
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        if (cs.jsonStart && cs.state == 1)
                        {
                            cs.childrenStart = true;
                        }
                        else
                        {
                            cs.state = 0;
                        }
                        cs.jsonStart = true;//开始。
                        return true;
                    }
                    #endregion
                    break;
                case '}':
                    #region 大括号结束
                    if (cs.keyStart <= 0 && cs.valueStart < 2 && cs.jsonStart)
                    {
                        cs.jsonStart = false;//正常结束。
                        cs.state = 0;
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.setDicValue = true;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart && cs.state == 0;
                    #endregion
                    break;
                case '[':
                    #region 中括号开始
                    if (!cs.jsonStart)
                    {
                        cs.arrayStart = true;
                        return true;
                    }
                    else if (cs.jsonStart && cs.state == 1)
                    {
                        cs.childrenStart = true;
                        return true;
                    }
                    #endregion
                    break;
                case ']':
                    #region 中括号结束
                    if (cs.arrayStart && !cs.jsonStart && cs.keyStart <= 2 && cs.valueStart <= 0)//[{},333]//这样结束。
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.arrayStart = false;
                        return true;
                    }
                    #endregion
                    break;
                case '"':
                case '\'':
                    #region 引号
                    if (cs.jsonStart || cs.arrayStart)
                    {
                        if (cs.state == 0)//key阶段,有可能是数组["aa",{}]
                        {
                            if (cs.keyStart <= 0)
                            {
                                cs.keyStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.keyStart == 2 && c == '\'') || (cs.keyStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.keyStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }
                        }
                        else if (cs.state == 1 && cs.jsonStart)//值阶段必须是Json开始了。
                        {
                            if (cs.valueStart <= 0)
                            {
                                cs.valueStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.valueStart == 2 && c == '\'') || (cs.valueStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.valueStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }

                        }
                    }
                    #endregion
                    break;
                case ':':
                    #region 冒号
                    if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 0)
                    {
                        if (cs.keyStart == 1)
                        {
                            cs.keyStart = -1;
                        }
                        cs.state = 1;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);
                    #endregion
                    break;
                case ',':
                    #region 逗号 //["aa",{aa:12,}]

                    if (cs.jsonStart)
                    {
                        if (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1)
                        {
                            cs.state = 0;
                            cs.keyStart = 0;
                            cs.valueStart = 0;
                            //if (cs.valueStart == 1)
                            //{
                            //    cs.valueStart = 0;
                            //}
                            cs.setDicValue = true;
                            return true;
                        }
                    }
                    else if (cs.arrayStart && cs.keyStart <= 2)
                    {
                        cs.keyStart = 0;
                        //if (cs.keyStart == 1)
                        //{
                        //    cs.keyStart = -1;
                        //}
                        return true;
                    }
                    #endregion
                    break;
                case ' ':
                case '\r':
                case '\n'://[ "a",\r\n{} ]
                case '\0':
                case '\t':
                    if (cs.keyStart <= 0 && cs.valueStart <= 0) //cs.jsonStart && 
                    {
                        return true;//跳过空格。
                    }
                    break;
                default: //值开头。。
                    if (c == '\\') //转义符号
                    {
                        if (cs.escapeChar)
                        {
                            cs.escapeChar = false;
                        }
                        else
                        {
                            cs.escapeChar = true;
                            return true;
                        }
                    }
                    else
                    {
                        cs.escapeChar = false;
                    }
                    if (cs.jsonStart || cs.arrayStart) // Json 或数组开始了。
                    {
                        if (cs.keyStart <= 0 && cs.state == 0)
                        {
                            cs.keyStart = 1;//无引号的
                        }
                        else if (cs.valueStart <= 0 && cs.state == 1 && cs.jsonStart)//只有Json开始才有值。
                        {
                            cs.valueStart = 1;//无引号的
                        }
                    }
                    break;
            }
            return false;
        }
        #endregion

        public static string ToMd5Hash(this string str, MD5HashLength hashLength = MD5HashLength.Lenth32)
        {
            return System.Security.HashUtility.ToMd5HashString(str, hashLength);
        }

        public static int ToFixedHashCode(this string str)
        {
            var bys = HashUtility.ToMd5HashBytes(str ?? "");

            var length = bys.Length / 4 * 4;
            int res = 0;
            for (int i = 0; i < length; i += 4)
            {
                res = res ^ BitConverter.ToInt32(bys, i);
            }
            return res;
        }
        public static Guid ToGuid(this string str)
        {
            str = str ?? string.Empty;
            Guid res = Guid.Empty;
            if (Guid.TryParse(str, out res))
            {
                return res;
            }
            else
            {
                var bys = HashUtility.ToMd5HashBytes(str);
                if (bys.Length > 16) bys = bys.SubArray(16);
                
                return new Guid(bys);
            }
        }
        public static string ToStyle(this string str, StringStyle style)
        {
            if (string.IsNullOrEmpty(str)) return str;
            switch (style)
            {
                case StringStyle.TitleCase:
                    return str.ToTitleCase();
                case StringStyle.Lower:
                    return str.ToLower();
                case StringStyle.Upper:
                    return str.ToUpper();
                default:
                    return str;
            }
        }
        public static string ToStyle(this string str, StringStyle style, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(str)) return str;
            switch (style)
            {
                case StringStyle.TitleCase:
                    return str.ToTitleCase(culture);
                case StringStyle.Lower:
                    return str.ToLower(culture);
                case StringStyle.Upper:
                    return str.ToUpper(culture);
                default:
                    return str;
            }
        }
        //public static FlagData<T> TryParse<T>(this string str)
        //{
        //    //TODO....
        //    return new FlagData<T>();
        //    switch (Type.GetTypeCode(typeof(T)))
        //    {
        //        case TypeCode.Boolean:

        //            break;
        //        case TypeCode.Byte:
        //            break;
        //        case TypeCode.Char:
        //            break;
        //        case TypeCode.DBNull:
        //            break;
        //        case TypeCode.DateTime:
        //            break;
        //        case TypeCode.Decimal:
        //            break;
        //        case TypeCode.Double:
        //            break;
        //        case TypeCode.Empty:
        //            break;
        //        case TypeCode.Int16:
        //            break;
        //        case TypeCode.Int32:
        //            break;
        //        case TypeCode.Int64:
        //            break;
        //        case TypeCode.Object:
        //            break;
        //        case TypeCode.SByte:
        //            break;
        //        case TypeCode.Single:
        //            break;
        //        case TypeCode.String:
        //            break;
        //        case TypeCode.UInt16:
        //            break;
        //        case TypeCode.UInt32:
        //            break;
        //        case TypeCode.UInt64:
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
    public enum StringStyle
    {
        None,
        TitleCase,
        Lower,
        Upper
    }
}
