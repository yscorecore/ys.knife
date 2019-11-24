using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Globalization
{
    /// <summary>
    /// 表示中国居民身份证
    /// </summary>
    public static class ChineseIdentityCard
    {
        private static char GetVerifyCode(string cardno)
        {
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += (int)(cardno[i] - '0') * iW[i];
            }
            return szVerCode[sum % 11];
        }
        public static bool IsVerifyIdCard18(string cardno)
        {
            if (cardno == null) return false;
            if (cardno.Length != 18) return false;
            cardno = cardno.ToUpper();
            return GetVerifyCode(cardno) == cardno[17];
        }

        private static char[] szVerCode = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
        private static int[] iW = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

    }
}
