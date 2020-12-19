using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data
{
    public static class Hex36Convert
    {
        static readonly char[] Chs;
        static readonly Dictionary<char, int> Dic;
        static Hex36Convert()
        {
            Chs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Dic = new Dictionary<char, int>(36);
            for (int i = 0; i < Chs.Length; i++)
            {
                if (char.IsDigit(Chs[i]))
                {
                    Dic.Add(Chs[i], i);
                }
                else
                {
                    Dic.Add(Chs[i], i);
                    Dic.Add(char.ToLower(Chs[i]), i);
                }
            }

        }
        /// <summary>
        /// 将十进制数字转为三十六进制数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ConvertDecToHex36(int num)
        {
            StringBuilder sb = new StringBuilder();
            bool flag = num < 0;
            num = Math.Abs(num);
            do
            {
                sb.Insert(0, Chs[num % 36]);
                num /= 36;
            }
            while (num > 0);
            if (flag) sb.Insert(0, '-');
            return sb.ToString();
        }
        /// <summary>
        /// 将三十六进制数字转为十进制数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int ConvertHex36ToDec(string num)
        {
            int res = 0;
            num = (num ?? string.Empty).Trim();
            int start = 0;
            bool flag = false;//是否是负数
            while (start < num.Length && (num[start] == '+' || num[start] == '-'))
            {
                if (num[start] == '-') flag = !flag;
                start++;
            }
            for (int i = start; i < num.Length; i++)
            {
                res = res * 36 + Dic[num[i]];
            }

            return res * (flag ? -1 : 1);
        }
    }

}
